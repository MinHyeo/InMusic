using Fusion;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections;

namespace Play
{
    public class MultiNoteSpawner : NetworkBehaviour
    {
        [SerializeField] private NetworkPrefabRef _multiNote1Prefab;
        [SerializeField] private NetworkPrefabRef _multiNote2Prefab;

        [SerializeField] private Transform _visualsRoot;
        public Transform VisualsRoot => _visualsRoot;
        [SerializeField] private Transform[] _noteSpawnPoints;
        [SerializeField] private Transform _judgementLine;

        public static Dictionary<PlayerRef, MultiNoteSpawner> Spawners = new Dictionary<PlayerRef, MultiNoteSpawner>();

        [Networked] public PlayerRef Owner { get; set; }
        private readonly List<MultiNote> _myNotes = new List<MultiNote>(); // 이제 각 스포너는 '자신의' 노트만 관리합니다.
        private readonly List<MultiNote> _allNotes = new List<MultiNote>(); // 모든 노트를 추적하는 리스트

        private const int GreateTickThreshold = 3;
        private const int GoodTickThreshold = 6;
        private const int BadTickThreshold = 11;

        private Vector3 offsetPosition = new Vector3(-5.148f, 0, 0);

        public override void Spawned()
        {
            StartCoroutine(WaitForManagerAndCheckIn());

            Spawners[Owner] = this;
            _visualsRoot.position += offsetPosition;
            if (Owner != Runner.LocalPlayer)
            {
                _visualsRoot.position *= -1;
            }
        }

        private IEnumerator WaitForManagerAndCheckIn()
        {
            // MultiPlayManager.Instance가 null이 아닐 때까지 매 프레임 기다립니다.
            while (MultiPlayManager.Instance == null)
            {
                yield return null;
            }

            // Manager가 준비된 것을 확인했으므로, 이제 안전하게 호출합니다.
            MultiPlayManager.Instance.CheckAndStartGame(this);
        }

        /// <summary>
        /// TimelineController로부터 악보를 받아, 자신의 노트 생성을 시작하는 새로운 함수입니다.
        /// </summary>
        public void StartNoteGeneration(SongInfo songInfo, int gameStartTickRaw, float speed, ref int noteId)
        {
            if (!Object.HasStateAuthority) return;

            float bpm = songInfo.BPM;
            float measureIntervalSec = (60f / bpm) * 4f;

            foreach (var noteData in songInfo.NoteList)
            {
                float barTime = noteData.bar * measureIntervalSec;
                int divisions = noteData.noteData.Count;
                float noteScore = 100000f / songInfo.NoteCount; // 노트 점수 계산

                for (int i = 0; i < divisions; i++)
                {
                    if (noteData.noteData[i] == 0) continue;

                    float noteAppearTime = barTime + (measureIntervalSec / divisions) * i;
                    int noteAppearOffsetTicks = (int)(noteAppearTime * Runner.TickRate);
                    int targetTickRaw = gameStartTickRaw + noteAppearOffsetTicks;

                    // 이제 스포너는 자기 자신만의 노트를 생성합니다.
                    SpawnNote(noteId, noteData.channel, noteScore, speed, targetTickRaw);
                    noteId++; // ID가 중복되지 않도록 여기서 증가시킵니다.
                }
            }
        }

        private void SpawnNote(int noteId, int channel, float noteScore, float speed, int targetTickRaw)
        {
            float distance = _noteSpawnPoints[channel - 11].position.y - _judgementLine.position.y;
            float travelTime = distance / speed;
            int travelTicks = (int)(travelTime * Runner.TickRate);
            int spawnTickRaw = targetTickRaw - travelTicks;

            Vector3 spawnPosition = _noteSpawnPoints[channel - 11].localPosition;

            var prefab = _multiNote1Prefab;
            switch (channel)
            {
                case 11:
                case 14:
                    prefab = _multiNote1Prefab;
                    break;
                case 12:
                case 13:
                    prefab = _multiNote2Prefab;
                    break;
                default:
                    Debug.LogError($"Invalid channel: {channel}");
                    return;
            }

            Runner.Spawn(prefab, spawnPosition, Quaternion.identity, null, (runner, obj) =>
            {
                var note = obj.GetComponent<MultiNote>();
                note.Init(Owner, noteId, channel, noteScore, speed, targetTickRaw, spawnTickRaw);
                _allNotes.Add(note);
                if (Owner == Runner.LocalPlayer) _myNotes.Add(note); // 자신의 노트 리스트에 추가
            });
        }

        public override void FixedUpdateNetwork()
        {
            if (!Object.HasStateAuthority) return;

            for (int i = _allNotes.Count - 1; i >= 0; i--)
            {
                MultiNote note = _allNotes[i];
                if (note == null)
                {
                    _allNotes.RemoveAt(i);
                    continue;
                }

                // 1. 노트 활성화 체크
                // 노트가 '준비' 상태이고, 현재 시간이 출발 시간을 지났다면 '활성' 상태로 변경
                if (note.CurrentState == NoteState.Pending && Runner.Tick.Raw >= note.SpawnTickRaw)
                {
                    note.Activate();
                }

                // 2. Miss 판정 체크
                // 노트가 '활성' 상태이고, 판정 시간을 훌쩍 지났다면 'Miss' 처리
                if (note.CurrentState == NoteState.Active && Runner.Tick.Raw > note.TargetTickRaw + 10)
                {
                    note.ProcessMiss();
                    MultiPlayManager.Instance.RPC_BroadcastHitResult(Owner, AccuracyType.Miss, 0, note.Channel);
                    _allNotes.RemoveAt(i); // 처리된 노트는 목록에서 제거
                }
            }
        }

        public void ProcessPlayerInput(int pressTickRaw, int channel, PlayerRef player)
        {
            if (!Object.HasStateAuthority) return;
            MultiNote targetNote = FindClosestActiveNote(channel, pressTickRaw, player);
            if (targetNote == null || targetNote.CurrentState != NoteState.Active) return;

            var tickDifference = Math.Abs(pressTickRaw - targetNote.TargetTickRaw);
            AccuracyType accuracy;

            if (tickDifference <= GreateTickThreshold) accuracy = AccuracyType.Great;
            else if (tickDifference <= GoodTickThreshold) accuracy = AccuracyType.Good;
            else if (tickDifference <= BadTickThreshold) accuracy = AccuracyType.Bad;
            else return;

            targetNote.ProcessHit(player, accuracy);
            MultiPlayManager.Instance.RPC_BroadcastHitResult(player, accuracy, targetNote.Score, channel);
        }

        private MultiNote FindClosestActiveNote(int channel, int pressTickRaw, PlayerRef player)
        {
            MultiNote closestNote = null;
            var minTickDifference = int.MaxValue;
            foreach (var note in _allNotes)
            {
                if (note.CurrentState != NoteState.Active) continue;
                if (note.Channel != channel) continue;

                var tickDifference = Math.Abs(pressTickRaw - note.TargetTickRaw);
                if (tickDifference < minTickDifference)
                {
                    minTickDifference = tickDifference;
                    closestNote = note;
                }
            }

            if (closestNote != null && closestNote.Owner == player)
            {
                return closestNote;
            }

            return null;
        }
        
        private void OnDestroy()
        {
            if (Spawners.ContainsKey(Owner) && Spawners[Owner] == this)
            {
                Spawners.Remove(Owner);
            }
        }
    }
}
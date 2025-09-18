using System.Collections;
using Fusion;
using UnityEngine;

namespace Play
{
    public enum NoteState : byte { Pending, Active, Hit, Missed }
    
    [RequireComponent(typeof(NetworkTransform))]
    public class MultiNote : NetworkBehaviour
    {
        [Networked] public PlayerRef Owner { get; private set; }
        [Networked] private byte SyncedState { get; set; }
        [Networked] public int NoteId { get; private set; }
        [Networked] public int Channel { get; private set; }
        [Networked] public float Score { get; private set; }
        [Networked] public int TargetTickRaw { get; private set; }
        [Networked] public int SpawnTickRaw { get; private set; }
        [Networked] private PlayerRef SyncedHittingPlayer { get; set; }
        [Networked] private byte SyncedHitAccuracy { get; set; }

        public NoteState CurrentState { get => (NoteState)SyncedState; private set => SyncedState = (byte)value; }
        public PlayerRef HittingPlayer { get => SyncedHittingPlayer; private set => SyncedHittingPlayer = value; }
        public AccuracyType HitAccuracy { get => (AccuracyType)SyncedHitAccuracy; private set => SyncedHitAccuracy = (byte)value; }

        private float _speed;
        private ChangeDetector _changeDetector;

        public void Init(PlayerRef owner, int noteId, int channel, float noteScore, float speed, int targetTickRaw, int spawnTickRaw)
        {
            Owner = owner;
            NoteId = noteId;
            Channel = channel;
            TargetTickRaw = targetTickRaw;
            Score = noteScore;
            SpawnTickRaw = spawnTickRaw;
            _speed = speed;
            CurrentState = NoteState.Pending;
            HittingPlayer = PlayerRef.None;
            HitAccuracy = AccuracyType.Miss;
        }

        public override void Spawned()
        {
            StartCoroutine(WaitAndSetParent());
            _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
        }

        private IEnumerator WaitAndSetParent()
        {
            MultiNoteSpawner spawner;
            while (!MultiNoteSpawner.Spawners.TryGetValue(Owner, out spawner))
            {
                yield return null;
            }
            transform.SetParent(spawner.VisualsRoot, false);
        }

        public override void FixedUpdateNetwork()
        {
            if (Object.HasStateAuthority)
            {
                // '활성' 상태일 때만 노트를 움직입니다.
                if (CurrentState == NoteState.Active)
                {
                    transform.position += Vector3.down * _speed * Runner.DeltaTime;

                    if (Runner.Tick.Raw > TargetTickRaw + 10)
                    {
                        CurrentState = NoteState.Missed;
                        MultiPlayManager.Instance.RPC_BroadcastHitResult(HittingPlayer, HitAccuracy, 0, Channel);
                    }
                }
            }
        }
        
        // Render() 메서드는 더 이상 상태 감지를 위해 필요하지 않습니다.
        public override void Render()
        {
            foreach (var changedPropertyName in _changeDetector.DetectChanges(this))
            {
                if (changedPropertyName == nameof(SyncedState))
                {
                    HandleStateChange();
                }
            }
        }

        private void HandleStateChange()
        {
            switch (CurrentState)
            {
                case NoteState.Pending:
                case NoteState.Hit:
                case NoteState.Missed:
                    gameObject.SetActive(false);
                    break;
                case NoteState.Active:
                    gameObject.SetActive(true);
                    break;
            }
        }

        public void ProcessHit(PlayerRef player, AccuracyType accuracy)
        {
            if (!Object.HasStateAuthority || CurrentState != NoteState.Active) return;

            HittingPlayer = player;
            HitAccuracy = accuracy;
            CurrentState = NoteState.Hit;
        }

        public void ProcessMiss()
        {
            if (!Object.HasStateAuthority || CurrentState != NoteState.Active) return;
            CurrentState = NoteState.Missed;
        }

        public void Activate()
        {
            if (Object.HasStateAuthority)
            {
                CurrentState = NoteState.Active;
            }
        }
    }
}
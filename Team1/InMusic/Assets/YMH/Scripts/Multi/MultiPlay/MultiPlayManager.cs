using Fusion;
using UnityEngine;
using System.Linq;
using SongList;
using System.Collections.Generic;

namespace Play
{
    public class MultiPlayManager : NetworkBehaviour
    {
        public static MultiPlayManager Instance { get; private set; }

        [Header("네트워크 프리팹")]
        [Tooltip("각 플레이어의 노트 세트를 생성하고 관리할 스포너 프리팹")]
        [SerializeField] private MultiNoteSpawner _playerNoteSpawnerPrefab;

        [Header("핵심 Scripts")]
        [SerializeField] private MultiTimelineController multiTimelineController;
        [SerializeField] private MatchController matchController; // 상대방 관련 처리를 위해 추가
        
        [Header("UI 및 게임 로직")]
        [SerializeField] private HpBar myHpBar;
        [SerializeField] private GameObject deathText;
        [SerializeField] private VideoPlay videoPlay;
        [SerializeField] private GameObject[] keyObjects;
        [SerializeField] private GameObject[] keyEffectObjects;
        
        private readonly List<MultiNoteSpawner> _spawners = new List<MultiNoteSpawner>();
        private bool _isGameStarted = false;
        private string _songName;
        private string _artist;

        #region Unity & Fusion Callbacks
        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        public override void Spawned()
        {
            if (Object.HasStateAuthority)
            {
                // 기존 스포너 정리
                foreach (var oldSpawner in FindObjectsOfType<MultiNoteSpawner>())
                {
                    if(oldSpawner.Object != null && oldSpawner.Object.IsValid)
                        Runner.Despawn(oldSpawner.Object);
                }

                // 각 플레이어에 대해 스포너 생성
                foreach (var player in Runner.ActivePlayers)
                {
                    Runner.Spawn(_playerNoteSpawnerPrefab, Vector3.zero, Quaternion.identity, player, (runner, obj) =>
                    {
                        var spawner = obj.GetComponent<MultiNoteSpawner>();
                        spawner.Owner = player;
                    });
                }
            }

            InitializeUI();
            GameManager.Input.SetNoteKeyPressEvent(OnKeyPress);
            GameManager.Input.SetNoteKeyReleaseEvent(OnKeyRelease);
            GameManager.Input.SetUIKeyEvent(LeaveMultiPlay);
            deathText.SetActive(false);
        }

        public override void FixedUpdateNetwork()
        {
            // 호스트만 게임 시작 시간을 체크합니다.
            if (Object.HasStateAuthority)
            {
                if (!_isGameStarted && multiTimelineController.GameStartTickRaw > 0 && Runner.Tick.Raw >= multiTimelineController.GameStartTickRaw)
                {
                    _isGameStarted = true;
                    // 모든 클라이언트에게 게임 시작을 알립니다.
                    RPC_StartGame();
                }
            }
        }

        private void OnDestroy()
        {
            if (GameManager.Input != null)
            {
                GameManager.Input.RemoveNoteKeyEvent(OnKeyPress);
                GameManager.Input.RemoveNoteKeyReleaseEvent(OnKeyRelease);
                GameManager.Input.RemoveUIKeyEvent(LeaveMultiPlay);
            }
        }
        #endregion

        #region Game Start Logic
        // 모든 스포너가 준비되면 호스트가 이 함수를 호출합니다.
        public void CheckAndStartGame(MultiNoteSpawner spawner)
        {
            if(!Object.HasStateAuthority) return;

            if(!_spawners.Contains(spawner))
                _spawners.Add(spawner);

            if (_spawners.Count == Runner.ActivePlayers.Count())
            {
                Debug.Log("모든 플레이어의 스포너 준비 완료! 게임을 시작합니다.");
                _songName = MultiRoomManager.Instance.songName;
                _artist = MultiRoomManager.Instance.artist;
                SongInfo loadedSong = LoadManager.Instance.Songs.FirstOrDefault(s => s.Title == _songName);
                if (loadedSong != null)
                {
                    multiTimelineController.StartGame(loadedSong, _spawners);
                }
            }
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void RPC_StartGame()
        {
            // 모든 클라이언트가 음악과 비디오를 재생합니다.
            _songName = MultiRoomManager.Instance.songName;
            _artist = MultiRoomManager.Instance.artist;
            StartCoroutine(PlayMusicAndVideo());
        }

        private System.Collections.IEnumerator PlayMusicAndVideo()
        {
            SoundManager.Instance.SongInit(_songName, PlayStyle.Normal);
            SoundManager.Instance.Play();
            videoPlay.GetVideoClip(_songName);
            videoPlay.Play();

            // 호스트만 음악 종료를 감지하고 게임 종료를 호출합니다.
            yield return null;
            if (Object.HasStateAuthority)
            {
                StartCoroutine(SoundManager.Instance.WaitForMusicEnd(() => End()));
            }
        }
        #endregion

        #region Input & Hit Logic
        private void OnKeyPress(Define.NoteControl keyEvent)
        {
            keyObjects[(int)keyEvent - (int)Define.NoteControl.Key1].SetActive(true);
            RPC_RequestHit((int)keyEvent, Runner.Tick.Raw);
        }

        private void OnKeyRelease(Define.NoteControl keyEvent)
        {
            keyObjects[(int)keyEvent - (int)Define.NoteControl.Key1].SetActive(false);
        }

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        private void RPC_RequestHit(int channel, int pressTickRaw, RpcInfo info = default)
        {
            foreach (var spawner in _spawners)
            {
                if (spawner.Owner == info.Source)
                {
                    spawner.ProcessPlayerInput(pressTickRaw, channel, info.Source);
                    break;
                }
            }
        }
        
        // 이 함수는 MultiNoteSpawner의 ProcessPlayerInput에서 호출됩니다.
        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        public void RPC_BroadcastHitResult(PlayerRef hitPlayer, AccuracyType accuracy, float score ,int channel)
        {
            // 내가 친 노트라면 내 HP와 점수를 올립니다.
            if (hitPlayer == Runner.LocalPlayer)
            {
                MultiScoreComparison.Instance.UpdateMyScore(score, accuracy);
                if (accuracy != AccuracyType.Miss)
                {
                    keyEffectObjects[channel - 11].SetActive(true);
                    myHpBar.SetHp(5);
                }
                else
                {
                    if (myHpBar.SetHp(-10))
                    {
                        deathText.SetActive(true);
                    }
                }
            }
            else // 상대가 친 노트라면 MatchController를 통해 이펙트를 보여줍니다.
            {
                MatchController.Instance.ShowOpponentHitResult(accuracy, channel);
                MultiScoreComparison.Instance.UpdateMatchScore(score, accuracy);
            }
        }
        #endregion

        #region Game State (Leave, End)
        private void InitializeUI()
        {
            // UI 초기화 로직
            foreach (var pRef in Runner.ActivePlayers)
            {
                var obj = Runner.GetPlayerObject(pRef);
                if (obj == null) continue;

                bool isMy = pRef == Runner.LocalPlayer;
                var playerInfo = obj.GetComponent<PlayerStateController>();
                MultiPlayUserSetting.Instance.SetUserSetting(playerInfo.Nickname, playerInfo.IsRed, isMy);
            }

            foreach (var keyObj in keyObjects)
            {
                keyObj.SetActive(false);
            }
            foreach (var keyEffectObj in keyEffectObjects)
            {
                keyEffectObj.SetActive(false);
            }
        }

        private void LeaveMultiPlay(Define.UIControl keyEvent)
        {
            if (keyEvent != Define.UIControl.Esc) return;
            
            SoundManager.Instance.End();
            videoPlay.End();
            GameManager.Instance.SetGameState(GameState.MultiLobby);
            
            // 이 RPC는 모든 플레이어(나 포함)에게 전달됩니다.
            RPC_PlayerLeft(Runner.LocalPlayer);
            
            // 씬 로드는 RPC 호출 후에 합니다.
            Runner.LoadScene("MultiLobbyScene_InMusic");
        }
        
        [Rpc(RpcSources.All, RpcTargets.All)]
        private void RPC_PlayerLeft(PlayerRef leftPlayer)
        {
            // 나간 플레이어가 내가 아니라면, UI 처리
            if (leftPlayer != Runner.LocalPlayer && matchController != null)
            {
                matchController.OnPlayerLeft();
            }

            // 호스트는 나간 플레이어의 스포너를 제거합니다.
            if(Object.HasStateAuthority)
            {
                MultiNoteSpawner spawnerToRemove = _spawners.FirstOrDefault(s => s.Owner == leftPlayer);
                if (spawnerToRemove != null)
                {
                    _spawners.Remove(spawnerToRemove);
                    if(spawnerToRemove.Object != null && spawnerToRemove.Object.IsValid)
                        Runner.Despawn(spawnerToRemove.Object);
                }
            }
        }

        private void End()
        {
            if(!Object.HasStateAuthority) return;

            Debug.Log("Game Ended by Host");
            RPC_EndGame();
        }
        
        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void RPC_EndGame()
        {
            SoundManager.Instance.End();
            videoPlay.End();

            // 게임이 끝났으므로 입력 이벤트를 제거합니다.
            if (GameManager.Input != null)
            {
                GameManager.Input.RemoveNoteKeyEvent(OnKeyPress);
                GameManager.Input.RemoveNoteKeyReleaseEvent(OnKeyRelease);
            }

            GameManager.Instance.SetGameState(GameState.MultiRoom);
            ScoreData[] scoreDatas = MultiScoreComparison.Instance.SetScore(_songName, _artist);
            MultiRoomManager.Instance.scoreDatas = scoreDatas;
            Runner.LoadScene("MultiRoomScene_InMusic");
        }
        #endregion
    }
}


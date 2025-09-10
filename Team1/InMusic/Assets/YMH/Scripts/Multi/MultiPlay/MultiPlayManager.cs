using Fusion;
using UnityEngine;
using System.Collections;
using System.Linq;
using SongList;

namespace Play
{
    public class MultiPlayManager : NetworkBehaviour
    {
        #region Singleton
        // 싱글톤
        public static MultiPlayManager Instance { get; private set; }
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }
        #endregion

        #region Fields
        [Header("관련 Scripts")]
        [SerializeField]
        private MatchController matchController;
        [SerializeField]
        private HpBar myHpBar;
        [SerializeField]
        private GameObject deathText;
        [SerializeField]
        private VideoPlay videoPlay;

        [Header("Key Objects")]
        [SerializeField]
        private GameObject[] keyObjects;

        [Header("키 입력 Effect")]
        [SerializeField]
        private GameObject[] keyEffectObjects;

        private string songName;
        private string artist;
        private bool isOpponentLeft = false;

        // 판정 기준
        private const float greateThreshold = 0.0533f;
        private const float goodThreshold = 0.0416f;
        private const float badThreshold = 0.0832f;
        private const float missThreshold = 0.0416f;
        #endregion

        #region Init
        private void Start()
        {
            var runner = NetworkManager.runnerInstance;
            runner.SetPlayerObject(runner.LocalPlayer, Object);

            // 멀티플레이 게임으로 상태 변경
            GameManager.Instance.SetGameState(GameState.MultiGamePlay);

            // 노래 제목, 가수 불러오기
            songName = MultiRoomManager.Instance.songName;
            artist = MultiRoomManager.Instance.artist;

            // User UI 설정
            foreach (var pRef in runner.ActivePlayers)
            {
                var obj = runner.GetPlayerObject(pRef);
                if (obj == null)
                    continue;

                bool isMy = pRef == runner.LocalPlayer;
                var playerInfo = obj.GetComponent<PlayerStateController>();
                string otherName = playerInfo.Nickname;
                bool isRed = playerInfo.IsRed;
                MultiPlayUserSetting.Instance.SetUserSetting(otherName, isRed, isMy);
            }

            // 키 입력 이펙트 초기화
            foreach (var obj in keyObjects)
            {
                obj.SetActive(false);
            }
            // 키 입력 이벤트 등록
            GameManager.Input.SetNoteKeyPressEvent(OnKeyPress);
            GameManager.Input.SetNoteKeyReleaseEvent(OnKeyRelase);

            // 시작 시간 계산
            double delay = 3.0f;
            double startTime = NetworkManager.runnerInstance.SimulationTime + delay;

            // Esc 키(나가기) 활성화
            GameManager.Input.SetUIKeyEvent(LeaveMultiPlay);

            // 죽었을 때 표시 비활성화
            deathText.SetActive(false);
            // 게임 시작 코루틴
            StartCoroutine(CallStartGameRpcWhenReady((float)startTime));
        }
        #endregion

        #region Destroy Object
        private void OnDestroy()
        {
            GameManager.Input.RemoveUIKeyEvent(LeaveMultiPlay);
            GameManager.Input.RemoveNoteKeyEvent(OnKeyPress);
            GameManager.Input.RemoveNoteKeyReleaseEvent(OnKeyRelase);
        }
        #endregion

        #region App Quit
        private void OnApplicationQuit()
        {
            //프로그램 종료
            SoundManager.Instance.End();
        }
        #endregion

        #region Start Game
        private IEnumerator CallStartGameRpcWhenReady(float startTime)
        {
            // 네트워크 오브젝트가 완전히 초기화될 때까지 기다림
            while (Object == null || !Object.IsValid)
            {
                Debug.Log("[MultiPlayManager] NetworkObject 초기화 대기 중...");
                yield return null;
            }

            Debug.Log("[MultiPlayManager] StartGameRpc 호출");
            StartGameRpc(startTime);
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void StartGameRpc(float startTime)
        {
            Debug.Log($"Starting game at: {startTime}");

            StartCoroutine(StartGameAfterDelay(startTime));
        }

        private IEnumerator StartGameAfterDelay(float startTime)
        {
            while (NetworkManager.runnerInstance.SimulationTime < startTime)
            {
                yield return null; // Wait until the specified start time
            }

            // LoadManager에서 이미 로드된 곡 정보 찾기  
            SongInfo loadedSong = LoadManager.Instance.Songs.FirstOrDefault(s => s.Title == songName);
            if (loadedSong != null)
            {
                TimelineController.Instance.Initialize(loadedSong);
                Debug.Log($"[MultiPlayManager] Using pre-loaded song data for: {songName}");
            }
            else
            {
                Debug.LogError($"[MultiPlayManager] Song not found in LoadManager: {songName}");
                // fallback: 기존 방식 사용
                TimelineController.Instance.Initialize(BmsLoader.Instance.SelectSongByTitle(songName));
            }

            StartCoroutine(StartGameCoroutine(songName));
        }

        private IEnumerator StartGameCoroutine(string songTitle)
        {
            yield return new WaitForSeconds(TimelineController.Instance.BeatDelayTime);

            // Play the soung
            Play.SoundManager.Instance.SongInit(songTitle, PlayStyle.Normal);
            Play.SoundManager.Instance.Play();
            StartCoroutine(Play.SoundManager.Instance.WaitForMusicEnd(() => End()));

            // Play the video associated with the song
            videoPlay.GetVideoClip(songTitle);
            videoPlay.Play();
        }
        #endregion

        #region Play
        private void OnKeyPress(Define.NoteControl keyEvent)
        {
            float pressTime = Time.time;
            // 해당 라인에 있는 노트 중 판정할 노트 검색
            Note closestNote = FindClosestNoteToPressTime((int)keyEvent, pressTime);
            int noteId = closestNote != null ? closestNote.NoteId : -1;

            //키 입력 임팩트
            keyObjects[(int)keyEvent - (int)Define.NoteControl.Key1].SetActive(true);

            if (closestNote != null)
            {
                float timeDifference = Mathf.Abs(pressTime - closestNote.targetTime);

                if (timeDifference <= greateThreshold)
                {
                    HandleNoteHit((int)keyEvent, closestNote, AccuracyType.Great, 100, noteId);
                }
                else if ((timeDifference -= greateThreshold) <= goodThreshold)
                {
                    HandleNoteHit((int)keyEvent, closestNote, AccuracyType.Good, 70, noteId);
                }
                else if ((timeDifference -= goodThreshold) <= badThreshold)
                {
                    HandleNoteHit((int)keyEvent, closestNote, AccuracyType.Bad, 40, noteId);
                }
                else if ((timeDifference -= badThreshold) <= missThreshold)
                {
                    HandleNoteHit((int)keyEvent, closestNote, AccuracyType.Miss, 0, noteId);
                }
            }
        }

        private Note FindClosestNoteToPressTime(int channel, float pressTime)
        {
            return TimelineController.Instance.GetClosestNote(channel, pressTime);
        }

        public void HandleNoteHit(int channel, Note note, AccuracyType accuracyResult, float percent, int noteId)
        {
            float noteScore = note.Hit();  // 노트를 맞췄을 때의 행동 (노트 삭제 또는 이펙트 생성 등)
                
            MultiScoreComparison.Instance.UpdateMyScore(noteScore, percent, accuracyResult);

            switch (accuracyResult)
            {
                case AccuracyType.Miss:
                    if (myHpBar.SetHp(-10))
                        deathText.SetActive(true);
                    break;
                default:
                    myHpBar.SetHp(5);
                    keyEffectObjects[channel - 11].SetActive(true);
                    RPC_ReceiveKeyInput(channel, accuracyResult, percent, noteId);
                    break;
            }
        }

        [Rpc(RpcSources.All, RpcTargets.All)]
        public void RPC_ReceiveKeyInput(int channel, AccuracyType accuracyResult, float percent, int noteId, RpcInfo info = default)
        {
            // 이미 Destroy된 경우 방어
            if (!Object || !Object.IsValid)
                return;
            if (info.Source == NetworkManager.runnerInstance.LocalPlayer)
                return;
            if (matchController == null || !matchController.isActiveAndEnabled)
                return;

            matchController.ShowKeyEffect(channel, accuracyResult, percent, noteId);
        }

        private void OnKeyRelase(Define.NoteControl keyEvent)
        {
            int index = (int)keyEvent - (int)Define.NoteControl.Key1;

            // 리스트 크기 확인
            if (keyObjects == null || keyObjects.Length <= index)
            {
                Debug.LogError($"keyObjects 배열이 초기화되지 않았거나 인덱스 {index}가 범위를 벗어났습니다.");
                return;
            }

            // 개별 요소 null 체크
            if (keyObjects[index] == null)
            {
                Debug.Log(keyObjects[0]);
                Debug.LogError($"keyObjects[{index}]가 null입니다.");
                return;
            }

            // 오브젝트가 삭제되지 않았는지 확인 후 SetActive(false)
            if (keyObjects[index] != null && keyObjects[index].gameObject != null)
            {
                keyObjects[index].SetActive(false);
            }
            else
            {
                Debug.LogError($"keyObjects[{index}]가 삭제되었거나 비활성화된 상태입니다.");
            }
        }
        #endregion

        #region Leave Multi Play
        private void LeaveMultiPlay(Define.UIControl keyEvent)
        {
            // ESC 키 누르면 퇴장
            if (keyEvent != Define.UIControl.Esc)
                return;

            // 사운드, 비디오 종료
            SoundManager.Instance.End();
            videoPlay.End();

            // 게임 상태 변경
            GameManager.Instance.SetGameState(GameState.MultiLobby);

            // // 상대에게 나갔다는 신호 보내기
            // RPC_LeaveMultiPlay();
            
            NetworkManager.runnerInstance.Shutdown();
        }

        public void HandleOpponentLeft()
        {
            if (isOpponentLeft) return;
            isOpponentLeft = true;

            if (matchController != null)
            {
                matchController.OnPlayerLeft();
            }
        }

        // [Rpc(RpcSources.All, RpcTargets.All)]
        // private void RPC_LeaveMultiPlay(RpcInfo info = default)
        // {
        //     if (info.Source == NetworkManager.runnerInstance.LocalPlayer)
        //     {
        //         return;
        //     }

        //     // 상대에게 나갔다는 신호 보내기
        //     matchController.OnPlayerLeft();
        // }
        #endregion

        #region End
        private void End()
        {
            Debug.Log("Game Ended");

            // 게임 종료 처리
            SoundManager.Instance.End();
            videoPlay.End();
            TimelineController.Instance.RemoveJudgementLine();
            GameManager.Instance.SetGameState(GameState.MultiRoom);


            // Room으로 이동
            if (isOpponentLeft)
            {
                SceneLoading.Instance.LoadScene("MultiLobbyScene_InMusic");
                return;
            }
            else
            {
                ScoreData[] scoreDatas = MultiScoreComparison.Instance.SetScore(songName, artist);
                MultiRoomManager.Instance.scoreDatas = scoreDatas;
                NetworkManager.runnerInstance.LoadScene("MultiRoomScene_InMusic");
            }
            //Result.MultiResultManager.Instance.ReceiveResult(scoreDatas);
        }
        #endregion
    }
}
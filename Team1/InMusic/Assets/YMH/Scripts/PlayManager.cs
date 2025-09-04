using System.Collections;
using UnityEngine;
using TMPro;
using System;
using System.Linq;
using Play.Result;
using SSW.DB;
using SongList;
using Unity.VisualScripting;

namespace Play
{
    #region play result data class
    [Serializable]
    public class ScoreData
    {
        public string songKey;
        //노래 정보
        public string songName;
        public string artist;

        // 플레이어 이름
        public string userName;
        public bool isRed;

        //점수
        public int score;
        public float accuracy;
        public string rank;
        //판정별 입력 횟수
        public int great;
        public int good;
        public int bad;
        public int miss;
        //최대 콤보
        public int maxCombo;
        //풀콤보 여부
        public bool isFullCombo;
    }
    #endregion

    public class PlayManager : MonoBehaviour
    {
        public static PlayManager Instance { get; private set; }

        [Header("관련 Scripts")]
        //점수 관련
        [SerializeField]
        private ScoreManager scoreManager;
        //콤보 관련
        [SerializeField]
        private Combo comboScript;
        //비디오 관련
        [SerializeField]
        private VideoPlay videoPlayScript;
        [SerializeField]
        private HpBar myHpBar;

        [Header("점수 관련")]
        [SerializeField]
        private TextMeshProUGUI scoreText;
        [SerializeField]
        private TextMeshProUGUI accuracyText;

        [Header("Key Objects")]
        [SerializeField]
        private GameObject[] keyObjects;
        [Header("키 입력 Effect")]
        [SerializeField]
        private GameObject[] keyEffectObjects;

        //노래 정보
        private string songName;
        private string artist;
        public string SongTitle { get { return songName; } private set { } }
        private const float preStartDelay = 2.0f;
        private Coroutine musicEndCoroutine;

        // 판정 기준
        private const float greateThreshold = 0.0533f;
        private const float goodThreshold = 0.0416f;
        private const float badThreshold = 0.0832f;
        private const float missThreshold = 0.0416f;

        //데이터 불러온 상태
        public bool isDataLoaded { get; private set; } = false;

        //게임 상태
        enum States
        {
            Ready,
            Playing,
            Pause,
            GameOver,
            End
        }
        private States state = States.Ready;

        protected virtual void Awake()
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

        private void Start()
        {
            GameManager.Instance.SetGameState(GameState.GamePlay);
        }

        private void OnApplicationQuit()
        {
            //프로그램 종료
            SoundManager.Instance.End();
        }


        #region Play Init
        public void Init(string songName, string artist)
        {
            //노래 정보 저장
            this.songName = songName;
            this.artist = artist;

            //비디오 불러오기
            videoPlayScript.GetVideoClip(songName);
            //노래 불러오기
            SoundManager.Instance.SongInit(songName, PlayStyle.Normal);

            isDataLoaded = true;
        }

        public bool IsDataLoaded()
        {
            return isDataLoaded;
        }
        #endregion

        #region Play Game
        public void StartGame()
        {
            //박자선 계산
            //metronome.CalculateSync();

            // LoadManager에서 이미 로드된 곡 정보 찾기
            SongInfo loadedSong = LoadManager.Instance.Songs.FirstOrDefault(s => s.Title == songName);
            if (loadedSong != null)
            {
                TimelineController.Instance.Initialize(loadedSong);
                Debug.Log($"[PlayManager] Using pre-loaded song data for: {songName}");
            }
            else
            {
                Debug.LogError($"[PlayManager] Song not found in LoadManager: {songName}");
                // fallback: 기존 방식 사용
                TimelineController.Instance.Initialize(BmsLoader.Instance.SelectSongByTitle(songName));
            }

            //점수 초기화
            scoreManager.Init();
            comboScript.Init();
            // HP 초기화
            myHpBar.InitHp();

            //키보드 설정
            GameManager.Input.SetNoteKeyPressEvent(OnKeyPress);
            GameManager.Input.SetNoteKeyReleaseEvent(OnKeyRelase);
            GameManager.Input.SetUIKeyEvent(OnUIKkeyPress);

            //게임 시작 딜레이 측정 및 시작
            StartCoroutine(StartGameWithIntroDelay());
        }

        private IEnumerator StartGameWithIntroDelay()
        {
            yield return null;

            //metronome.StartMetronome();
            //NoteManager.Instance.InitializeNotes(BmsLoader.Instance.SelectSong(songName));

            //딜레이
            Debug.Log(TimelineController.Instance.BeatDelayTime);
            yield return new WaitForSeconds(TimelineController.Instance.BeatDelayTime);

            //게임 상태 수정
            state = States.Playing;

            //노래 재생
            SoundManager.Instance.Play();
            //노래 끝나는지 체크
            musicEndCoroutine = StartCoroutine(SoundManager.Instance.WaitForMusicEnd(() => End()));
            //비디오 재생
            videoPlayScript.Play();

        }
        #endregion

        private void OnKeyPress(Define.NoteControl keyEvent)
        {
            float pressTime = Time.time;
            // 해당 라인에 있는 노트 중 판정할 노트 검색
            Note closestNote = FindClosestNoteToPressTime((int)keyEvent, pressTime);

            //키 입력 임팩트
            keyObjects[(int)keyEvent - (int)Define.NoteControl.Key1].SetActive(true);

            if (closestNote != null)
            {
                float timeDifference = Mathf.Abs(pressTime - closestNote.targetTime);

                if (timeDifference <= greateThreshold)
                {
                    HandleNoteHit(closestNote, AccuracyType.Great, 100);
                    keyEffectObjects[(int)keyEvent - (int)Define.NoteControl.Key1].SetActive(true);
                }
                else if ((timeDifference -= greateThreshold) <= goodThreshold)
                {
                    HandleNoteHit(closestNote, AccuracyType.Good, 70);
                    keyEffectObjects[(int)keyEvent - (int)Define.NoteControl.Key1].SetActive(true);
                }
                else if ((timeDifference -= goodThreshold) <= badThreshold)
                {
                    HandleNoteHit(closestNote, AccuracyType.Bad, 40);
                    keyEffectObjects[(int)keyEvent - (int)Define.NoteControl.Key1].SetActive(true);
                }
                else if ((timeDifference -= badThreshold) <= missThreshold)
                {
                    HandleNoteHit(closestNote, AccuracyType.Miss, 0);
                    keyEffectObjects[(int)keyEvent - (int)Define.NoteControl.Key1].SetActive(true);
                }
            }
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

        private void OnUIKkeyPress(Define.UIControl keyEvent)
        {
            switch (keyEvent)
            {
                case Define.UIControl.Esc:
                    if (state == States.Pause)
                        break;

                    Pause();
                    break;
            }
        }

        public void DestroyKeyEvent()
        {
            GameManager.Input.RemoveNoteKeyEvent(OnKeyPress);
            GameManager.Input.RemoveNoteKeyEvent(OnKeyRelase);
            GameManager.Input.RemoveUIKeyEvent(OnUIKkeyPress);
        }

        private void OnDestroy()
        {
            DestroyKeyEvent();
        }

        private Note FindClosestNoteToPressTime(int channel, float pressTime)
        {
            return TimelineController.Instance.GetClosestNote(channel, pressTime);
        }

        public void HandleNoteHit(Note note, AccuracyType accuracyResult, float percent)
        {
            float noteScore = note.Hit();  // 노트를 맞췄을 때의 행동 (노트 삭제 또는 이펙트 생성 등)

            switch (accuracyResult)
            {
                case AccuracyType.Miss:
                    if (myHpBar.SetHp(-10))
                        GameOver();
                    break;
                default:
                    myHpBar.SetHp(5);
                    break;
            }

            //점수 계산 및 표시
            scoreManager.AddScore(noteScore, percent, accuracyResult);
        }

        #region Pause
        //일시정지
        public void Pause()
        {
            //상태 변환
            state = States.Pause;

            //일시정지
            PauseManager.Instance.Pause();
            videoPlayScript.Pause();
        }

        /// <summary>
        /// 계속하기
        /// </summary>
        public void Continue()
        {
            //상태 변환
            state = States.Playing;

            //계속하기
            Time.timeScale = 1;
            //노래 실행
            SoundManager.Instance.SetPause(false);
            //비디오 실행
            videoPlayScript.Play();
        }

        /// <summary>
        /// 재시작
        /// </summary>
        public void ReStart()
        {
            Time.timeScale = 1;

            //fade in-out

            //영상 초기화
            videoPlayScript.End();
            //노래 초기화
            StopCoroutine(musicEndCoroutine);
            SoundManager.Instance.End();
            // 노트 및 박자선 삭제
            TimelineController.Instance.StopAllSpawnCoroutines();
            TimelineController.Instance.ClearAll();

            //재시작
            Init(songName, artist);
            StartGame();
        }
        #endregion

        #region GameOver
        private void GameOver()
        {
            state = States.GameOver;
            GameOverManager.Instance.GameOver();
            TimelineController.Instance.StopAllSpawnCoroutines();
            TimelineController.Instance.ClearAll();
        }
        #endregion

        #region End
        //노래 종료
        public void End()
        {
            //상태 변환
            state = States.End;

            //노래 종료
            SoundManager.Instance.End();
            //비디오 종료
            videoPlayScript.End();
            //판정선 안보에기 표시
            //NoteManager.Instance.RemoveJudgementLine();
            TimelineController.Instance.RemoveJudgementLine();

            //등급 판정
            scoreManager.SetRank();
            //결과 점수 저장
            ScoreData scoreData = SaveScore();

            //결과창 띄우기
            ResultManager.Instance.ReceiveResult(scoreData);

            //결과 DB로 보내기
            string userId = Steamworks.SteamUser.GetSteamID().m_SteamID.ToString();
            DBService.Instance.SaveMusicLog(userId, scoreData);
        }

        private ScoreData SaveScore()
        {
            ScoreData scoreData = new ScoreData();

            scoreData.songName = songName.ToString();
            scoreData.artist = artist;
            scoreData.songKey = songName.ToString() + "_" + artist;
            scoreData.score = (int)scoreManager.Score;
            scoreData.accuracy = scoreManager.Accuracy;
            scoreData.rank = scoreManager.Rank;
            scoreData.great = scoreManager.InputCount[0];
            scoreData.good = scoreManager.InputCount[1];
            scoreData.bad = scoreManager.InputCount[2];
            scoreData.miss = scoreManager.InputCount[3];
            scoreData.maxCombo = comboScript.MaxCombo;
            scoreData.isFullCombo = comboScript.IsFullCombo;

            return scoreData;
        }
        #endregion
    }
}
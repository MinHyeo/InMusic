using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using Play.Result;
using UnityEngine.InputSystem;

namespace Play
{
    #region play result data class
    public class ScoreData
    {
        //노래 정보
        public string songName;
        public string artist;
        //점수
        public int score;
        public float accuracy;
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

    public class PlayManager : SingleTon<PlayManager>
    {
        [Header("관련 Scripts")]
        //정확도 관련
        [SerializeField]
        private Accuracy accuracyScript;
        //콤보 관련
        [SerializeField]
        private Combo comboScript;
        //비디오 관련
        [SerializeField]
        private VideoPlay videoPlayScript;
        //박자선 관련
        [SerializeField]
        private Metronome metronome;

        [Header("점수 관련")]
        [SerializeField]
        private TextMeshProUGUI scoreText;
        [SerializeField]
        private TextMeshProUGUI accuracyText;

        private SongInfo songInfo;

        [Header("Key Objects")]
        [SerializeField]
        private GameObject[] keyObjects;

        //노래 정보
        private Song songName;
        public Song SongTitle { get { return songName; } private set { } }
        private const float preStartDelay = 2.0f;
        private const float noteSpeed = 5.0f;

        // 판정 기준
        private const float greateThreshold = 0.0533f;
        private const float goodThreshold = 0.0416f;
        private const float badThreshold = 0.0832f;
        private const float missThreshold = 0.0416f;

        //점수 관련
        private float score = 0;            //점수
        private float accuracy = 0;         //정확도
        private float totalPercent = 0;     //정확도 총합
        private int noteCount = 0;          //노트 입력 횟수

        //입력 횟수
        private int[] inputCount = new int[4] { 0, 0, 0, 0 };

        //게임 상태
        enum States
        {
            Ready,
            Playing,
            Pause,
            End
        }
        private States state = States.Ready;

        public void OnClickButton()
        {
            StartGame(Song.Heya);
        }

        private void OnApplicationQuit()
        {
            //프로그램 종료
            SoundManager.Instance.End();
        }

        public void StartGame(Song songName)
        {
            this.songName = songName;
            SoundManager.Instance.SongInit(songName.ToString());
            videoPlayScript.GetVideoClip(songName);
            metronome.CalculateSync();

            //점수 관련 초기화
            score = 0;
            comboScript.Init();

            //텍스트 초기화
            scoreText.text = "0";
            accuracyText.text = "0.00%";

            StartCoroutine(StartMusicWithIntroDelay());

            //키 설정
            GameManager.Input.SetNoteKeyPressEvent(OnKeyPress);
            GameManager.Input.SetNoteKeyReleaseEvent(OnKeyRelase);
            GameManager.Input.SetUIKeyEvent(OnUIKkeyPress);
        }

        private IEnumerator StartMusicWithIntroDelay()
        {
            metronome.StartInitialMetronome();
            NoteManager.Instance.InitializeNotes(BmsLoader.Instance.SelectSong(songName));
            // 2초 인트로 시간 대기
            yield return new WaitForSeconds(preStartDelay);

            //상태 변환
            state = States.Playing;

            //노래 재생 및 마디선 생성
            PlaySong();
            metronome.StartMetronome();
        }

        private void PlaySong()
        {
            SoundManager.Instance.Play();
            videoPlayScript.Play();
        }

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
                }
                else if ((timeDifference -= greateThreshold) <= goodThreshold)
                {
                    HandleNoteHit(closestNote, AccuracyType.Good, 70);
                }
                else if ((timeDifference -= goodThreshold) <= badThreshold)
                {
                    HandleNoteHit(closestNote, AccuracyType.Bad, 40);
                }
                else if ((timeDifference -= badThreshold) <= missThreshold)
                {
                    HandleNoteHit(closestNote, AccuracyType.Miss, 0);
                }
            }
        }

        private void OnKeyRelase(Define.NoteControl keyEvent)
        {
            Debug.Log("키 입력 끝");
            keyObjects[(int)keyEvent - (int)Define.NoteControl.Key1].SetActive(false);
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

        private Note FindClosestNoteToPressTime(int channel, float pressTime)
        {
            Note closestNote = null;
            float minTimeDifference = float.MaxValue;

            foreach (Note note in NoteManager.Instance.NoteList)
            {
                if (note.Channel == channel)
                {
                    float timeDifference = Mathf.Abs(note.targetTime - pressTime);
                    if (timeDifference < minTimeDifference)
                    {
                        minTimeDifference = timeDifference;
                        closestNote = note;
                    }
                }
            }

            return closestNote;
        }

        public void HandleNoteHit(Note note, AccuracyType accuracyResult, float percent)
        {
            float noteScore = note.Hit();  // 노트를 맞췄을 때의 행동 (노트 삭제 또는 이펙트 생성 등)

            //점수 계산
            score += noteScore * (percent / 100);
            int scoreInt = (int)score;
            noteCount++;
            totalPercent += percent;
            accuracy = totalPercent / (float)noteCount;

            //정확도 표시
            accuracyScript.ShowAccracy(accuracyResult);
            //콤보
            comboScript.ChangeInCombo(accuracyResult);

            //입력 횟수 증가
            inputCount[(int)accuracyResult] += 1;

            //점수 표시
            scoreText.text = scoreInt.ToString();
            accuracyText.text = accuracy.ToString("F2") + "%";
        }

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
            SoundManager.Instance.Pause(false);
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
            SoundManager.Instance.End();
            //노트 초기화
            NoteManager.Instance.Restart();


            //재시작
            StartGame(songName);
        }

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
            NoteManager.Instance.RemoveJudgementLine();

            //결과 점수 저장
            ScoreData scoreData = SaveScore();
            //결과창 띄우기
            ResultManager.Instance.ReceiveResult(scoreData);
        }

        private ScoreData SaveScore()
        {
            ScoreData scoreData = new ScoreData();

            scoreData.songName = songName.ToString();
            scoreData.artist = "artist";
            scoreData.score = (int)score;
            scoreData.accuracy = accuracy;
            scoreData.great = inputCount[0];
            scoreData.good = inputCount[1];
            scoreData.bad = inputCount[2];
            scoreData.miss = inputCount[3];
            scoreData.maxCombo = comboScript.MaxCombo;
            scoreData.isFullCombo = comboScript.IsFullCombo;

            return scoreData;
        }
    }
}
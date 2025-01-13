using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

namespace Play
{
    public class PlayManager : MonoBehaviour
    {
        public static PlayManager Instance;
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Debug.LogError("Scene에 여러개의 PlayManager 존재");
                Destroy(this.gameObject);
                return;
            }
            Instance = this;
        }

        //정확도 관련
        [SerializeField]
        private Accuracy accuracyScript;
        //콤보 관련
        [SerializeField]
        private Combo comboScript;
        //비디오 관련
        [SerializeField]
        private VideoPlay videoPlayScript;

        [SerializeField]
        private Metronome metronome;
        public TextMeshProUGUI text;

        //노래 정보
        private Song songName;
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
            StartGame();
        }

        private void OnApplicationQuit()
        {
            //프로그램 종료
            SoundManager.Instance.End();
        }

        private void StartGame()
        {
            songName = Song.Heya;
            SoundManager.Instance.SongInit(songName.ToString());
            videoPlayScript.GetVideoClip(songName);
            metronome.CalculateSync();

            StartCoroutine(StartMusicWithIntroDelay());
            //Task.Run(async () => await StartMusicWithIntroDelay());
        }

        //private async Task StartMusicWithIntroDelay()
        //{
        //    metronome.StartInitialMetronome();
        //    NoteManager.Instance.InitializeNotes(BmsLoader.Instance.SelectSong(songName));

        //    await Task.Delay((int)(preStartDelay * 1000));
        //    state = States.Playing;

        //    PlaySong();
        //    metronome.StartMetronome();
        //}

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

        public void OnKeyPress(int channel, float pressTime)
        {
            // 해당 라인에 있는 노트 중 판정할 노트 검색
            Note closestNote = FindClosestNoteToPressTime(channel, pressTime);

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

        private Note FindClosestNoteToPressTime(int channel, float pressTime)
        {
            Note closestNote = null;
            float minTimeDifference = float.MaxValue;

            foreach (Note note in NoteManager.Instance.NoteList)
            {
                if (note.channel == channel)
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

            //테스트용
            text.text = "Score : " + scoreInt.ToString() + "\n";
            text.text += accuracy.ToString("F2") + "%";
        }

        //일시정지
        public void OnPause()
        {
            //상태 변환
            state = States.Pause;

            //일시정지
            PauseManager.Instance.Pause();
            videoPlayScript.Pause();
        }

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

        public void ReStart()
        {
            Time.timeScale = 1;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
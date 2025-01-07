using System.Collections;
using UnityEngine;
using FMODUnity;
using TMPro;
using static Accuracy;

public class PlayManager : MonoBehaviour
{
    public static PlayManager Instance;
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Scene에 여러개의 PlayManager 존재");
        }
        Instance = this;
    }

    //정확도 관련
    [SerializeField]
    private Accuracy accuracyScript;

    [SerializeField]
    private Metronome metronome;
    public TextMeshProUGUI text;

    private Song songName;
    private float preStartDelay = 2.0f;
    private float noteSpeed = 5.0f;

    // 판정 기준
    private float greateThreshold = 0.0533f;
    private float goodThreshold = 0.0416f;
    private float badThreshold = 0.0832f;
    private float missThreshold = 0.0416f;

    //점수 관련
    private float score = 0;            //점수
    private float accuracy = 0;         //정확도
    private float totalPercent = 0;     //정확도 총합
    private int noteCount = 0;          //노트 입력 횟수
    private int combo = 0;              //콤보
    private int maxCombo = 0;           //최대 콤보

    private int[] inputCount = new int[4] { 0, 0, 0, 0 };

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
        metronome.CalculateSync();

        StartCoroutine(StartMusicWithIntroDelay());
    }
    private IEnumerator StartMusicWithIntroDelay()
    {
        metronome.StartInitialMetronome();
        NoteManager.Instance.InitializeNotes(BmsLoader.Instance.SelectSong(songName));
        // 2초 인트로 시간 대기
        yield return new WaitForSeconds(preStartDelay);

        //노래 재생 및 마디선 생성
        PlaySong();
        metronome.StartMetronome();
    }

    private void PlaySong()
    {
        SoundManager.Instance.Play();
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
            else if((timeDifference -= goodThreshold) <= badThreshold)
            {
                HandleNoteHit(closestNote, AccuracyType.Bad, 40);
            }
            else if((timeDifference -= badThreshold) <= missThreshold)
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
        if (accuracyResult != AccuracyType.Miss)
        {
            combo += 1;
            if (combo > maxCombo)
                maxCombo = combo;
        }
        else
        {
            combo = 0;
        }

        //입력 횟수 증가
        inputCount[(int)accuracyResult] += 1;

        //테스트용
        text.text = "Score : " + scoreInt.ToString() +"\n";
        text.text += accuracy.ToString("F2") + "%";
    }
}
using System.Collections;
using System.Security.Cryptography;
using UnityEngine;
using FMODUnity;
using TMPro;

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

    [SerializeField]
    private Metronome metronome;
    public TextMeshProUGUI text;

    private Song songName;
    private float preStartDelay = 2.0f;
    private float noteSpeed = 5.0f;

    private float greateThreshold = 0.0533f;
    private float goodThreshold = 0.0416f;
    private float badThreshold = 0.0832f;
    private float missThreshold = 0.0416f;

    private float score = 0;
    private float accuracy = 100;

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
        NoteManager.Instance.InitializeNotes(BmsLoader.Instance.songInfo);
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
                HandleNoteHit(closestNote, "Perfect" , 100);
            }
            else if ((timeDifference -= greateThreshold) <= goodThreshold)
            {
                HandleNoteHit(closestNote, "Good", 70);
            }
            else if((timeDifference -= goodThreshold) <= badThreshold)
            {
                HandleNoteHit(closestNote, "Bad", 40);
            }
            else if((timeDifference -= badThreshold) <= missThreshold)
            {
                HandleNoteHit(closestNote, "Miss", 0);
            }
        }
        else
        {
            HandleNoteMiss(channel);
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

    private void HandleNoteHit(Note note, string result, float percent)
    {
        Debug.Log($"Hit! {result}");
        //text.text = result;
        float noteScore = note.Hit();  // 노트를 맞췄을 때의 행동 (노트 삭제 또는 이펙트 생성 등)

        score += noteScore * (percent / 100);
        int scoreInt = (int)score;
        accuracy = (accuracy + percent) / 2;

        text.text = "Score : " + scoreInt.ToString() +"\n";
        text.text += accuracy.ToString("F2") + "%";
    }   

    private void HandleNoteMiss(int line)
    {
        Debug.Log($"Miss on line {line}!");
    }
}
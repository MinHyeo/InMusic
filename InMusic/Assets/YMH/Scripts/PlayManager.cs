using System.Collections;
using System.Security.Cryptography;
using UnityEngine;
using FMODUnity;

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

    private Song songName;
    private float preStartDelay = 2.0f;
    private float noteSpeed = 5.0f;

    private void Start()
    {
    }

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
}
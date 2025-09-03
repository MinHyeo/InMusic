using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 뮤비 없을 때, 음악을 재생하는 객체
/// </summary>
public class MusicSamplePlayer : MonoBehaviour
{
    [Header("뮤비 대신 보여줄 앨범 사진")]
    [SerializeField]private Image backgroundAlbum;
    [Header("음악 재생 관련 변수")]
    [SerializeField] private AudioSource muPlayer;
    private Coroutine loopCoroutine = null;
    [Tooltip("하이라이트 시작 시간(초)")]
    public float loopStartTime = 50.0f;
    [Tooltip("하이라이트 종료 시간(초)")]
    public float loopEndTime = 110.0f;

    private void Awake()
    {
        muPlayer = gameObject.GetComponent<AudioSource>();
    }

    public void PlayMusic(Image newAlbum, AudioClip audio)
    {
        muPlayer.clip = audio;
        muPlayer.time = loopStartTime;
        loopCoroutine = StartCoroutine(LoopSound());

        backgroundAlbum.sprite = newAlbum.sprite;
    }

    public void StopMusic()
    {
        if (muPlayer.isPlaying)
        {
            muPlayer.Stop();
            StopCoroutine(loopCoroutine);
            loopCoroutine = null;
        }
    }

    //특정 구간 반복 재생
    IEnumerator LoopSound()
    {
        muPlayer.Play();
        while (true)
        {
            if (muPlayer.time >= loopEndTime)
            {
                muPlayer.Stop();
                yield return null;
                muPlayer.time = loopStartTime;
                muPlayer.Play();
            }
            yield return null;
        }
    }
}

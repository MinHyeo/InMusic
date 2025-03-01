using System.Collections;
using UnityEngine;
using UnityEngine.Video;

/// <summary>
/// 뮤비를 실행하는 객체
/// </summary>
public class MusicVideoPlayer : MonoBehaviour
{
    [Header("뮤비 재생 관련 변수")]
    [Tooltip("VideoPlayer")]
    [SerializeField] private VideoPlayer muviPlayer;
    private Coroutine loopCoroutine = null;
    [Tooltip("하이라이트 시작 시간(초)")]
    public float loopStartTime = 50.0f; 
    [Tooltip("하이라이트 종료 시간(초)")]
    public float loopEndTime = 110.0f;

    /// <summary>
    /// 음악 재생, 뮤비가 없으면 종료됨
    /// </summary>
    public void PlayMusicVideo(VideoClip Muvi) {
        //VideoClip 설정
        muviPlayer.clip = Muvi;
        //시작 시간 설정
        muviPlayer.time = loopStartTime;
        //재생
        if (loopCoroutine == null)
        {
            loopCoroutine = StartCoroutine(LoopVideo());
        }
    }

    public void StopMusicVideo()
    {
        if (muviPlayer.isPlaying)
        {
            muviPlayer.Stop();
            StopCoroutine(LoopVideo());
            loopCoroutine = null;
        }
    }

    //특정 구간 반복 재생
    IEnumerator LoopVideo()
    {
        muviPlayer.Play();
        while (true)
        {
            if (muviPlayer.time >= loopEndTime)
            {
                muviPlayer.Stop();
                muviPlayer.time = loopStartTime;
                muviPlayer.Play();
            }
            yield return null;
        }
    }
}

using UnityEngine;
using UnityEngine.Video;

public class MusicVideoPlayer : MonoBehaviour
{
    [Header("뮤비 재생 관련 변수")]
    [Tooltip("VideoPlayer")]
    [SerializeField] private VideoPlayer muviPlayer;
    [Tooltip("하이라이트 시작 시간(초)")]
    public float loopStartTime = 50.0f; 
    [Tooltip("하이라이트 종료 시간(초)")]
    public float loopEndTime = 120.0f;

    /// <summary>
    /// 음악 재생, 뮤비가 없으면 종료됨
    /// </summary>
    public void PlayMusicVideo(VideoClip Muvi= null) {
        //VideoClip 설정
        muviPlayer.clip = Muvi;
        //음악 반복 Play
        if (Muvi!= null) {
            muviPlayer.time = loopStartTime;
        }
    }
}

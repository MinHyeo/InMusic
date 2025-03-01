using UnityEngine;
using UnityEngine.Video;

public class MusicVideoPlayer : MonoBehaviour
{
    [Header("�º� ��� ���� ����")]
    [Tooltip("VideoPlayer")]
    [SerializeField] private VideoPlayer muviPlayer;
    [Tooltip("���̶���Ʈ ���� �ð�(��)")]
    public float loopStartTime = 50.0f; 
    [Tooltip("���̶���Ʈ ���� �ð�(��)")]
    public float loopEndTime = 120.0f;

    /// <summary>
    /// ���� ���, �º� ������ �����
    /// </summary>
    public void PlayMusicVideo(VideoClip Muvi= null) {
        //VideoClip ����
        muviPlayer.clip = Muvi;
        //���� �ݺ� Play
        if (Muvi!= null) {
            muviPlayer.time = loopStartTime;
        }
    }
}

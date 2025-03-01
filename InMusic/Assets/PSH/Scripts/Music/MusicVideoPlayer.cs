using System.Collections;
using UnityEngine;
using UnityEngine.Video;

/// <summary>
/// �º� �����ϴ� ��ü
/// </summary>
public class MusicVideoPlayer : MonoBehaviour
{
    [Header("�º� ��� ���� ����")]
    [Tooltip("VideoPlayer")]
    [SerializeField] private VideoPlayer muviPlayer;
    private Coroutine loopCoroutine = null;
    [Tooltip("���̶���Ʈ ���� �ð�(��)")]
    public float loopStartTime = 50.0f; 
    [Tooltip("���̶���Ʈ ���� �ð�(��)")]
    public float loopEndTime = 110.0f;

    /// <summary>
    /// ���� ���, �º� ������ �����
    /// </summary>
    public void PlayMusicVideo(VideoClip Muvi) {
        //VideoClip ����
        muviPlayer.clip = Muvi;
        //���� �ð� ����
        muviPlayer.time = loopStartTime;
        //���
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

    //Ư�� ���� �ݺ� ���
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

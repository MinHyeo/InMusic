using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Video;

public class MusicVideoPlayer : MonoBehaviour
{
    [Header("�º� ��� ���� ����")]
    [Tooltip("VideoPlayer")]
    [SerializeField] private VideoPlayer muviPlayer;
    [SerializeField] private Coroutine loopCoroutine = null;
    [Tooltip("���̶���Ʈ ���� �ð�(��)")]
    public float loopStartTime = 50.0f; 
    [Tooltip("���̶���Ʈ ���� �ð�(��)")]
    public float loopEndTime = 110.0f;

    /// <summary>
    /// ���� ���, �º� ������ �����
    /// </summary>
    public void PlayMusicVideo(VideoClip Muvi = null) {
        if (Muvi == null)
        {
            if (muviPlayer.isPlaying) {
                muviPlayer.Stop();
                StopCoroutine(LoopVideo());
                loopCoroutine = null;
            }
            return;
        }
        //VideoClip ����
        muviPlayer.clip = Muvi;
        //���� �ð� ����
        muviPlayer.time = loopStartTime;
        //���
        if (loopCoroutine == null)
        {
            muviPlayer.Play();
            loopCoroutine = StartCoroutine(LoopVideo());
        }
    }

    //Ư�� ���� �ݺ� ���
    IEnumerator LoopVideo()
    {
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

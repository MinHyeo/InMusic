using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �º� ���� ��, ������ ����ϴ� ��ü
/// </summary>
public class MusicSamplePlayer : MonoBehaviour
{
    [Header("�º� ��� ������ �ٹ� ����")]
    [SerializeField]private Image backgroundAlbum;
    [Header("���� ��� ���� ����")]
    [SerializeField] private AudioSource muPlayer;
    [SerializeField] private Coroutine loopCoroutine = null;
    [Tooltip("���̶���Ʈ ���� �ð�(��)")]
    public float loopStartTime = 50.0f;
    [Tooltip("���̶���Ʈ ���� �ð�(��)")]
    public float loopEndTime = 110.0f;


    public void SetAlbum(Image newAlbum)
    {
        backgroundAlbum.sprite = newAlbum.sprite;
    }

    public void PlayMusic(AudioClip audio)
    {
        if (muPlayer.isPlaying){

        }
        muPlayer.clip = audio;
        muPlayer.time = loopStartTime;
        muPlayer.Play();
        loopCoroutine = StartCoroutine(LoopSound());
    }

    public void StopMusic()
    {
        if (muPlayer.isPlaying)
        {
            muPlayer.Stop();
            StopCoroutine(LoopSound());
            loopCoroutine = null;
        }
    }

    //Ư�� ���� �ݺ� ���
    IEnumerator LoopSound()
    {
        while (true)
        {
            if (muPlayer.time >= loopEndTime)
            {
                muPlayer.Stop();
                muPlayer.time = loopStartTime;
                muPlayer.Play();
            }
            yield return null;
        }
    }
}

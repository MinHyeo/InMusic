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
    private Coroutine loopCoroutine = null;
    [Tooltip("���̶���Ʈ ���� �ð�(��)")]
    public float loopStartTime = 50.0f;
    [Tooltip("���̶���Ʈ ���� �ð�(��)")]
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

    //Ư�� ���� �ݺ� ���
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

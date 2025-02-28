using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class MusicInfo : MonoBehaviour
{
    [Header("�º� �����ֱ�")]
    [Tooltip("���")]
    [SerializeField] private CanvasGroup background;
    [Tooltip("�ٹ� ��¿�(�º� ���� �� ��� ������)")]
    [SerializeField] private GameObject muviAlbum;
    [Tooltip("�º� ��¿�")]
    [SerializeField] private VideoPlayer muviPlayer;

    [Header("������ ������ ����")]
    [Tooltip("�ٹ�, ����, ��Ƽ��Ʈ, ����")]
    [SerializeField] private GameObject[] curMusicData = new GameObject[4];

    [Header("������ ������ �÷��� ���")]
    [Tooltip("����, ��Ȯ��, �޺�, ��ũ")]
    [SerializeField] private Text[] logData = new Text[4];

    [Tooltip("����Ʈ �����̴� �ð����� �۰� �����ؼ� fade ���� �߻� ����")]
    private float fadeDuration = 0.28f;
    private float showAlpha = 1.0f;
    private float hideAlpha = 0.5f;
    private float clearAlpha = 0.0f;

    /// <summary>
    /// ������ �׸�(MusicItem)�� �Ѱ��ָ� �ش� �����Ϳ� �°� ������
    /// </summary>
    public void UpdateInfo(MusicItem newItem)
    {
        UpdateInfomation(newItem);
        UpdateLog(newItem);
        UpdateMusicVideo(newItem);  
    }

    //���� ���� ������Ʈ
    void UpdateInfomation(MusicItem newItem) 
    {
        curMusicData[0].GetComponent<Image>().sprite = newItem.Album.sprite;
        curMusicData[1].GetComponent<Text>().text = newItem.Title.text;
        curMusicData[2].GetComponent<Text>().text = newItem.Artist.text;
        curMusicData[3].GetComponent<Text>().text = newItem.Length;
    }

    //��� ���� ������Ʈ
    void UpdateLog(MusicItem newItem)
    {
        logData[0].text = newItem.Score;
        logData[1].text = newItem.Accuracy;
        logData[2].text = newItem.Combo;
        logData[3].text = newItem.Rank.text;
    }


    //�º� ���� ������Ʈ
    void UpdateMusicVideo(MusicItem newItem)
    {
        //���̸� ���ȭ�� �����ֱ�
        if (newItem.IsDummy) {
            FadeController(1);
            muviPlayer.gameObject.SetActive(false);
            muviAlbum.gameObject.SetActive(false);
            return;
        }
        //StartCoroutine(Fade(hi));

        //�º� ������ �º� �����ֱ�
        if (newItem.HasMV)
        {
            FadeController(2);
            muviPlayer.clip = newItem.MuVi;
            //TODO:���̶���Ʈ�� ����ϱ�

            muviPlayer.gameObject.SetActive(true);
            muviAlbum.gameObject.SetActive(false);
        }
        //�º� ������ �ٹ����� ����
        else
        {
            FadeController(2);
            muviPlayer.gameObject.SetActive(false);
            muviAlbum.GetComponent<Image>().sprite = newItem.Album.sprite;
            muviAlbum.gameObject.SetActive(true);
            //TODO: ���� ���� ���̶���Ʈ�� ����ϱ�
        }
    }

    #region MusicVideoFadeControl

    /// <summary>
    /// ��Ȳ�� �°� Fade in/out ����/����ȭ�ϴ� �ż��� 
    /// </summary>
    /// <param name="situation">1: ���� 2: �Ϲ�</param>
    void FadeController(int situation) {
        switch (situation)
        {
            case 1:
                //�Ϲ� -> ����
                if (background.alpha != 0.0f)
                {
                    StartCoroutine(Fade(showAlpha, clearAlpha, fadeDuration));
                }
                break;
            case 2:
                //���� -> �Ϲ�
                if (background.alpha == 0.0f)
                {
                    StartCoroutine(Fade(clearAlpha, showAlpha, fadeDuration));
                }
                //�Ϲ� -> �Ϲ� (����ȭ)
                else
                {
                    StartCoroutine(Fade(showAlpha, hideAlpha, fadeDuration, () =>
                    {
                        StartCoroutine(Fade(hideAlpha, showAlpha, fadeDuration));
                    }));
                }
                break;
        }
    }

    /// <summary>
    /// �ڷ�ƾ�� �񵿱�� �۵��ϴ� ������ ��
    /// </summary>
    IEnumerator Fade(float startAlpha, float endAlpha, float duration, Action callback = null)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
            background.alpha = alpha;
            yield return null;
        }
        //Correction(����)
        background.alpha = endAlpha;
        callback?.Invoke(); // �ݹ� �Լ� ȣ��
    }

    #endregion

}

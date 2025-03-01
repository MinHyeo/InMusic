using UnityEngine;
using UnityEngine.UI;

public class MusicInfo : MonoBehaviour
{
    [Header("�������� ����")]
    [Tooltip("���")]
    [SerializeField] private MusicVideoController fadeController;
    [Tooltip("�ٹ� ����)")]
    [SerializeField] private GameObject muviAlbum;
    [Tooltip("�º� ����")]
    [SerializeField] private MusicVideoPlayer muviController;

    [Header("������ ������ ����")]
    [Tooltip("�ٹ�, ����, ��Ƽ��Ʈ, ����")]
    [SerializeField] private GameObject[] curMusicData = new GameObject[4];

    [Header("������ ������ �÷��� ���")]
    [Tooltip("����, ��Ȯ��, �޺�, ��ũ")]
    [SerializeField] private Text[] logData = new Text[4];

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
            fadeController.ControlFade(1);
            muviController.gameObject.SetActive(false);
            muviAlbum.gameObject.SetActive(false);
            return;
        }

        //�º� ������ �º� �����ֱ�
        if (newItem.HasMV)
        {
            muviController.gameObject.SetActive(true);
            fadeController.ControlFade(2, newItem.HasMV);

            muviController.PlayMusicVideo(newItem.MuVi);
            muviAlbum.gameObject.SetActive(false);
        }
        //�º� ������ �ٹ����� ����
        else
        {
            fadeController.ControlFade(2);
            muviController.PlayMusicVideo();
            muviController.gameObject.SetActive(false);
            muviAlbum.GetComponent<Image>().sprite = newItem.Album.sprite;
            muviAlbum.gameObject.SetActive(true);
        }
    }
}

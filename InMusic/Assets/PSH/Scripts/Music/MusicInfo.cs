using UnityEngine;
using UnityEngine.UI;

public class MusicInfo : MonoBehaviour
{
    [Header("�������� ����")]
    [Tooltip("���")]
    [SerializeField] private MusicVideoController muviController;
    [Tooltip("�ٹ� ����)")]
    [SerializeField] private GameObject muviAlbum;
    [Tooltip("�º� ����")]
    [SerializeField] private MusicVideoPlayer muviPlayer;

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
            muviController.ControlFade(1);
            muviPlayer.PlayMusicVideo();
            return;
        }

        //�º� ������ �º� �����ֱ�
        if (newItem.HasMV)
        {
            muviController.ControlFade(2, newItem.HasMV);
            muviPlayer.PlayMusicVideo(newItem.MuVi);
        }
        //�º� ������ �ٹ����� ����
        else
        {
            muviController.ControlFade(2);
            muviPlayer.PlayMusicVideo();
            muviAlbum.GetComponent<Image>().sprite = newItem.Album.sprite;
        }
    }
}

using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class MusicInfo : MonoBehaviour
{
    [Header("�º� �����ֱ�")]
    [Tooltip("���")]
    [SerializeField] private GameObject background;
    [Tooltip("�ٹ� ��¿�")]
    [SerializeField] private GameObject muAlbum;
    [Tooltip("�º� ��¿�")]
    [SerializeField] private VideoPlayer muPlayer;

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
            background.SetActive(false);
            return;
        }
        background.SetActive(true);

        //�º� ������ �º� �����ֱ�
        if (newItem.HasMV)
        {
            muPlayer.clip = newItem.MuVi;
            muPlayer.gameObject.SetActive(true);
            muAlbum.gameObject.SetActive(false);
        }
        //�º� ������ �ٹ����� ����
        else
        {
            muPlayer.gameObject.SetActive(false);
            muAlbum.GetComponent<Image>().sprite = newItem.Album.sprite;
            muAlbum.gameObject.SetActive(true);
        }
    }

}

using UnityEngine;
using UnityEngine.UI;

public class MusicInfo : MonoBehaviour
{
    [Header("뮤직비디오 제어")]
    [Tooltip("배경")]
    [SerializeField] private MusicVideoController fadeController;
    [Tooltip("앨범 제어)")]
    [SerializeField] private GameObject muviAlbum;
    [Tooltip("뮤비 제어")]
    [SerializeField] private MusicVideoPlayer muviController;

    [Header("선택한 음악의 정보")]
    [Tooltip("앨범, 제목, 아티스트, 길이")]
    [SerializeField] private GameObject[] curMusicData = new GameObject[4];

    [Header("선택한 음악의 플레이 기록")]
    [Tooltip("점수, 정확도, 콤보, 랭크")]
    [SerializeField] private Text[] logData = new Text[4];

    /// <summary>
    /// 선택한 항목(MusicItem)를 넘겨주면 해당 데이터에 맞게 보여줌
    /// </summary>
    public void UpdateInfo(MusicItem newItem)
    {
        UpdateInfomation(newItem);
        UpdateLog(newItem);
        UpdateMusicVideo(newItem);  
    }

    //음악 정보 업데이트
    void UpdateInfomation(MusicItem newItem) 
    {
        curMusicData[0].GetComponent<Image>().sprite = newItem.Album.sprite;
        curMusicData[1].GetComponent<Text>().text = newItem.Title.text;
        curMusicData[2].GetComponent<Text>().text = newItem.Artist.text;
        curMusicData[3].GetComponent<Text>().text = newItem.Length;
    }

    //기록 정보 업데이트
    void UpdateLog(MusicItem newItem)
    {
        logData[0].text = newItem.Score;
        logData[1].text = newItem.Accuracy;
        logData[2].text = newItem.Combo;
        logData[3].text = newItem.Rank.text;
    }


    //뮤비 정보 업데이트
    void UpdateMusicVideo(MusicItem newItem)
    {
        //더미면 배경화면 보여주기
        if (newItem.IsDummy) {
            fadeController.ControlFade(1);
            muviController.gameObject.SetActive(false);
            muviAlbum.gameObject.SetActive(false);
            return;
        }

        //뮤비가 있으면 뮤비 보여주기
        if (newItem.HasMV)
        {
            muviController.gameObject.SetActive(true);
            fadeController.ControlFade(2, newItem.HasMV);

            muviController.PlayMusicVideo(newItem.MuVi);
            muviAlbum.gameObject.SetActive(false);
        }
        //뮤비가 없으면 앨범사진 띄우기
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

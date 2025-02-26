using UnityEngine;
using UnityEngine.UI;

public class MusicInfo : MonoBehaviour
{
    [Header("선택한 음악의 정보")]
    [Tooltip("앨범, 제목, 아티스트, 길이")]
    [SerializeField] private GameObject[] curMusicData = new GameObject[4];
    [Header("선택한 음악의 플레이 기록")]
    [Tooltip("점수, 정확도, 콤보, 랭크")]
    [SerializeField] private Text[] logData = new Text[4];

    public void UpdateInfo(Music_Item newItem)
    {
        //음악 정보 업데이트
        curMusicData[0].GetComponent<Image>().sprite = newItem.Album.sprite;
        curMusicData[1].GetComponent<Text>().text = newItem.Title.text;
        curMusicData[2].GetComponent<Text>().text = newItem.Artist.text;
        curMusicData[3].GetComponent<Text>().text = newItem.Length;
        //기록 정보 업데이트
        logData[0].text = newItem.Score;
        logData[1].text = newItem.Accuracy;
        logData[2].text = newItem.Combo;
        logData[3].text = newItem.Rank.text;
    }

}

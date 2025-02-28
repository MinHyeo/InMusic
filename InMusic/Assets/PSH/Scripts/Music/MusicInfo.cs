using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class MusicInfo : MonoBehaviour
{
    [Header("뮤비 보여주기")]
    [Tooltip("배경")]
    [SerializeField] private CanvasGroup background;
    [Tooltip("앨범 출력용(뮤비 없을 때 대신 보여줌)")]
    [SerializeField] private GameObject muviAlbum;
    [Tooltip("뮤비 출력용")]
    [SerializeField] private VideoPlayer muviPlayer;

    [Header("선택한 음악의 정보")]
    [Tooltip("앨범, 제목, 아티스트, 길이")]
    [SerializeField] private GameObject[] curMusicData = new GameObject[4];

    [Header("선택한 음악의 플레이 기록")]
    [Tooltip("점수, 정확도, 콤보, 랭크")]
    [SerializeField] private Text[] logData = new Text[4];

    [Tooltip("리스트 움직이는 시간보다 작게 설정해서 fade 버그 발생 방지")]
    private float fadeDuration = 0.28f;
    private float showAlpha = 1.0f;
    private float hideAlpha = 0.5f;
    private float clearAlpha = 0.0f;

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
            FadeController(1);
            muviPlayer.gameObject.SetActive(false);
            muviAlbum.gameObject.SetActive(false);
            return;
        }
        //StartCoroutine(Fade(hi));

        //뮤비가 있으면 뮤비 보여주기
        if (newItem.HasMV)
        {
            FadeController(2);
            muviPlayer.clip = newItem.MuVi;
            //TODO:하이라이트만 재생하기

            muviPlayer.gameObject.SetActive(true);
            muviAlbum.gameObject.SetActive(false);
        }
        //뮤비가 없으면 앨범사진 띄우기
        else
        {
            FadeController(2);
            muviPlayer.gameObject.SetActive(false);
            muviAlbum.GetComponent<Image>().sprite = newItem.Album.sprite;
            muviAlbum.gameObject.SetActive(true);
            //TODO: 음원 파일 하이라이트만 재생하기
        }
    }

    #region MusicVideoFadeControl

    /// <summary>
    /// 상황에 맞게 Fade in/out 조절/동기화하는 매서드 
    /// </summary>
    /// <param name="situation">1: 더미 2: 일반</param>
    void FadeController(int situation) {
        switch (situation)
        {
            case 1:
                //일반 -> 더미
                if (background.alpha != 0.0f)
                {
                    StartCoroutine(Fade(showAlpha, clearAlpha, fadeDuration));
                }
                break;
            case 2:
                //더미 -> 일반
                if (background.alpha == 0.0f)
                {
                    StartCoroutine(Fade(clearAlpha, showAlpha, fadeDuration));
                }
                //일반 -> 일반 (동기화)
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
    /// 코루틴을 비동기로 작동하니 주의할 것
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
        //Correction(보정)
        background.alpha = endAlpha;
        callback?.Invoke(); // 콜백 함수 호출
    }

    #endregion

}

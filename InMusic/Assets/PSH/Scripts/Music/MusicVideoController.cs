using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class MusicVideoController : MonoBehaviour
{
    [Header("Fade 효과를 제어할 CanvasGruop")]
    [Tooltip("배경, 뮤직비디오, 앨범")]
    [SerializeField] CanvasGroup[] canvas;
    [Tooltip("리스트 움직이는 시간보다 작게 설정해서 fade 버그 발생 방지")]
    private float fadeDuration = 0.3f;
    private float showAlpha = 1.0f;
    private float hideAlpha = 0.1f;
    private float clearAlpha = 0.0f;
    private bool pasthasMuvi = false;


    /// <summary>
    /// 상황에 맞게 Fade in/out 조절/동기화하는 매서드 
    /// </summary>
    /// <param name="situation">1: 더미 2: 일반</param>
    public void ControlFade(int situation, bool curhasMuvi = false)
    {
        switch (situation)
        {
            case 1:
                //일반 -> 더미
                if (canvas[0].alpha != 0.0f)
                {
                    StartCoroutine(Fade(canvas[0],showAlpha, clearAlpha, fadeDuration));
                }
                break;
            case 2:
                //더미 -> 일반
                if (canvas[0].alpha == 0.0f)
                {
                    StartCoroutine(Fade(canvas[0], clearAlpha, showAlpha, fadeDuration));
                    return;
                }
                //일반 -> 일반 (동기화)
                if (pasthasMuvi) {
                    //뮤비 -> 뮤비
                    if (curhasMuvi)
                    {
                        StartCoroutine(Fade(canvas[1], showAlpha, hideAlpha, fadeDuration, () =>
                        {
                            StartCoroutine(Fade(canvas[1], hideAlpha, showAlpha, fadeDuration));
                        }));
                    }
                    else
                    //뮤비 -> 앨범
                    {
                        StartCoroutine(Fade(canvas[1], showAlpha, hideAlpha, fadeDuration, () =>
                        {
                            StartCoroutine(Fade(canvas[2], hideAlpha, showAlpha, fadeDuration));
                        }));
                    }
                }
                else
                {
                    //앨범 -> 뮤비
                    if (curhasMuvi)
                    {
                        StartCoroutine(Fade(canvas[2], showAlpha, hideAlpha, fadeDuration, () =>
                        {
                            StartCoroutine(Fade(canvas[1], hideAlpha, showAlpha, fadeDuration));
                        }));
                    }
                    //앨범 -> 앨범
                    else
                    {
                        StartCoroutine(Fade(canvas[2], showAlpha, hideAlpha, fadeDuration, () =>
                        {
                            StartCoroutine(Fade(canvas[2], hideAlpha, showAlpha, fadeDuration));
                        }));
                    }
                }
                break;
           }
        pasthasMuvi = curhasMuvi;
    }


    /// <summary>
    /// 코루틴을 비동기로 작동하니 주의할 것
    /// </summary>
    IEnumerator Fade(CanvasGroup target,float startAlpha, float endAlpha, float duration, Action callback = null)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
            target.alpha = alpha;
            yield return null;
        }
        //Correction(보정)
        target.alpha = endAlpha;
        //콜백 함수 호출
        callback?.Invoke();
    }
}

using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// �º�� �ٹ� �����ִ°� �����ϴ� ��ü
/// </summary>
public class MusicFadeController : MonoBehaviour
{
    [Header("Fade ȿ���� ������ CanvasGruop")]
    [Tooltip("���, ��������, �ٹ�")]
    [SerializeField] CanvasGroup[] canvas;
    [Header("�������� ��� ���� ����")]

    private float fadeDuration = 0.28f;
    private float showAlpha = 1.0f;
    private float hideAlpha = 0.0f;
    private bool pasthasMuvi = false;


    /// <summary>
    /// ��Ȳ�� �°� Fade in/out ����/����ȭ�ϴ� �ż��� 
    /// </summary>
    /// <param name="situation">1: ���� 2: �Ϲ�</param>
    public void ControlFade(int situation, bool curhasMuvi = false)
    {
        switch (situation)
        {
            case 1:
                //�Ϲ� -> ���� or ���� -> ����
                if (canvas[0].alpha != 0.0f)
                {
                    StartCoroutine(Fade(canvas[0],showAlpha, hideAlpha, fadeDuration));
                }
                break;
            case 2:
                //���� -> �Ϲ�
                if (canvas[0].alpha == 0.0f)
                {
                    //���� -> �º�
                    if (curhasMuvi)
                    {
                        if (canvas[1].alpha != 1.0f)
                            canvas[1].alpha = 1.0f;
                        if (canvas[2].alpha != 0.0f)
                            canvas[2].alpha = 0.0f;
                    }
                    //���� -> �ٹ�
                    else
                    {
                        if (canvas[1].alpha != 0.0f)
                            canvas[1].alpha = 0.0f;
                        if (canvas[2].alpha != 1.0f)
                            canvas[2].alpha = 1.0f;
                    }
                    StartCoroutine(Fade(canvas[0], hideAlpha, showAlpha, fadeDuration));
                    return;
                }
                //�Ϲ� -> �Ϲ� (����ȭ)
                if (pasthasMuvi) {
                    //�º� -> �º�
                    if (curhasMuvi)
                    {
                        StartCoroutine(Fade(canvas[1], showAlpha, hideAlpha, fadeDuration, () =>
                        {
                            StartCoroutine(Fade(canvas[1], hideAlpha, showAlpha, fadeDuration));
                        }));
                    }
                    else
                    //�º� -> �ٹ�
                    {
                        StartCoroutine(Fade(canvas[1], showAlpha, hideAlpha, fadeDuration, () =>
                        {
                            StartCoroutine(Fade(canvas[2], hideAlpha, showAlpha, fadeDuration));
                        }));
                    }
                }
                else
                {
                    //�ٹ� -> �º�
                    if (curhasMuvi)
                    {
                        StartCoroutine(Fade(canvas[2], showAlpha, hideAlpha, fadeDuration, () =>
                        {
                            StartCoroutine(Fade(canvas[1], hideAlpha, showAlpha, fadeDuration));
                        }));
                    }
                    //�ٹ� -> �ٹ�
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
    /// �ڷ�ƾ�� �񵿱�� �۵��ϴ� ������ ��
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
        //Correction(����)
        target.alpha = endAlpha;
        //�ݹ� �Լ� ȣ��
        callback?.Invoke();
    }
}

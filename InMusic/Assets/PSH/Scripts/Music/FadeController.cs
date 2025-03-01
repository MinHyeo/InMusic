using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class FadeController : MonoBehaviour
{
    [Header("Fade ȿ���� ������ CanvasGruop")]
    [Tooltip("���, ��������, �ٹ�")]
    [SerializeField] CanvasGroup[] canvas;
    [Tooltip("����Ʈ �����̴� �ð����� �۰� �����ؼ� fade ���� �߻� ����")]
    private float fadeDuration = 0.28f;
    private float showAlpha = 1.0f;
    private float hideAlpha = 0.5f;
    private float clearAlpha = 0.0f;


    /// <summary>
    /// ��Ȳ�� �°� Fade in/out ����/����ȭ�ϴ� �ż��� 
    /// </summary>
    /// <param name="situation">1: ���� 2: �Ϲ�</param>
    public void ControlFade(int situation, bool hasMuvi = false)
    {
        switch (situation)
        {
            case 1:
                //�Ϲ� -> ����
                if (canvas[0].alpha != 0.0f)
                {
                    StartCoroutine(Fade(canvas[0],showAlpha, clearAlpha, fadeDuration));
                }
                break;
            case 2:
                //���� -> �Ϲ�
                if (canvas[0].alpha == 0.0f)
                {
                    StartCoroutine(Fade(canvas[0], clearAlpha, showAlpha, fadeDuration));
                }
                //�Ϲ� -> �Ϲ� (����ȭ)

                break;
        }
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

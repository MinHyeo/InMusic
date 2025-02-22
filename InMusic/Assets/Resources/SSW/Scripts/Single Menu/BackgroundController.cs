using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using System.Collections;

public class BackgroundController : MonoBehaviour
{
    #region Singleton
    private static BackgroundController _instance;
    public static BackgroundController Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<BackgroundController>();
                if (_instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = typeof(BackgroundController).Name;
                    _instance = obj.AddComponent<BackgroundController>();
                }
            }
            return _instance;
        }
    }
    #endregion

    [Header("References")]
    [SerializeField] private AudioSource _bgAudio;
    [SerializeField] private VideoPlayer _bgVideo;
    [SerializeField] private CanvasGroup _bgCanvasGroup; // 페이드 인/아웃용

    private Coroutine _highlightCoroutine;

    #region Unity Methods
    private void Awake()
    {
        if(_instance == null) {
            _instance = this;
        } else if(_instance != this) {
            Destroy(gameObject);
        }
        if (_bgAudio != null) _bgAudio.volume = 0f;
        if (_bgCanvasGroup != null) _bgCanvasGroup.alpha = 0f;
    }
    #endregion

    /// <summary>
    /// 슬롯(ScrollSlot)에서 "0.2초 게이지 완료" 이벤트를 수신.
    /// 이 시점부터 대기 + 1초 페이드인 (뮤직/비디오) 진행.
    /// </summary>
    public void StartHighlightProcess(string songName)
    {
        // 기존 코루틴 중지
        if (_highlightCoroutine != null)
        {
            StopCoroutine(_highlightCoroutine);
        }
        _highlightCoroutine = StartCoroutine(HighlightRoutine(songName));
    }

    /// <summary>
    /// 대기 → 1초 페이드 인
    /// </summary>
    private IEnumerator HighlightRoutine(string songName)
    {
        yield return new WaitForSeconds(1.5f);

        // 1초 페이드 인
        yield return StartCoroutine(FadeInMedia(1f, songName));

        // 필요하다면 여기서 반복(15초 후 재생 위치 되감기 등)도 가능
    }

    /// <summary>
    /// 노래/뮤비 페이드 인(볼륨 0.5→1, alpha 0→1)
    /// </summary>
    private IEnumerator FadeInMedia(float fadeDuration, string songName)
    {
        if (_bgAudio != null)
        {
            _bgAudio.volume = 0.5f;
            AudioClip clip = Resources.Load<AudioClip>($"Song/{songName}/{songName}");
            _bgAudio.clip = clip; 
            _bgAudio.Play(); // clip이 세팅되어있다고 가정
            _bgAudio.time = 60f;
        }

        if (_bgCanvasGroup != null) _bgCanvasGroup.alpha = 0f;

        if (_bgVideo != null)
        {
            VideoClip vclip = Resources.Load<VideoClip>($"Song/{songName}/{songName}");
            _bgVideo.clip = vclip;
            _bgVideo.audioOutputMode = VideoAudioOutputMode.None;
            _bgVideo.Play();
            _bgVideo.time = 60f;
        }

        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration);

            if (_bgAudio != null)
                _bgAudio.volume = Mathf.Lerp(0.5f, 1f, t);

            if (_bgCanvasGroup != null)
                _bgCanvasGroup.alpha = Mathf.Lerp(0f, 1f, t);

            yield return null;
        }

        // 최종 보정
        if (_bgAudio != null) _bgAudio.volume = 1f;
        if (_bgCanvasGroup != null) _bgCanvasGroup.alpha = 1f;
    }

    /// <summary>
    /// 다른 곡이 하이라이트되면 이전 곡의 하이라이트 정지
    /// </summary>
    public void StopHighlight()
    {
        if (_highlightCoroutine != null)
        {
            StopCoroutine(_highlightCoroutine);
        }
        // 오디오/비디오도 정지
        // if (_bgAudio != null && _bgAudio.isPlaying) _bgAudio.Stop();
        // if (_bgVideo != null && _bgVideo.isPlaying) _bgVideo.Stop();
        // if (_bgCanvasGroup != null) _bgCanvasGroup.alpha = 0f;
    }
}
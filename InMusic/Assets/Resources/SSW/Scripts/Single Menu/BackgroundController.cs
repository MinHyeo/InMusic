using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using System.Collections;
using System.Timers;

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
    private Coroutine _fadeCoroutine;
    private Coroutine _resetCoroutine;

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

    void Update()
    {
        if(_bgAudio.clip != null && _bgVideo.clip != null) {
            if ((_bgAudio.time >= 80f || _bgVideo.time >= 80f) && _resetCoroutine == null)
            {
                _resetCoroutine = StartCoroutine(ResetBackgroundTimeCoroutine());
            }
        }
    }
    #endregion

    #region Background Control

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
        yield return new WaitForSeconds(1f);

        // 1초 페이드 인
        _fadeCoroutine = StartCoroutine(FadeInMedia(1.5f, songName));
        yield return _fadeCoroutine;
    }

    /// <summary>
    /// 노래/뮤비 페이드 인(볼륨 0.5→1, alpha 0→1)
    /// </summary>
    private IEnumerator FadeInMedia(float fadeDuration, string songName)
    {
        float elapsed = 0f;
        float fadeOutTime = 0.5f;

        // 이전 하이라이트 음악 볼륨 감소
        if (_bgAudio.volume >= 1f)
        {
            while (elapsed < fadeOutTime)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / fadeOutTime);
                if (_bgAudio != null)
                    _bgAudio.volume = Mathf.Lerp(1f, 0f, t);
                yield return null;
            }
        }

        if (_bgAudio != null)
        {
            _bgAudio.volume = 0.5f;
            AudioClip clip = Resources.Load<AudioClip>($"Song/{songName}/{songName}");
            _bgAudio.clip = clip;
            _bgAudio.Play();
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

        elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration);

            if (_bgAudio != null)
                _bgAudio.volume = Mathf.Lerp(0.3f, 1f, t);

            if (_bgCanvasGroup != null)
                _bgCanvasGroup.alpha = Mathf.Lerp(0.7f, 1f, t);

            yield return null;
        }

        // 최종 보정
        if (_bgAudio != null) _bgAudio.volume = 1f;
        if (_bgCanvasGroup != null) _bgCanvasGroup.alpha = 1f;
    }

    /// <summary>
    /// 오디오 또는 비디오가 75초에 도달하면 1.5초 동안 페이드 후 재생 위치를 60초로 리셋
    /// </summary>
    private IEnumerator ResetBackgroundTimeCoroutine()
    {
        float fadeDuration = 1f;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration);

            if (_bgAudio != null)
                _bgAudio.volume = Mathf.Lerp(0.7f, 1f, t);

            if (_bgCanvasGroup != null)
                _bgCanvasGroup.alpha = Mathf.Lerp(0.7f, 1f, t);

            yield return null;
        }

        if (_bgAudio.clip != null && _bgAudio.clip != null) {
            _bgAudio.time = 60f;
            _bgVideo.time = 60f;
        }
        
        yield return new WaitForSeconds(1f);
        _resetCoroutine = null; // 코루틴 종료 표시
    }

    /// <summary>
    /// 다른 곡이 하이라이트되면 이전 곡의 하이라이트 정지
    /// </summary>
    public void StopHighlight()
    {
        if (_highlightCoroutine != null)
        {
            StopCoroutine(_highlightCoroutine);
            _highlightCoroutine = null;
        }
    }
    #endregion

    #region ReSources
    public AudioSource GetAudioSource()
    {
        return _bgAudio;
    }

    public VideoPlayer GetVideoPlayer()
    {
        return _bgVideo;
    }
    #endregion
}
using UnityEngine;
using UnityEngine.UI;

namespace SongList{
    public class HighlightSong : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SongListManager _songListManager; // SongListManager 참조
    [SerializeField] private Text _detailTitleText; // 좌측에 표시할 제목 Text
    [SerializeField] private Image _detailImage; // 좌측에 표시할 이미지
    [SerializeField] private Button startButton;

    private void OnEnable()
    {
        Debug.Log("ddddddddddddddddddddd");

        if (_songListManager != null)
        {
            _songListManager.OnHighlightedSongChanged += HandleHighlightedSongChanged;
            Debug.Log("[HighlightSong] Event subscribed.");
        }
        else
        {
            Debug.LogWarning("[HighlightSong] _songListManager is not assigned.");
        }
    }

    private void OnDisable()
    {
        if (_songListManager != null)
        {
            _songListManager.OnHighlightedSongChanged -= HandleHighlightedSongChanged;
            Debug.Log("[HighlightSong] Event unsubscribed.");
        }
    }

    private void HandleHighlightedSongChanged(string songTitle)
    {
        Debug.Log($"[HighlightSong] HandleHighlightedSongChanged 호출됨: {songTitle}");

        if (!string.IsNullOrEmpty(songTitle))
        {
            // LoadManager에서 해당 제목을 가진 SongInfo 찾기
            SongInfo songInfo = LoadManager.Instance.Songs.Find(song => song.Title == songTitle);

            if (songInfo != null)
            {
                // 제목 업데이트
                if (_detailTitleText != null)
                {
                    _detailTitleText.text = songInfo.Title;
                    startButton.onClick.AddListener(() => GameManager.Instance.StartGame(songInfo.Title));
                }

                // 이미지 업데이트
                if (_detailImage != null)
                {
                    // Resources 폴더에서 {Title}.png 스프라이트 로드
                    // Resources/Song/{SongTitle}.png 경로를 사용
                    Sprite songSprite = Resources.Load<Sprite>($"Song/{songInfo.Title}/{songInfo.Title}");
                    if (songSprite != null)
                    {
                        _detailImage.sprite = songSprite;
                    }
                    else
                    {
                        Debug.LogWarning($"[HighlightSong] Could not find sprite for title '{songInfo.Title}' in Resources/Song.");
                        // 기본 이미지로 설정하거나 비활성화
                        _detailImage.sprite = null;
                    }
                }
            }
            else
            {
                Debug.LogWarning($"[HighlightSong] No SongInfo found for title '{songTitle}'.");
            }
        }
    }
}
}

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

//MusicItem Prefab에 사용할 스크립트
/// <summary>
/// 로비에서 음악 정보 확인용 GameObject 스크립트
/// </summary>
public class MusicItem : MonoBehaviour
{
    [Header("음악 관련 정보(UI용)")]
    private string mPath;
    [SerializeField] private Text title;
    [SerializeField] private Text artist;
    [SerializeField] private string length;
    [SerializeField] private Image albumArt;
    [SerializeField] private AudioClip audioClip;
    [SerializeField] private VideoClip musicVideo;
    [SerializeField] private bool hasBMS = false;
    [SerializeField] private bool hasMV = false;
    [Header("기록 관련 정보(UI용)")]
    [SerializeField] private string logId;
    [SerializeField] private string score;
    [SerializeField] private string accuracy;
    [SerializeField] private string combo;
    [SerializeField] private Text rank;
    [SerializeField] private string musicID;

    [Header("Item 상태 정보(UI용)")]
    [SerializeField] private Image background;
    [SerializeField] private Image rightCircle;
    [SerializeField] private Color wMint = new Color(104.0f, 240.0f, 235.0f, 0.08f);
    [SerializeField] private Color sMint = new Color(57, 255, 255, 1.0f);
    [SerializeField] private Color purple = new Color(155.0f, 48.0f, 255.0f, 1.0f);
    [SerializeField] private MusicData mData;
    [SerializeField] private bool isDummy = true;


    #region Get/Set
    public string DirPath { get { return mPath; } set { mPath = value; } }
    public Text Title { get { return title; } set { title = value; } }
    public Text Artist { get { return artist; } set { artist = value; } }
    public string Length { get { return length; } set { length = value; } }
    public Image Album { get { return albumArt; } set { albumArt = value; } }
    public AudioClip Audio { get { return audioClip;  }set { audioClip = value; } }
    public VideoClip MuVi { get { return musicVideo; } set { musicVideo = value; } }
    public bool HasBMS { get { return hasBMS; } set { hasBMS = value; } }
    public bool HasMV { get { return hasMV; } set { hasMV = value; } }

    public string LogID { get { return logId; } set { logId = value; } }
    public string Score { get { return score; } set { score = value; } }
    public string Accuracy { get { return accuracy; } set { accuracy = value; } }
    public string Combo { get { return combo; } set { combo = value; } }
    public Text Rank { get { return rank; } set { rank = value; } }
    public MusicData Data { get { return mData; }set { mData = value; } }
    public bool IsDummy { get { return isDummy; } set { isDummy = value; } }
    public string MusicID {  get { return musicID; } set { musicID= value; } }
    #endregion

    /*
    //List에서 Item값 바꿀 때 사용할 메서드
    public void SetData(string newTitle = "Title", string newArtist = "Artist", string newLength = "00:00",
                        Sprite newAlbum = null, VideoClip newMuvi = null, 
                        string newScore = "0", string newAccuracy = "0", string newCombo = "-", string newRank = "0")
    {
        title.text = newTitle;
        artist.text = newArtist;
        length = newLength;
        if (newAlbum != null) {
            albumArt.sprite = newAlbum;
        }
        musicVideo = newMuvi;
        score = newScore;
        accuracy = newAccuracy;
        combo = newCombo;
        rank.text = newRank;
    }*/

    #region Select Reaction
    public void ItemSelect()
    {
        
        SetColor(background, purple);
        SetColor(rightCircle, purple);

        SetAlpha(title, 1.0f);
        SetAlpha(artist, 1.0f);
        SetAlpha(rank, 1.0f);

        SetAlpha(albumArt, 1.0f);
    }

    public void ItemUnselect()
    {
        SetColor(background, wMint);
        SetColor(rightCircle, wMint);
        
        SetAlpha(title, 0.5f);
        SetAlpha(artist, 0.5f);
        SetAlpha(rank, 0.5f);
        
        SetAlpha(albumArt, 0.5f);
    }

    //투명도만 바꾸기
    void SetAlpha(Graphic target, float alpha)
    {
        Color Temp = target.color;
        Temp.a = alpha;
        target.color = Temp;
    }

    //색상 바꾸기
    void SetColor(Graphic target, Color color)
    {
        target.color = color;
    }

    #endregion

}

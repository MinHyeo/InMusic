using UnityEngine;
using UnityEngine.Video;

public class MusicData : MonoBehaviour
{
    /*MusicDataBox Prefab의 Inspactor창에서 기본값 할당*/
    [Header("음악 관련 정보")]
    [SerializeField] private string title;
    [SerializeField] private string artist;
    [SerializeField] private string length;
    [SerializeField] private Sprite albumArt;
    [SerializeField] private AudioClip audioClip;
    [SerializeField] private VideoClip musicVideo;
    [Header("기록 관련 정보")]
    [SerializeField] private string score;
    [SerializeField] private string accuracy;
    [SerializeField] private string combo;
    [SerializeField] private string rank;

    public string Title { get { return title; } set { title = value; } }
    public string Artist { get { return artist; } set { artist = value; } }
    public string Length { get { return length; } set { length = value; } }
    public Sprite Album { get { return albumArt; } set { albumArt = value; } }
    public AudioClip Audio { get { return audioClip; } set { audioClip = value; } }
    public VideoClip MuVi { get { return musicVideo; } set { musicVideo = value; } }
    public string Score { get { return score; } set { score = value; } }
    public string Accuracy { get { return accuracy; } set { accuracy = value; } }
    public string Combo { get { return combo; } set { combo = value; } }
    public string Rank { get { return rank; } set { rank = value; } }
}

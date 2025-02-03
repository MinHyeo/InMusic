using UnityEngine;
using UnityEngine.Video;

public class MusicData : MonoBehaviour
{
    /*MusicDataBox Prefab의 Inspactor창에서 기본값 할당*/
    [Header("파일 관련 정보")]
    [SerializeField] string mPath;
    [Header("음악 관련 정보")]
    [SerializeField] BMSData mBMS;
    [SerializeField] private string length;
    [SerializeField] private Sprite albumArt;
    [SerializeField] private AudioClip audioClip;
    [SerializeField] private VideoClip musicVideo;
    [SerializeField] private bool hasBMS = false;
    [Header("기록 관련 정보")]
    [SerializeField] private string score = "0";
    [SerializeField] private string accuracy = "0%";
    [SerializeField] private string combo = "0";
    [SerializeField] private string rank = "-";


    public string DirPath { get { return mPath; } set { mPath = value; } }
    public BMSData BMS { get { return mBMS; } set { mBMS = value; } }
    public string Title { get { return mBMS.header.title; } set { mBMS.header.title = value; } }
    //Artist 메서드는 문제가 있어서 사용 안함
    public string Artist { get { return mBMS.header.artist; } set { mBMS.header.artist = value; } }
    public string Length { get { return length; } set { length = value; } }
    public Sprite Album { get { return albumArt; } set { albumArt = value; } }
    public AudioClip Audio { get { return audioClip; } set { audioClip = value; } }
    public VideoClip MuVi { get { return musicVideo; } set { musicVideo = value; } }
    public bool HasBMS { get { return hasBMS; } set { hasBMS = value; } }

    public string Score { get { return score; } set { score = value; } }
    public string Accuracy { get { return accuracy; } set { accuracy = value; } }
    public string Combo { get { return combo; } set { combo = value; } }
    public string Rank { get { return rank; } set { rank = value; } }
}

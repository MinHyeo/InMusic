using Microsoft.Unity.VisualStudio.Editor;
using System.IO;
using UnityEngine;
using UnityEngine.Video;

public enum Song 
{
    나는아픈건딱질색이니까 = 0,
    첫만남은계획대로되지않아 = 1,
    클락션 = 2,
    Heya = 3,
    Armageddon = 4,
    BubbleGum = 5,
    HotSweet = 6,
    Magentic = 7,
    Sticky = 8,
    Supernova = 9,
}

public class SongInfo : MonoBehaviour
{
    public string part;
    public string Title;
    public string Artist;
    public string Genre;
    public float BPM;
    public float PlayLevel;
    public float Rank;
    public string NoteCount;
}

public class SongInfoLoading : MonoBehaviour
{
    public string[] songList;       // 곡 이름 리스트
    public int index;               // 선택된 곡 인덱스

    private FileInfo fileName = null;
    private StreamReader reader = null;
    private string path;            // 곡 파일 path
    private string StrText;         // 파일 한 줄씩 읽어을 때 사용할 변수
    private string songName;        // 곡 제목

    private char[] seps;            // 구분자 저장할 배열
    private string[] tempSplit;     // 구분자로 나눈 문자열을 저장할 임시 문자열 배열
    private string tempStr;         // 구분자로 나눈 문자열을 저장할 변수

    private void Start()
    {
        SelectSong(Song.Heya);
    }

    public void SelectSong(Song song)
    {
        this.index = (int)song;  //임시 값
        tempSplit = null;
        tempStr = "";
        StrText = "";
        songName = "";
        path = "Assets/Resources/Song/";
        seps = new char[] { ' ', ':' };

        songName = songList[index];
        path += songName + "/";
        fileName = new FileInfo(path + songList[index] + ".bms");

        if (fileName != null)
        {
            reader = fileName.OpenText();
        }
        else
        {
            Debug.Log("파일이 없습니다.");
        }

        ParseBMS();
    }

    public SongInfo ParseBMS()
    {
        SongInfo bmsData = new SongInfo();

        while ((tempStr = reader.ReadLine()) != null)
        {
            string trimmedLine = tempStr.Trim();

            // BMS 파일의 헤더 처리
            if (trimmedLine.StartsWith("#TITLE"))
            {
                bmsData.Title = trimmedLine.Split(' ')[1];
            }
            else if (trimmedLine.StartsWith("#GENRE"))
            {
                bmsData.Genre = trimmedLine.Split(' ')[1];
            }
            else if (trimmedLine.StartsWith("#ARTIST"))
            {
                bmsData.Artist = trimmedLine.Split(' ')[1];
            }
            else if (trimmedLine.StartsWith("#BPM"))
            {
                bmsData.BPM = float.Parse(trimmedLine.Split(' ')[1]);
            }
            else if (trimmedLine.StartsWith("#PLAYLEVEL"))
            {
                bmsData.PlayLevel = float.Parse(trimmedLine.Split(' ')[1]);
            }
            else if (trimmedLine.StartsWith("#RANK"))
            {
                bmsData.Rank = float.Parse(trimmedLine.Split(' ')[1]);
            }
        }

        return bmsData;
    }
}

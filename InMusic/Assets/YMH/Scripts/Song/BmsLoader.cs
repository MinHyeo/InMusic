using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;

public enum Song
{
    sksmsdkvmsrjsEkrwlftordlslRk = 0,
    cjtaksskadmsrPghlreofhehlwldksgdk = 1,
    Klaxon = 2,
    Heya = 3,
    Armageddon = 4,
    BubbleGum = 5,
    HotSweet = 6,
    Magentic = 7,
    Sticky = 8,
    Supernova = 9,
}

[System.Serializable]
public class SongInfo
{
    public string Part;
    public string Title;
    public string Artist;
    public string Genre;
    public float BPM;
    public float PlayLevel;
    public float Rank;
    public List<NoteData> NoteList;
    public int NoteCount;

    public override string ToString()
    {
        return string.Format("Title: {0}, Artist: {1}, Genre: {2}, BPM: {3}, PlayLevel: {4}, Rank: {5}, NoteCount: {6}", Title, Artist, Genre, BPM, PlayLevel, Rank, NoteCount);
    }
}

public class BmsLoader : MonoBehaviour
{
    public static BmsLoader Instance;
    private void Awake()
    {
        Instance = this;
    }

    private SongInfo songInfo;

    private FileInfo fileName = null;
    private StreamReader reader = null;
    private string path;            // 곡 파일 path
    private string StrText;         // 파일 한 줄씩 읽어을 때 사용할 변수
    private string songName;        // 곡 제목
    private int noteCount;           // 노트 개수

    private char[] seps;            // 구분자 저장할 배열
    private string[] tempSplit;     // 구분자로 나눈 문자열을 저장할 임시 문자열 배열
    private string tempStr;         // 구분자로 나눈 문자열을 저장할 변수

    private void Start()
    {
        SelectSong(Song.Heya);
    }

    public SongInfo SelectSong(Song song)
    {
        tempSplit = null;
        tempStr = "";
        StrText = "";
        songName = "";
        path = "Assets/Resources/Song/";
        seps = new char[] { ' ', ':' };

        songName = song.ToString();
        path += songName + "/";
        fileName = new FileInfo(path + song.ToString() + ".bms");

        if (fileName != null)
        {
            reader = fileName.OpenText();
        }
        else
        {
            Debug.Log("파일이 없습니다.");
        }

        songInfo = ParseBMS();

        return songInfo;
    }

    private SongInfo ParseBMS()
    {
        SongInfo bmsData = new SongInfo();
        bmsData.NoteList = new List<NoteData>();
        noteCount = 0;

        while ((tempStr = reader.ReadLine()) != null)
        {
            string trimmedLine = tempStr.Trim();
            if (trimmedLine.StartsWith("#"))
            {
                string[] data = trimmedLine.Split(' ');

                // 데이터 섹션이 아니면서 헤더 데이터가 없는 경우에는 건너 뜀.
                if (data[0].IndexOf(":") == -1 && data.Length == 1)
                {
                    continue;
                }

                // BMS 파일의 헤더 처리
                if (data[0].Equals("#TITLE"))
                {
                    bmsData.Title = data[1];
                }
                else if (data[0].Equals("#GENRE"))
                {
                    bmsData.Genre = data[1];
                }
                else if (data[0].Equals("#ARTIST"))
                {
                    bmsData.Artist = data[1];
                }
                else if (data[0].Equals("#BPM"))
                {
                    bmsData.BPM = int.Parse(data[1]);
                }
                else if (data[0].Equals("#PLAYLEVEL"))
                {
                    bmsData.PlayLevel = float.Parse(data[1]);
                }
                else if (data[0].Equals("#RANK"))
                {
                    bmsData.Rank = float.Parse(data[1]);
                }
                else if (data[0].Equals("#PLAYER"))
                {
                }
                else if (data[0].Equals("#TOTAL"))
                {
                }
                else if (data[0].Equals("#VOLWAV"))
                {
                }
                else if (data[0].Equals("#MIDIFILE"))
                {
                }
                else if (data[0].StartsWith("#WAV"))
                {
                }
                else if (data[0].Equals("#BMP"))
                {
                }
                else if (data[0].Equals("#STAGEFILE"))
                {
                }
                else if (data[0].Equals("#VIDEOFILE"))
                {
                }
                else if (data[0].Equals("#BGA"))
                {
                }
                else if (data[0].Equals("#STOP"))
                {
                }
                else if (data[0].Equals("#LNTYPE"))
                {
                }
                else if (data[0].Equals("#LNOBJ"))
                {
                }
                else
                {
                    // 위의 경우에 모두 해당하지 않을 경우, 데이터 섹션
                    int bar = 0;
                    Int32.TryParse(data[0].Substring(1, 3), out bar);

                    int channel = 0;
                    Int32.TryParse(data[0].Substring(4, 2), out channel);

                    string noteStr = data[0].Substring(7);
                    List<int> noteData = getNoteDataOfStr(noteStr);

                    NoteData note = new NoteData
                    {
                        bar = bar,
                        channel = channel,
                        noteData = noteData
                    };

                    bmsData.NoteList.Add(note);
                }
            }
        }
        bmsData.NoteCount = noteCount;
        songInfo = bmsData;

        return bmsData;
    }

    private List<int> getNoteDataOfStr(string str)
    {
        string tempStr = str;
        List<int> noteList = new List<int>();

        int strLen = tempStr.Length;

        while (true)
        {
            if (tempStr.Length >= 2)
            {
                int data = 0;
                Int32.TryParse(tempStr.Substring(0, 2), out data);

                if (data != 0)
                    noteCount++;

                noteList.Add(data);
                tempStr = tempStr.Substring(2);
            }
            else
            {
                int data = 0;
                Int32.TryParse(tempStr, out data);
                break;
            }
        }

        return noteList;
    }
}
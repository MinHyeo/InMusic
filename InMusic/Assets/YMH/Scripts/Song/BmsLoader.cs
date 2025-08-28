using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;
using Play;
using FMOD.Studio;
using FMODUnity;
using UnityEditor;

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
    public float Duration;
    public List<NoteData> NoteList;
    public int NoteCount;

    public override string ToString()
    {
        return string.Format("Title: {0}, Artist: {1}, Genre: {2}, BPM: {3}, PlayLevel: {4}, Rank: {5}, Duration: {6:F1}s, NoteCount: {7}", 
            Title, Artist, Genre, BPM, PlayLevel, Rank, Duration, NoteCount);
    }
}

public class BmsLoader : Singleton<BmsLoader>
{
    private SongInfo songInfo;

    private FileInfo fileName = null;
    private StreamReader reader = null;
    private string path;            // �� ���� path
    private string StrText;         // ���� �� �پ� �о��� �� ����� ����
    private string songName;        // �� ����
    private int noteCount;           // ��Ʈ ����

    private char[] seps;            // ������ ������ �迭
    private string tempStr;         // �����ڷ� ���� ���ڿ��� ������ ����

    public SongInfo SelectSongByTitle(string songTitle)
    {
        tempStr = "";
        StrText = "";
        songName = songTitle;
        path = Path.Combine(Application.streamingAssetsPath, "Song", songTitle);
        seps = new char[] { ' ', ':' };

        fileName = new FileInfo(Path.Combine(path, songTitle + ".bms"));

        if (!fileName.Exists) // 디렉토리나 파일이 없는 경우 처리
        {
            Debug.LogWarning($"File not found: {fileName.FullName}. Returning dummy data for testing.");
            return GenerateDummyData(songTitle);
        }

        reader = fileName.OpenText();
        songInfo = ParseBMS(songTitle);
        songInfo.Duration = GetSongDurationFromFMOD(songTitle);

        return songInfo;
    }

    /// <summary>
    /// FMOD에서 User Property로 곡의 재생시간을 가져옵니다.
    /// </summary>
    private float GetSongDurationFromFMOD(string songTitle)
    {
        string musicEventPath = "event:/Musics/" + songTitle;
        
        EventDescription eventDescription;
        var result = RuntimeManager.StudioSystem.getEvent(musicEventPath, out eventDescription);
        
        if (result == FMOD.RESULT.OK && eventDescription.isValid())
        {
            USER_PROPERTY userProperty;
            var propertyResult = eventDescription.getUserProperty("Duration", out userProperty);
            
            if (propertyResult == FMOD.RESULT.OK)
            {
                if (userProperty.type == USER_PROPERTY_TYPE.FLOAT)
                {
                    float duration = userProperty.floatValue();
                    Debug.Log($"[BmsLoader] Duration: {duration}s for {songTitle}");
                    return duration;
                }
                else if (userProperty.type == USER_PROPERTY_TYPE.STRING)
                {
                    string value = userProperty.stringValue();
                    if (float.TryParse(value, out float duration))
                    {
                        Debug.Log($"[BmsLoader] Duration: {duration}s for {songTitle}");
                        return duration;
                    }
                }
            }
        }
        
        Debug.LogWarning($"[BmsLoader] Failed to get duration for {songTitle}");
        return 0f;
    }

    private SongInfo GenerateDummyData(string songName)
    {
        // 테스트용 기본 SongInfo 데이터를 생성
        SongInfo dummyData = new SongInfo
        {
            Title = songName,
            Artist = "Unknown Artist",
            Genre = "Test Genre",
            BPM = 120,
            PlayLevel = 5,
            Rank = 1,
            Duration = 754.0f,
            NoteList = new List<NoteData>
            {
                new NoteData { bar = 1, channel = 1, noteData = new List<int> { 1, 2, 3 } },
                new NoteData { bar = 2, channel = 2, noteData = new List<int> { 4, 5, 6 } }
            },
            NoteCount = 2
        };

        return dummyData;
    }


    private SongInfo ParseBMS(string songTitle)
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

                // ������ ������ �ƴϸ鼭 ��� �����Ͱ� ���� ��쿡�� �ǳ� ��.
                if (data[0].IndexOf(":") == -1 && data.Length == 1)
                {
                    continue;
                }

                // BMS ������ ��� ó��
                if (data[0].Equals("#TITLE"))
                {
                    bmsData.Title = songTitle;
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
                    // ���� ��쿡 ��� �ش����� ���� ���, ������ ����
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
                int.TryParse(tempStr.Substring(0, 2), out data);

                if (data != 0)
                    noteCount++;

                noteList.Add(data);
                tempStr = tempStr.Substring(2);
            }
            else
            {
                int data = 0;
                int.TryParse(tempStr, out data);
                break;
            }
        }

        return noteList;
    }
}
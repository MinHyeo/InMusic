using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;
using Play;
using FMOD.Studio;
using FMODUnity;

public enum Song
{
    sksmsdkvmsRJsEkrwlftordlslRk = 0,
    cjtaksskadmsrPghlreofhehlwldksgdk = 1,
    Klaxon = 2,
    Heya = 3,
    Armageddon = 4,
    BubbleGum = 5,
    HotSweet = 6,
    Magnetic = 7,
    Sticky = 8,
    Supernova = 9,
    // dummy10 = 10,
    // dummy11 = 11,
    // dummy12 = 12,
    // dummy13 = 13,
    // dummy14 = 14,
    // dummy15 = 15,
    // dummy16 = 16,
    // dummy17 = 17,
    // dummy18 = 18,
    // dummy19 = 19,
    // dummy20 = 20,
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
    public float Duration;
    public List<NoteData> NoteList;
    public int NoteCount;

    public override string ToString()
    {
        return string.Format("Title: {0}, Artist: {1}, Genre: {2}, BPM: {3}, PlayLevel: {4}, Rank: {5}, Duration: {6:F1}s, NoteCount: {7}", 
            Title, Artist, Genre, BPM, PlayLevel, Rank, Duration, NoteCount);
    }
}

public class BmsLoader : SingleTon<BmsLoader>
{
    private SongInfo songInfo;

    private FileInfo fileName = null;
    private StreamReader reader = null;
    private string path;            // ?? ???? path
    private string StrText;         // ???? ?? ??? ?��??? ?? ????? ????
    private string songName;        // ?? ????
    private int noteCount;           // ??? ????

    private char[] seps;            // ?????? ?????? ?��
    private string tempStr;         // ??????? ???? ??????? ?????? ????

    private void Start()
    {
        SelectSong(Song.Heya);
    }

    public SongInfo SelectSong(Song song)
    {
        tempStr = "";
        StrText = "";
        songName = "";
        path = Path.Combine(Application.streamingAssetsPath, "Song", song.ToString());
        seps = new char[] { ' ', ':' };

        songName = song.ToString();
        //path += songName + "/";
        fileName = new FileInfo(Path.Combine(path, song.ToString() + ".bms"));

        if (!fileName.Exists) // ���丮�� ������ ���� ��� ó��
        {
            Debug.LogWarning($"File not found: {fileName.FullName}. Returning dummy data for testing.");
            return GenerateDummyData(songName);
        }

        reader = fileName.OpenText();
        songInfo = ParseBMS();
        songInfo.Duration = GetSongDurationFromFMOD(song.ToString());

        return songInfo;
    }

    /// <summary>
    /// FMOD���� ���� ����ð��� �����ɴϴ�. �����ϸ� 0�� ��ȯ�մϴ�.
    /// </summary>
    private float GetSongDurationFromFMOD(string songTitle)
    {
        string bgmEventPath = "event:/BGM/" + songTitle;
        Debug.Log($"[BmsLoader] Trying FMOD path: {bgmEventPath}");
        
        EventDescription eventDescription;
        var result = RuntimeManager.StudioSystem.getEvent(bgmEventPath, out eventDescription);
        
        Debug.Log($"[BmsLoader] FMOD getEvent result: {result}");
        
        if (result == FMOD.RESULT.OK && eventDescription.isValid())
        {
            int lengthMs;
            var lengthResult = eventDescription.getLength(out lengthMs);
            Debug.Log($"[BmsLoader] getLength result: {lengthResult}, length: {lengthMs}ms");
            
            float durationSeconds = lengthMs / 1000f;
            Debug.Log($"[BmsLoader] Final duration: {durationSeconds}s for {songTitle}");
            return durationSeconds;
        }
        else
        {
            Debug.LogWarning($"[BmsLoader] Failed to get FMOD event: {bgmEventPath}, result: {result}");
        }
        
        return 0f; // �����ϸ� 0 ��ȯ
    }

    private SongInfo GenerateDummyData(string songName)
    {
        // �׽�Ʈ�� �⺻ SongInfo �����͸� ����
        SongInfo dummyData = new SongInfo
        {
            Title = songName,
            Artist = "Unknown Artist",
            Genre = "Test Genre",
            BPM = 120,
            PlayLevel = 5,
            Rank = 1,
            NoteList = new List<NoteData>
            {
                new NoteData { bar = 1, channel = 1, noteData = new List<int> { 1, 2, 3 } },
                new NoteData { bar = 2, channel = 2, noteData = new List<int> { 4, 5, 6 } }
            },
            NoteCount = 2
        };

        return dummyData;
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

                // ?????? ?????? ???? ??? ??????? ???? ??��?? ??? ??.
                if (data[0].IndexOf(":") == -1 && data.Length == 1)
                {
                    continue;
                }

                // BMS ?????? ??? ???
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
                    // ???? ??�� ??? ??????? ???? ???, ?????? ????
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
using FMOD.Studio;
using Microsoft.Unity.VisualStudio.Editor;
using System.Collections.Generic;
using System;
using System.Data.Common;
using System.IO;
using UnityEngine;
using UnityEngine.Video;

public enum Song 
{
    ���¾��°ǵ������̴ϱ� = 0,
    ù��������ȹ��ε����ʾ� = 1,
    Ŭ���� = 2,
    Heya = 3,
    Armageddon = 4,
    BubbleGum = 5,
    HotSweet = 6,
    Magentic = 7,
    Sticky = 8,
    Supernova = 9,
}

public class SongInfo
{
    public string part;
    public string Title;
    public string Artist;
    public string Genre;
    public float BPM;
    public float PlayLevel;
    public float Rank;
    public List<Note> noteList;
    public int NoteCount;

    public override string ToString()
    {
        return string.Format("Title: {0}, Artist: {1}, Genre: {2}, BPM: {3}, PlayLevel: {4}, Rank: {5}, NoteCount: {6}", Title, Artist, Genre, BPM, PlayLevel, Rank, NoteCount);
    }
}

public class BmsLoader : MonoBehaviour
{
    public SongInfo songInfo;
    public string[] songList;       // �� �̸� ����Ʈ
    public int index;               // ���õ� �� �ε���

    private FileInfo fileName = null;
    private StreamReader reader = null;
    private string path;            // �� ���� path
    private string StrText;         // ���� �� �پ� �о��� �� ����� ����
    private string songName;        // �� ����

    private char[] seps;            // ������ ������ �迭
    private string[] tempSplit;     // �����ڷ� ���� ���ڿ��� ������ �ӽ� ���ڿ� �迭
    private string tempStr;         // �����ڷ� ���� ���ڿ��� ������ ����

    private void Start()
    {
        SelectSong(Song.Heya);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log(songInfo);
        }
    }

    public void SelectSong(Song song)
    {
        this.index = (int)song;  //�ӽ� ��
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
            Debug.Log("������ �����ϴ�.");
        }

        ParseBMS();
    }

    public SongInfo ParseBMS()
    {
        SongInfo bmsData = new SongInfo();

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
                else
                {
                    // ���� ��쿡 ��� �ش����� ���� ���, ������ ����
                    int bar = 0;
                    Int32.TryParse(data[0].Substring(1, 3), out bar);

                    int channel = 0;
                    Int32.TryParse(data[0].Substring(4, 2), out channel);

                    string noteStr = data[0].Substring(7);
                    List<int> noteData = getNoteDataOfStr(noteStr);

                    Note note = new Note
                    {
                        bar = bar,
                        channel = channel,
                        noteData = noteData
                    };
                    songInfo.noteList.Add(note);
                }
            }
        }

        songInfo = bmsData;
        Debug.Log(songInfo);

        return bmsData;
    }

    private List<int> getNoteDataOfStr(string str)
    {
        string tempStr = str;
        List<int> noteList = new List<int>();

        while (true)
        {
            if (tempStr.Length > 2)
            {
                int data = 0;
                Int32.TryParse(tempStr.Substring(0, 2), out data);

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

        songInfo.NoteCount = 0;
        //�ѳ�Ʈ�� ����
        foreach (int note in noteList)
        {
            if(note != 0)
            {
                songInfo.NoteCount++;
            }
        }

        return noteList;
    }
}
using System;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class BMSManager : MonoBehaviour
{

    public static BMSManager Instance { get; private set; }

    void Awake()
    {
        // 싱글톤 인스턴스 설정
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        
        //BMSData parsedData = ParseBMS("test");
        //if (parsedData != null)
        //{
        //    Debug.Log("BMS Parsing complete.");
        //    Debug.Log("Title: " + parsedData.header.title);
        //    Debug.Log("Artist: " + parsedData.header.artist);
        //    Debug.Log("Number of notes: " + parsedData.notes.Count);
        //    foreach (BMSNoteData note in parsedData.notes)
        //    {
        //        Debug.Log($"Measure: {note.measure}, Channel: {note.channel}, Notes: {note.noteString}");
        //    }
        //}
    }

    // Update is called once per frame
    void Update()
    {

    }

    public BMSData ParseBMS(string fileName)
    {
        BMSData bmsData = new BMSData();

        TextAsset bmsFile = Resources.Load<TextAsset>(fileName);
        if (bmsFile == null)
        {
            Debug.LogError($"BMS 못 찾음: {fileName}");
            return null;
        }
        string[] lines = bmsFile.text.Split(new[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);

        foreach (string line in lines)
        {
            if (string.IsNullOrWhiteSpace(line) || line.StartsWith("*") || line.StartsWith("//"))
                continue;

            if (line.StartsWith("#"))
            {
                ParseLine(line, bmsData);
            }
        }





        return bmsData;
    }

    private void ParseLine(string line, BMSData bmsData)
    {
        if (line.StartsWith("#PLAYER"))
        {
            bmsData.header.player = int.Parse(line.Split(' ')[1]);
        }
        else if (line.StartsWith("#GENRE"))
        {
            bmsData.header.genre = line.Substring(7).Trim();
        }
        else if (line.StartsWith("#TITLE"))
        {
            bmsData.header.title = line.Substring(7).Trim();
        }
        else if (line.StartsWith("#ARTIST"))
        {
            bmsData.header.artist = line.Substring(8).Trim();
        }
        else if (line.StartsWith("#BPM"))
        {
            bmsData.header.bpm = float.Parse(line.Split(' ')[1]);
        }
        else if (line.StartsWith("#PLAYLEVEL"))
        {
            bmsData.header.playLevel = int.TryParse(line.Split(' ')[1], out int level) ? level : 0;
        }
        else if (line.StartsWith("#RANK"))
        {
            bmsData.header.rank = int.Parse(line.Split(' ')[1]);
        }
        else if (line.StartsWith("#LNTYPE"))
        {
            bmsData.header.LNType = int.Parse(line.Split(' ')[1]);
        }
        else if (line.StartsWith("#WAV"))
        {
            // WAV 처리
        }
        else
        {
            if (line.Length >= 6 && int.TryParse(line.Substring(1, 3), out int measure))
            {
                int channel = int.Parse(line.Substring(4, 2));
                string noteData = line.Split(':')[1].Trim();

                BMSNoteData note = new BMSNoteData
                {
                    measure = measure,
                    channel = channel,
                    noteString = noteData
                };
                bmsData.notes.Add(note);
            }
        }
    }
}

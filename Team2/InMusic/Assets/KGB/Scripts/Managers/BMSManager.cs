using System;
using System.IO;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class BMSManager
{

    public BMSData ParseBMS(string fileName)
    {
        BMSData bmsData = new BMSData();
        string fileContent = null;
        string filePath = Path.Combine(Application.dataPath, "Resources", fileName + ".bms");
        if (File.Exists(filePath))
        {
            // 파일 내용을 읽어서 string에 저장
            fileContent = File.ReadAllText(filePath);
            Debug.Log("BMS파일 찾음");
        }
        else
        {
            Debug.Log("BMS파일없음: "+ filePath);
            Debug.LogError($"BMS 못 찾음: {fileName}");
            return null;
        }

        string[] lines = fileContent.Split(new[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);

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

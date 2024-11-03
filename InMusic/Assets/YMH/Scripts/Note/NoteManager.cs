using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Note
{
    public int bar;
    public int channel;
    public List<int> noteData;
}

public class NoteManager : MonoBehaviour
{
    private List<Note> notes = new List<Note>();
    private string bmsFilePath = "Assets/Resources/Song/Heya/Heya.bms";

    void Start()
    {
        ParseBMS(bmsFilePath);
        Debug.Log("Total Notes: " + notes.Count);
    }

    void ParseBMS(string filePath)
    {
        using (StreamReader reader = new StreamReader(filePath))
        {
            string line;
            int currentMeasure = 0;

            while ((line = reader.ReadLine()) != null)
            {
                string trimmedLine = line.Trim();

                // 마디 번호 파싱 (예: #00111)
                if (trimmedLine.StartsWith("#") && trimmedLine.Length >= 6)
                {
                    string measureString = trimmedLine.Substring(1, 3); // #XXX에서 마디 번호
                    Debug.Log(measureString);
                    int measure = int.Parse(measureString); // 마디 번호
                    currentMeasure = measure;

                    string channelString = trimmedLine.Substring(4, 2); // 채널 번호
                    int channel = int.Parse(channelString); // 채널 번호

                    // 노트 데이터 (00 01 02 등)
                    string noteData = trimmedLine.Substring(7); // 노트 데이터

                    // 노트가 있는 채널인지 확인 (예: 11~19, 21~29 등)
                    if (IsNoteChannel(channel))
                    {
                        ParseNotesInChannel(noteData, currentMeasure, channel);
                    }
                }
            }
        }
    }
    bool IsNoteChannel(int channel)
    {
        return (channel >= 11 && channel <= 19) || (channel >= 21 && channel <= 29);
    }

    // 특정 채널에서 노트를 파싱하는 함수
    void ParseNotesInChannel(string noteData, int measure, int channel)
    {
        int noteCount = noteData.Length / 2; // 노트 데이터는 2자리씩 끊어서 읽음

        // 노트 데이터 파싱 (2글자씩 읽어옴)
        for (int i = 0; i < noteCount; i++)
        {
            string noteValue = noteData.Substring(i * 2, 2);

            // '00'은 노트가 없음을 의미
            if (noteValue != "00")
            {
                // 노트의 위치는 마디 내에서 상대적 위치 (0.0 ~ 1.0)
                float position = (float)i / noteCount;

                // 노트 객체 생성 및 리스트에 추가
                Note note = new Note
                {
                    //channel = channel,
                    //measure = measure,
                    //position = position,
                    //value = noteValue
                };

                notes.Add(note);
            }
        }
    }
}
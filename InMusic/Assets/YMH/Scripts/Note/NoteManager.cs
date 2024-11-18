using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class NoteData
{
    public int bar;
    public int channel;
    public List<int> noteData;

    public override string ToString()
    {
        return string.Format("Bar: {0}, Channel: {1}, NoteData: {2}", bar, channel, noteData);
    }
}

public class NoteManager : MonoBehaviour
{
    public static NoteManager instance;  // 싱글톤 인스턴스
    [SerializeField] private GameObject notePrefab;
    [SerializeField] private Transform[] noteSpawnPoints; // 각 채널별 노트 생성 위치
    [SerializeField] private float noteSpeed = 5.0f;      // 노트 이동 속도
    [SerializeField] private Transform judgementLine;

    private List<NoteData> noteDataList;
    private float songStartTime;
    private float travelTime; // 판정선까지 도달하는 시간
    private bool isPlaying = false;

    private void Awake()
    {
        instance = this;
    }

    public void InitializeNotes(SongInfo songInfo)
    {
        noteDataList = songInfo.NoteList;

        // 노트 생성 시 음악과의 타이밍 맞추기 위해 초기화
        travelTime = (noteSpawnPoints[0].position.y - judgementLine.position.y) / noteSpeed;
        songStartTime = Time.time + Metronome.instance.preStartDelay; // 노래 시작 시간 설정

        StartCoroutine(SpawnNotes());
    }

    private IEnumerator SpawnNotes()
    {
        foreach (NoteData noteData in noteDataList)
        {
            float barTime = GetBarTime(noteData.bar);
            int divisions = noteData.noteData.Count;

            for (int i = 0; i < divisions; i++)
            {
                int noteValue = noteData.noteData[i];
                if (noteValue != 0)
                {
                    int channel = noteData.channel;

                    // 노트 생성 시점 계산
                    float spawnTime = (barTime / divisions) * i;
                    float noteAppearTime = songStartTime + barTime + spawnTime - travelTime;

                    // 노트 생성 예약
                    StartCoroutine(SpawnNoteCoroutine(channel, noteAppearTime));
                }
            }

            yield return null;
        }
    }

    private IEnumerator SpawnNoteCoroutine(int channel, float spawnTime)
    {
        // spawnTime까지 대기
        float waitTime = spawnTime - Time.time;
        if (waitTime > 0)
        {
            yield return new WaitForSeconds(waitTime);
        }

        // 노트 생성
        GameObject note = Instantiate(notePrefab, noteSpawnPoints[channel - 11].position, Quaternion.identity);
        Note noteScript = note.GetComponent<Note>();
        noteScript.Initialize(noteSpeed, judgementLine.position.y);
    }

    private float GetBarTime(int bar)
    {
        // 한 마디의 길이 계산 (BPM에 따라 변경 가능)
        float beatTime = 60f / BmsLoader.instance.songInfo.BPM;
        float barTime = beatTime * 4.0f; // 4/4박자 기준
        return barTime;
    }
}
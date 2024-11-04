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
    public GameObject notePrefab;      // 노트 프리팹
    public Transform[] spawnPositions;  // 노트가 떨어질 4개의 라인 위치
    public float bpm;           // BPM 값
    [SerializeField]
    private List<NoteData> notes;       // 파싱된 노트 데이터를 담을 리스트
    private float beatInterval;
    private float songStartTime;

    private bool isStart = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("space바 입력");
            // BMS에서 파싱된 노트 데이터를 가져옴
            notes = BmsLoader.instance.songInfo.NoteList;
            bpm = BmsLoader.instance.songInfo.BPM;

            // 타이밍 계산
            float bps = bpm / 60.0f;
            beatInterval = 1.0f / bps;
            songStartTime = Time.time;  // 음악 시작 시간

            isStart = true;

            //SpawnNotes();
            StartCoroutine(SpawnNotesCoroutine());
        }
    }

    private IEnumerator SpawnNotesCoroutine()
    {
        foreach(NoteData note in notes)
        {
            float spawnTime = songStartTime + (note.bar * beatInterval * 4);

            yield return new WaitForSeconds(spawnTime - Time.deltaTime);

            for(int i = 0; i < note.noteData.Count; i++)
            {
                if (note.noteData[i] != 0)
                {
                    Transform spwanPosition = spawnPositions[i];
                    GameObject newNote = Instantiate(notePrefab, spwanPosition.position, Quaternion.identity);
                    newNote.GetComponent<Note>().Initialize(note, bpm);
                }
            }
        }
    }
}
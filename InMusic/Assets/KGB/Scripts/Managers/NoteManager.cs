using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class NoteManager : MonoBehaviour
{
    public ObjectPool measureLinePool; // 경계선 풀
    public ObjectPool notePool1;       // 노트 풀 (1번 키, 4번 키)
    public ObjectPool notePool2;       // 노트 풀 (2번 키, 3번 키)

    public GameObject measureLinePrefab;
    public GameObject notePrefab1;
    public GameObject notePrefab2;
    public Transform spawnArea;
    public Transform judgeLinePos;

    [SerializeField] GameObject songStartNote;

    public int lineCount = 5;          // 초기 경계선 개수
    public float baseScrollSpeed = 1f; // 기본 스크롤 속도
    //public float bpm = 120;            // BPM (비트 당 박자 수)
    public float beatsPerMeasure = 4;  // 마디당 박자 수 (4/4 박자 기준)
    public float noteOffset = 0.15f;   // 노트 시작 오프셋
    public float secondsPerBeat;      // 1비트에 걸리는 시간(초)
    public float measureDuration;     // 한 마디의 지속 시간(초)
    public float lineInterval;        // 마디 간의 간격
    public bool isMoving = false;   //노트 움직임 상태

    private float startPositionY = -1.38f; //노트 이미지에 따른 세부 위치 조절

    public int totalNotes; // 총 노트 개수



    public static NoteManager Instance { get; private set; }

    void Awake()
    {
        // 싱글톤 인스턴스 설정
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {

        //BMSData parsedData = BMSManager.Instance.ParseBMS("test5");
        //if (parsedData != null)
        //{
        //    int lastMeasure = parsedData.notes.Last().measure;
        //    lineCount = lastMeasure +5 ; //전체 마디 수
        //    secondsPerBeat = 60f / parsedData.header.bpm; //bpm
        //    measureDuration = secondsPerBeat * beatsPerMeasure;
        //    lineInterval = baseScrollSpeed * measureDuration;
        //    SpawnNotes(parsedData, spawnArea);
        //    SpawnMeasureLines();
        //}

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        UpdateMeasureLines();
    }

    public void SetNote(BMSData bms)
    {
        baseScrollSpeed = 5f;

        if (bms != null)
        {
            int lastMeasure = bms.notes.Last().measure;
            lineCount = lastMeasure + 5; //전체 마디 수
            secondsPerBeat = 60f / bms.header.bpm; //bpm
            measureDuration = secondsPerBeat * beatsPerMeasure;
            lineInterval = baseScrollSpeed * measureDuration;
            SpawnNotes(bms, spawnArea);
            SpawnMeasureLines();
        }
    }

    void SpawnMeasureLines()
    {
        for (int i = 0; i < lineCount; i++)
        {
            
            Vector3 spawnPosition = new Vector3(spawnArea.position.x, i * lineInterval - noteOffset+ startPositionY+ spawnArea.position.y, 0);

            GameObject measureLine = measureLinePool.GetObject();
            measureLine.transform.position = spawnPosition;
            measureLine.GetComponent<ScrollDown>().SetScrollSpeed(baseScrollSpeed);
            measureLine.SetActive(true);
            ////// 경계선 생성
            //GameObject measureLine = Instantiate(measureLinePrefab, spawnPosition, Quaternion.identity, spawnArea);

            //// 로컬 위치 설정 (필요할 경우)
            //measureLine.transform.localPosition = new Vector3(0, i * lineinterval- noteOffset, 0);
        }
    }

    public void SpawnNotes(BMSData bmsData, Transform spawnArea)
    {
        totalNotes = 0; // 총 노트 개수 초기화
        foreach (var noteData in bmsData.notes)
        {
            string data = noteData.noteString;
            int divisions = data.Length / 2; // 마디를 몇 등분할지 계산 (2자리씩 노트)

            for (int i = 0; i < divisions; i++)
            {
                string noteID = data.Substring(i * 2, 2);

                if (noteID != "00") // 노트가 있는 경우만 처리
                {
                   
                    float beatPosition = (float)i / divisions;
                    float yPosition = noteData.measure * lineInterval + +(lineInterval / divisions) * i + startPositionY + spawnArea.position.y;

                    float xPosition = GetChannelPosition(noteData.channel);
                    if (noteID == "02")
                    {
                        // "02"일 때 프리팹 생성
                        GameObject specialNote = Instantiate(songStartNote, new Vector3(xPosition, yPosition-1f, 0), Quaternion.identity, spawnArea);
                        specialNote.GetComponent<ScrollDown>().SetScrollSpeed(baseScrollSpeed);
                        specialNote.SetActive(true);
                    }
                    else
                    {
                        totalNotes++; // 유효한 노트 수 증가
                        GameObject selectedNote = SelectNotePool(noteData.channel).GetObject();
                        selectedNote.GetComponent<ScrollDown>().SetScrollSpeed(baseScrollSpeed);
                        selectedNote.transform.position = new Vector3(xPosition, yPosition, 0);
                        selectedNote.SetActive(true);
                    }
                }
            }
        }
        Debug.Log($"Total Notes: {totalNotes}");
        if (GameManager.Instance != null)
        {
            GameManagerProvider.Instance.InitializeGame();
        }
        else if (KGB_GameManager_Multi.Instance != null)
        {
            KGB_GameManager_Multi.Instance.InitializeGame();
        }
        else
        {
            Debug.LogWarning("No GameManager instance found.");
        }
    }

    private float GetChannelPosition(int channel)
    {
        switch (channel)
        {
            case 11: return -2.25f + spawnArea.position.x; // 1번 키
            case 12: return -0.74f + spawnArea.position.x; // 2번 키
            case 13: return 0.75f + spawnArea.position.x;  // 3번 키
            case 14: return 2.25f + spawnArea.position.x;  // 4번 키
            default: return 0f + spawnArea.position.x;     // 기본 값
        }
    }
    private ObjectPool SelectNotePool(int channel)
    {
        // 채널에 따라 노트 풀 선택
        return (channel == 11 || channel == 14) ? notePool1 : notePool2;
    }

    private GameObject SelectPrefab(int channel)
    {
        // 채널에 따라 프리팹 선택
        if (channel == 11 || channel == 14) // 1번 키, 4번 키
        {
            return notePrefab1;
        }
        else if (channel == 12 || channel == 13) // 2번 키, 3번 키
        {
            return notePrefab2;
        }
        else
            return notePrefab1; // 기본 값
    }

    void UpdateMeasureLines()
    {

    }



}

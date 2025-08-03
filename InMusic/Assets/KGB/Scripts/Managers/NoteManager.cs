using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class NoteManager : MonoBehaviour
{
    public ObjectPool measureLinePool; // ��輱 Ǯ
    public ObjectPool notePool1;       // ��Ʈ Ǯ (1�� Ű, 4�� Ű)
    public ObjectPool notePool2;       // ��Ʈ Ǯ (2�� Ű, 3�� Ű)

    public GameObject measureLinePrefab;
    public GameObject notePrefab1;
    public GameObject notePrefab2;
    public Transform spawnArea;
    public Transform judgeLinePos;

    [SerializeField] GameObject songStartNote;

    public int lineCount = 5;          // �ʱ� ��輱 ����
    public float baseScrollSpeed = 1f; // �⺻ ��ũ�� �ӵ�
    //public float bpm = 120;            // BPM (��Ʈ �� ���� ��)
    public float beatsPerMeasure = 4;  // ����� ���� �� (4/4 ���� ����)
    public float noteOffset = 0.15f;   // ��Ʈ ���� ������
    public float secondsPerBeat;      // 1��Ʈ�� �ɸ��� �ð�(��)
    public float measureDuration;     // �� ������ ���� �ð�(��)
    public float lineInterval;        // ���� ���� ����
    public bool isMoving = false;   //��Ʈ ������ ����

    private float startPositionY = -1.38f; //��Ʈ �̹����� ���� ���� ��ġ ����

    public int totalNotes; // �� ��Ʈ ����



    public static NoteManager Instance { get; private set; }

    void Awake()
    {
        // �̱��� �ν��Ͻ� ����
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
        //    lineCount = lastMeasure +5 ; //��ü ���� ��
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
            lineCount = lastMeasure + 5; //��ü ���� ��
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
            ////// ��輱 ����
            //GameObject measureLine = Instantiate(measureLinePrefab, spawnPosition, Quaternion.identity, spawnArea);

            //// ���� ��ġ ���� (�ʿ��� ���)
            //measureLine.transform.localPosition = new Vector3(0, i * lineinterval- noteOffset, 0);
        }
    }

    public void SpawnNotes(BMSData bmsData, Transform spawnArea)
    {
        totalNotes = 0; // �� ��Ʈ ���� �ʱ�ȭ
        foreach (var noteData in bmsData.notes)
        {
            string data = noteData.noteString;
            int divisions = data.Length / 2; // ���� �� ������� ��� (2�ڸ��� ��Ʈ)

            for (int i = 0; i < divisions; i++)
            {
                string noteID = data.Substring(i * 2, 2);

                if (noteID != "00") // ��Ʈ�� �ִ� ��츸 ó��
                {
                   
                    float beatPosition = (float)i / divisions;
                    float yPosition = noteData.measure * lineInterval + +(lineInterval / divisions) * i + startPositionY + spawnArea.position.y;

                    float xPosition = GetChannelPosition(noteData.channel);
                    if (noteID == "02")
                    {
                        // "02"�� �� ������ ����
                        GameObject specialNote = Instantiate(songStartNote, new Vector3(xPosition, yPosition-1f, 0), Quaternion.identity, spawnArea);
                        specialNote.GetComponent<ScrollDown>().SetScrollSpeed(baseScrollSpeed);
                        specialNote.SetActive(true);
                    }
                    else
                    {
                        totalNotes++; // ��ȿ�� ��Ʈ �� ����
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
            case 11: return -2.25f + spawnArea.position.x; // 1�� Ű
            case 12: return -0.74f + spawnArea.position.x; // 2�� Ű
            case 13: return 0.75f + spawnArea.position.x;  // 3�� Ű
            case 14: return 2.25f + spawnArea.position.x;  // 4�� Ű
            default: return 0f + spawnArea.position.x;     // �⺻ ��
        }
    }
    private ObjectPool SelectNotePool(int channel)
    {
        // ä�ο� ���� ��Ʈ Ǯ ����
        return (channel == 11 || channel == 14) ? notePool1 : notePool2;
    }

    private GameObject SelectPrefab(int channel)
    {
        // ä�ο� ���� ������ ����
        if (channel == 11 || channel == 14) // 1�� Ű, 4�� Ű
        {
            return notePrefab1;
        }
        else if (channel == 12 || channel == 13) // 2�� Ű, 3�� Ű
        {
            return notePrefab2;
        }
        else
            return notePrefab1; // �⺻ ��
    }

    void UpdateMeasureLines()
    {

    }



}

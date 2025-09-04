using ExitGames.Client.Photon.StructWrapping;
using Fusion;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Play
{
    [System.Serializable]
    public class NoteData
    {
        public int bar;
        public int channel;
        public List<int> noteData;
    }

    public class TimelineController : Singleton<TimelineController>
    {
        private INoteSpawn[] noteSpawners;
        private IMeasureLineSpawn[] measureLineSpawners;

        [SerializeField] private Transform[] judgementLines;

        // 시작 체크
        bool isStart = false;

        // 노트 및 박자선 생성에 필요한 변수
        private float bpm = 92f;
        private float beatIntervalMs;
        private float measureInterval;
        private float travelTime;
        private int noteCount;
        public int NoteCount { get { return noteCount; } private set { } }
        private float speed = 5f;
        private float startDelayBeats = 2f;
        private float beatDelayTime;
        public float BeatDelayTime { get { return beatDelayTime; } private set { } }
        private float songStartTime = 0f;
        public float SongStartTime { get { return songStartTime; } private set { } }
        private float nextSample;
        private int underBeats = 4;
        private int upperBeats = 1;

        // 노트 소환 리스트 코루틴
        private List<Coroutine> coroutines = new List<Coroutine>();

        // 노트 데이터 리스트 및 생성된 리스트
        private List<NoteData> noteDataList;

        public void Initialize(SongInfo songInfo)
        {
            noteSpawners = gameObject.GetComponents<INoteSpawn>();
            measureLineSpawners = gameObject.GetComponents<IMeasureLineSpawn>();
            Transform lineSpawnPoint = measureLineSpawners[0].GetLineSpawnPoint();

            // 곡 정보 받아오기
            noteDataList = songInfo.NoteList;
            noteCount = songInfo.NoteCount;

            // 비트 및 마디 계산
            bpm = songInfo.BPM;
            beatIntervalMs = 60000f / bpm;
            measureInterval = beatIntervalMs * 4f;

            // 마디 간격을 초 단위로 변환
            float measureIntervalSec = measureInterval / 1000f;

            // 내려오는데 걸리는 시간
            float distanceToJudgementLine = lineSpawnPoint.position.y - judgementLines[0].position.y;
            travelTime = distanceToJudgementLine / speed;

            // 시작 시간 계산(2마디 후 노래 시작)
            beatDelayTime = measureIntervalSec * startDelayBeats;
            songStartTime = Time.time + beatDelayTime;

            nextSample = songStartTime + measureIntervalSec;
            Debug.Log($"nextSample : {nextSample}, time : {Time.time}, TimellinePosition : {SoundManager.Instance.GetTimelinePosition()}");

            isStart = true;

            StartCoroutine(SpawnRoutine());
        }

        private void Update()
        {
            if (!isStart)
                return;

            // 곡 진행시간 * 주파수가 다음 라인보다 크거나 같으면 작동.
            if (Time.time >= nextSample)
            {
                StartCoroutine(PlayTicks());
            }
        }

        private IEnumerator PlayTicks()
        {
            nextSample += beatIntervalMs / 1000f;

            if (upperBeats == 1)
            {
                float measureStartTime = Time.time + measureInterval / 1000;

                float unitySpawnDelay = measureStartTime - travelTime - Time.time;

                foreach (IMeasureLineSpawn measureLineSpawn in measureLineSpawners)
                {
                    if (measureLineSpawn == null) continue;

                    coroutines.Add(StartCoroutine(measureLineSpawn.SpawnMeasureLine(speed, unitySpawnDelay, judgementLines[0].position.y)));
                }
            }

            upperBeats++;
            if (upperBeats > underBeats)
                upperBeats = 1;

            yield return null;
        }

        private IEnumerator SpawnRoutine()
        {
            int noteId = 0;
            foreach (NoteData noteData in noteDataList)
            {
                float barTime = noteData.bar * (measureInterval / 1000);
                int divisions = noteData.noteData.Count;

                for (int i = 0; i < divisions; i++)
                {
                    int noteValue = noteData.noteData[i];
                    if (noteValue == 0) continue;

                    int channel = noteData.channel;
                    float noteAppearTime = barTime + ((measureInterval / divisions) / 1000) * i;
                    float spawnTime = songStartTime + noteAppearTime - travelTime;

                    // foreach (INoteSpawn noteSpawner in noteSpawners)
                    // {
                    //     if (noteSpawner == null) continue;

                    //     coroutines.Add(StartCoroutine(noteSpawner.SpawnNote(noteId, channel, spawnTime, speed, noteCount, travelTime)));
                    // }
                    for (int j = 0; j < noteSpawners.Length; j++)
                    {
                        INoteSpawn noteSpawner = noteSpawners[j];
                        if (noteSpawner == null) continue;

                        coroutines.Add(StartCoroutine(noteSpawner.SpawnNote(noteId, channel, spawnTime, speed, noteCount, travelTime, j == 1)));
                    }
                    noteId++;
                }

                yield return null;
            }
        }

        public Note GetClosestNote(int channel, float pressTime)
        {
            return noteSpawners[0].GetClosestNote(channel, pressTime);
        }

        public Note GetClosestNoteById(int noteId)
        {
            return noteSpawners[1].GetClosestNoteById(noteId);
        }

        public void RemoveNote(Note note, int isMatch = 0)
        {
            noteSpawners[isMatch].RemoveNote(note);
        }

        public void RemoveLine(GameObject line, int isMatch = 0)
        {
            measureLineSpawners[isMatch].RemoveLine(line);
        }

        public void StopAllSpawnCoroutines()
        {
            foreach (Coroutine coroutine in coroutines)
            {
                if (coroutine != null)
                {
                    StopCoroutine(coroutine);
                }
            }
            coroutines.Clear();
            isStart = false;
        }

        public void ClearAll()
        {
            foreach (IMeasureLineSpawn measureLineSpawner in measureLineSpawners)
            {
                if (measureLineSpawner != null)
                {
                    measureLineSpawner.ClearAll();
                }
            }

            foreach (INoteSpawn noteSpawner in noteSpawners)
            {
                if (noteSpawner != null)
                {
                    noteSpawner.ClearAll();
                }
            }
        }
        
        public void RemoveJudgementLine()
        {
            foreach (Transform judgementLine in judgementLines)
            {
                judgementLine.gameObject.SetActive(false);
            }
        }
    }
}
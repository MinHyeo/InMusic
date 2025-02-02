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
        public float spawnTime;
        public List<int> noteData;

        public NoteData(int bar, int channel, float spawnTime, List<int> noteData)
        {
            this.bar = bar;
            this.channel = channel;
            this.spawnTime = spawnTime;
            this.noteData = noteData;
        }

        public override string ToString()
        {
            return string.Format("Bar: {0}, Channel: {1}, NoteData: {2}", bar, channel, noteData);
        }
    }

    public class NoteManager : SingleTon<NoteManager>
    {
        [Header("Prefabs")]
        [SerializeField]
        private GameObject note1Prefab;
        [SerializeField]
        private GameObject note2Prefab;

        [Header("Note datas")]
        [SerializeField]
        private Transform[] noteSpawnPoints; // 각 채널별 노트 생성 위치
        [SerializeField]
        private float noteSpeed;
        [SerializeField]
        private Transform judgementLine;

        private int noteCount;
        //public List<Note> NoteList = new List<Note>();
        private List<NoteData> noteList;
        private float songStartTime;

        private float measureInterval;  // 한 마디 간격 (4/4박자 기준)
        private float travelTime;       // 판정선까지 도달하는 시간

        public void InitializeNotes(SongInfo songInfo)
        {
            //노트 오브젝트풀 생성
            ObjectPoolManager.Instance.CreatePool(note1Prefab);
            ObjectPoolManager.Instance.CreatePool(note2Prefab);

            // 노트 데이터 초기화
            noteCount = songInfo.NoteCount;
            noteList.Clear();
            SetNoteList(songInfo.NoteList);

            // 노트 생성 시 음악과의 타이밍 맞추기 위해 초기화
            songStartTime = Time.time + Metronome.Instance.preStartDelay; // 노래 시작 시간 설정

            //StartCoroutine(SpawnNotes());
        }

        private void SetNoteList(List<NoteData> noteDataList)
        {
            //노트데이터를 받아와서 소환타이밍 계산
            foreach(NoteData noteData in noteDataList)
            {
                int bar = noteData.bar;
                int channel = noteData.channel;

                float barTime = bar * measureInterval;      //마디 시작 시간
                int divisions = noteData.noteData.Count;    //마디의 분할 수

                for(int i = 0; i < divisions; i++)
                {
                    //노트가 있는 경우
                    int noteValue = noteData.noteData[i];
                    if(noteValue != 0)
                    {
                        //노트 생성 타이밍 계산
                        float noteAppearTime = barTime + (measureInterval / divisions) * i;
                        //실제 소환 시점을 결정하기 위해 이동 시간을 고려
                        float spawnTime = songStartTime + noteAppearTime - travelTime;

                        noteList.Add(new NoteData(bar, channel, spawnTime, null));
                    }
                }
            }

            noteList.Sort((a, b) => a.spawnTime.CompareTo(b.spawnTime));
        }

        public void SetTimingInfo(float measureInterval, float travelTime)
        {
            this.measureInterval = measureInterval;
            this.travelTime = travelTime;
        }

        private void Update()
        {
            SpawnNote();
        }

        private void SpawnNote()
        {
            if(noteList.Count > 0)
            {
                var nextNote = noteList[0];

                if (nextNote.spawnTime <= Time.time)
                {
                    noteList.RemoveAt(0);

                    var note = GetNote(nextNote);

                    note.transform.position = noteSpawnPoints[nextNote.channel - 11].position;

                }
            }
        }

        private GameObject GetNote(NoteData nextNote)
        {
            //오브젝트풀 객체 Get
            GameObject note = null;
            switch (nextNote.channel)
            {
                case 11:
                case 14:
                    note = ObjectPoolManager.Instance.GetFromPool("note1Prefab");
                    break;
                case 12:
                case 13:
                    note = ObjectPoolManager.Instance.GetFromPool("note2Prefab");
                    break;
            }
            //객체에 정보 전달
            note.GetComponent<Note>().Initialize(nextNote.channel, noteSpeed, 1000000 / noteCount, travelTime);

            return note;
        }

        private float GetSpawnTime(NoteData nextNote)
        {
            float barTime = nextNote.bar * measureInterval;
            int divisions = nextNote.noteData.Count;

            for(int i = 0; i < divisions; i++)
            {
                int noteValue = nextNote.noteData[i];
                if(noteValue != 0)
                {
                    float noteAppearTime = barTime + (measureInterval / divisions) * i;
                    float spawnTime = songStartTime + noteAppearTime - travelTime;
                    return spawnTime;
                }
            }

            return 0.0f;
        }

        //private IEnumerator SpawnNotes()
        //{
        //    foreach (NoteData noteData in noteQueue)
        //    {
        //        // 각 마디(bar)에 대한 시작 시간을 계산
        //        float barTime = GetBarTime(noteData.bar);  // 마디(bar) 번호에 따른 시간 계산

        //        int divisions = noteData.noteData.Count;  // 해당 마디의 나누어진 개수 (노트의 분할 수)

        //        for (int i = 0; i < divisions; i++)
        //        {
        //            int noteValue = noteData.noteData[i];

        //            if (noteValue != 0)  // 값이 0이 아니면 노트를 생성할 필요가 있음
        //            {
        //                int channel = noteData.channel;

        //                // 노트 생성 타이밍 계산
        //                float noteAppearTime = barTime + (measureInterval / divisions) * i;

        //                // 실제 소환 시점을 결정하기 위해 이동 시간을 고려
        //                float spawnTime = songStartTime + noteAppearTime - travelTime;

        //                // 노트 생성 예약 (코루틴 실행)
        //                StartCoroutine(SpawnNoteCoroutine(channel, spawnTime));
        //            }
        //        }

        //        yield return null;  // 각 마디를 처리한 후 한 프레임 대기
        //    }
        //}

        //private float GetBarTime(int barNumber)
        //{
        //    // 각 마디의 시간을 계산하는 함수
        //    return barNumber * measureInterval;
        //}

        //private IEnumerator SpawnNoteCoroutine(int channel, float spawnTime)
        //{
        //    // spawnTime까지 대기
        //    yield return new WaitForSeconds(spawnTime - Time.time);

        //    // 노트 생성
        //    var note = GetNote(channel);
        //    note.transform.position = noteSpawnPoints[channel - 11].position;
        //    //GameObject note = Instantiate(notePrefab, noteSpawnPoints[channel - 11].position, Quaternion.identity);
        //    Note noteScript = note.GetComponent<Note>();

        //    NoteList.Add(noteScript);
        //    noteScript.Initialize(channel, noteSpeed, travelTime, 1000000 / noteCount);
        //}

        public void Restart()
        {
            StopCoroutine("SpawnNoteCoroutine");
            foreach (Note note in NoteList)
            {
                ObjectPoolManager.Instance.ReleaseToPool(note.gameObject.name, note.gameObject);
            }
        }

        public void RemoveNoteFromActiveList(Note note)
        {
            if (NoteList.Contains(note))
            {
                NoteList.Remove(note);
                ObjectPoolManager.Instance.ReleaseToPool(note.gameObject.name, note.gameObject);
            }
        }

        public void RemoveJudgementLine()
        {
            judgementLine.gameObject.SetActive(false);
        }
    }
}
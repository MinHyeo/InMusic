using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Play 
{
    [System.Serializable]
    public class NoteData
    {
        public int bar;
        public int channel;
        public int noteCount;
        public List<int> noteData;

        public override string ToString()
        {
            return string.Format("Bar: {0}, Channel: {1}, NoteData: {2}", bar, channel, noteData);
        }
    }

    public class NoteManager : SingleTon<NoteManager>
    {
        [SerializeField]
        private GameObject notePrefab;
        [SerializeField]
        private Transform[] noteSpawnPoints; // 각 채널별 노트 생성 위치
        [SerializeField]
        private float noteSpeed;
        [SerializeField]
        private Transform judgementLine;

        private int noteCount;
        public List<Note> NoteList = new List<Note>();
        private List<NoteData> noteDataList;
        private float songStartTime;

        private float measureInterval;  // 한 마디 간격 (4/4박자 기준)
        private float travelTime;       // 판정선까지 도달하는 시간

        public void InitializeNotes(SongInfo songInfo)
        {
            noteDataList = songInfo.NoteList;
            noteCount = songInfo.NoteCount;

            // 노트 생성 시 음악과의 타이밍 맞추기 위해 초기화
            songStartTime = Time.time + Metronome.Instance.preStartDelay; // 노래 시작 시간 설정

            StartCoroutine(SpawnNotes());
        }

        public void SetTimingInfo(float measureInterval, float travelTime)
        {
            this.measureInterval = measureInterval;
            this.travelTime = travelTime;
        }

        private IEnumerator SpawnNotes()
        {
            foreach (NoteData noteData in noteDataList)
            {
                // 각 마디(bar)에 대한 시작 시간을 계산
                float barTime = GetBarTime(noteData.bar);  // 마디(bar) 번호에 따른 시간 계산

                int divisions = noteData.noteData.Count;  // 해당 마디의 나누어진 개수 (노트의 분할 수)

                for (int i = 0; i < divisions; i++)
                {
                    int noteValue = noteData.noteData[i];

                    if (noteValue != 0)  // 값이 0이 아니면 노트를 생성할 필요가 있음
                    {
                        int channel = noteData.channel;

                        // 노트 생성 타이밍 계산
                        float noteAppearTime = barTime + (measureInterval / divisions) * i;

                        // 실제 소환 시점을 결정하기 위해 이동 시간을 고려
                        float spawnTime = songStartTime + noteAppearTime - travelTime;

                        // 노트 생성 예약 (코루틴 실행)
                        StartCoroutine(SpawnNoteCoroutine(channel, spawnTime));
                    }
                }

                yield return null;  // 각 마디를 처리한 후 한 프레임 대기
            }
        }

        private float GetBarTime(int barNumber)
        {
            // 각 마디의 시간을 계산하는 함수
            return barNumber * measureInterval;
        }

        private IEnumerator SpawnNoteCoroutine(int channel, float spawnTime)
        {
            // spawnTime까지 대기
            yield return new WaitForSeconds(spawnTime - Time.time);

            // 노트 생성
            //var note = ObjectPool.Instance.Pool.Get();
            GameObject note = Instantiate(notePrefab, noteSpawnPoints[channel - 11].position, Quaternion.identity);
            Note noteScript = note.GetComponent<Note>();

            NoteList.Add(noteScript);
            noteScript.Initialize(channel, noteSpeed, travelTime, 1000000 / noteCount);
        }

        public void RemoveNoteFromActiveList(Note note)
        {
            if (NoteList.Contains(note))
            {
                NoteList.Remove(note);
            }
        }

        public void RemoveJudgementLine()
        {
            judgementLine.gameObject.SetActive(false);
        }
    }
}
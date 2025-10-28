using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Play
{
    public class NoteSpawner : MonoBehaviour, INoteSpawn
    {
        [SerializeField] private GameObject note1Prefab;
        [SerializeField] private GameObject note2Prefab;
        [SerializeField] private Transform[] noteSpawnPoints;

        private List<Note> activeNotes = new List<Note>();

        private void Start()
        {
            // 초기화 작업
            ObjectPoolManager.Instance.CreatePool(note1Prefab);
            ObjectPoolManager.Instance.CreatePool(note2Prefab);
        }

        public IEnumerator SpawnNote(int noteId, int channel, float spawnTime, float speed, int noteCount, float travelTime, bool isMatch = false)
        {
            yield return new WaitForSeconds(spawnTime - Time.time);

            GameObject prefab = (channel == 11 || channel == 14) ? note1Prefab : note2Prefab;
            GameObject note = ObjectPoolManager.Instance.GetFromPool(prefab.name);
            note.transform.position = noteSpawnPoints[channel - 11].position;

            Note noteScript = note.GetComponent<Note>();
            noteScript.Initialize(noteId, channel, speed, 1000000 / noteCount, travelTime, isMatch);
            activeNotes.Add(noteScript);
        }

        public Note GetClosestNote(int channel, float pressTime)
        {
            Note closestNote = null;
            float minTimeDifference = float.MaxValue;

            foreach (Note note in activeNotes)
            {
                if (note.Channel == channel)
                {
                    float timeDifference = Mathf.Abs(note.TargetTime - pressTime);
                    if (timeDifference < minTimeDifference)
                    {
                        minTimeDifference = timeDifference;
                        closestNote = note;
                    }
                }
            }

            return closestNote;
        }

        public Note GetClosestNoteById(int noteId)
        {
            foreach (Note note in activeNotes)
            {
                if (note.NoteId == noteId)
                {
                    return note;
                }
            }
            return null;
        }

        public void RemoveNote(Note note)
        {
            if (activeNotes.Contains(note))
            {
                activeNotes.Remove(note);
                ObjectPoolManager.Instance.ReleaseToPool(note.gameObject.name, note.gameObject);
            }
        }

        public void ClearAll()
        {
            foreach (var note in activeNotes)
                ObjectPoolManager.Instance.ReleaseToPool(note.gameObject.name, note.gameObject);
            activeNotes.Clear();
        }
    }
}
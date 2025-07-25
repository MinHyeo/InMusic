using System.Collections;

namespace Play
{
    public interface INoteSpawn
    {
        IEnumerator SpawnNote(int noteId, int channel, float spawnTime, float speed, int noteCount, float travelTime);
        Note GetClosestNote(int channel, float pressTime);
        Note GetClosestNoteById(int noteId);
        void RemoveNote(Note note);
        void ClearAll();
    }
}

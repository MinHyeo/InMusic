using UnityEngine;

namespace Play
{
    public interface INote
    {
        public int NoteId { get; }
        public int Channel { get; }
        public float TargetTime { get; }

        public float Hit(int accuracy);
        public void Initialize(int noteId, int channel, float speed, float beatIntervalMs, float travelTime, bool isMatch = false);
    }
}
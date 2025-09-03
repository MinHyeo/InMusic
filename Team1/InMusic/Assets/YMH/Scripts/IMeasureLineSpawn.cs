using UnityEngine;
using System.Collections;

namespace Play
{
    interface IMeasureLineSpawn
    {
        IEnumerator SpawnMeasureLine(float speed, float delaySeconds, float judgementLineY);
        Transform GetLineSpawnPoint();
        void RemoveLine(GameObject line);
        void ClearAll();
    }
}
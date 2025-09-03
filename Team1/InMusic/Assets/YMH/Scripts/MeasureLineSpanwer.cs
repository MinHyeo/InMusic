using UnityEngine;
using System.Collections.Generic;
using System.Collections;

namespace Play
{
    public class MeasureLineSpawner : MonoBehaviour, IMeasureLineSpawn
    {
        [SerializeField] private GameObject linePrefab;
        [SerializeField] private Transform lineSpawnPoint;

        private List<GameObject> linesObject = new List<GameObject>();

        private void Start()
        {
            // 오브젝트 풀 생성
            ObjectPoolManager.Instance.CreatePool(linePrefab);
        }

        public IEnumerator SpawnMeasureLine(float speed, float delaySeconds, float judgementLineY)
        {
            yield return new WaitForSeconds(delaySeconds);

            GameObject newLine = ObjectPoolManager.Instance.GetFromPool("Line");
            newLine.transform.position = lineSpawnPoint.position;
            newLine.GetComponent<Line>().Initialize(speed, judgementLineY);
            linesObject.Add(newLine);
        }

        public Transform GetLineSpawnPoint()
        {
            return lineSpawnPoint;
        }

        public void RemoveLine(GameObject line)
        {
            if (linesObject.Contains(line))
            {
                linesObject.Remove(line);
                ObjectPoolManager.Instance.ReleaseToPool("Line", line);
            }
        }

        public void ClearAll()
        {
            foreach (var line in linesObject)
                ObjectPoolManager.Instance.ReleaseToPool("Line", line);
            linesObject.Clear();
        }
    }
}
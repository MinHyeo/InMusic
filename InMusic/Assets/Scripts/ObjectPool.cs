using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public GameObject prefab; // 생성할 프리팹
    public int initialSize = 10; // 초기 생성 개수

    private Queue<GameObject> pool = new Queue<GameObject>();

    void Start()
    {
        // 초기 오브젝트 생성
        for (int i = 0; i < initialSize; i++)
        {
            GameObject obj = Instantiate(prefab);
            obj.transform.SetParent(transform);
            obj.SetActive(false);
            pool.Enqueue(obj);
        }
    }

    // 오브젝트 가져오기
    public GameObject GetObject()
    {
        if (pool.Count > 0)
        {
            GameObject obj = pool.Dequeue();
            obj.SetActive(true);
            return obj;
        }
        else
        {
            // 풀에 오브젝트가 없으면 새로 생성
            GameObject obj = Instantiate(prefab);
            obj.transform.SetParent(transform);
            obj.SetActive(true);
            return obj;
        }
    }

    // 오브젝트 반환
    public void ReturnObject(GameObject obj)
    {
        obj.SetActive(false);
        obj.transform.SetParent(transform);
        pool.Enqueue(obj);
    }
}

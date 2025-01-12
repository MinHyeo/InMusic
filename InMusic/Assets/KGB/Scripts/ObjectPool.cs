using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public GameObject prefab; // ������ ������
    public int initialSize = 10; // �ʱ� ���� ����

    private Queue<GameObject> pool = new Queue<GameObject>();

    void Start()
    {
        // �ʱ� ������Ʈ ����
        for (int i = 0; i < initialSize; i++)
        {
            GameObject obj = Instantiate(prefab);
            obj.transform.SetParent(transform);
            obj.SetActive(false);
            pool.Enqueue(obj);
        }
    }

    // ������Ʈ ��������
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
            // Ǯ�� ������Ʈ�� ������ ���� ����
            GameObject obj = Instantiate(prefab);
            obj.transform.SetParent(transform);
            obj.SetActive(true);
            return obj;
        }
    }

    // ������Ʈ ��ȯ
    public void ReturnObject(GameObject obj)
    {
        obj.SetActive(false);
        obj.transform.SetParent(transform);
        pool.Enqueue(obj);
    }
}

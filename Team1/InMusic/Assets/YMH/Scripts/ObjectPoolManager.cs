using Play;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ObjectPoolManager : Singleton<ObjectPoolManager>
{
    private int defaultCapacity = 10;
    private int maxPoolSize = 20;

    // Ǯ�� �����ϴ� ��ųʸ� (key: prefab �̸�, value: IObjectPool<GameObject>)
    private Dictionary<string, IObjectPool<GameObject>> pools = new Dictionary<string, IObjectPool<GameObject>>();

    /// <summary>
    /// Ư�� �����տ� ���� ������Ʈ Ǯ ���� �� �ʱ�ȭ
    /// </summary>
    /// <param name="prefab">Ǯ���� ������</param>
    public void CreatePool(GameObject prefab)
    {
        string key = prefab.name;

        if (pools.ContainsKey(key))
        {
            Debug.LogWarning($"Pool for {key} already exists.");
            return;
        }

        // ���ο� Ǯ ����
        pools[key] = new ObjectPool<GameObject>(
            () => CreatePooledItem(prefab), // ������Ʈ ���� ����
            OnTakeFromPool, // Ǯ���� ������ �� ȣ��
            OnReturnedToPool, // Ǯ�� ��ȯ�� �� ȣ��
            OnDestroyPoolObject, // ������Ʈ ���� ����
            true, // �ݹ� ȣ�� ����
            defaultCapacity, // �ʱ� ���� ����
            maxPoolSize // �ִ� Ǯ ũ��
        );

        // �̸� ������Ʈ ����
        for (int i = 0; i < defaultCapacity; i++)
        {
            GameObject pooledItem = CreatePooledItem(prefab);
            pools[key].Release(pooledItem);
        }
    }

    /// <summary>
    /// Ǯ���� ������Ʈ ��������
    /// </summary>
    /// <param name="prefabName">������ �̸�</param>
    /// <returns>GameObject</returns>
    public GameObject GetFromPool(string prefabName)
    {
        if (pools.ContainsKey(prefabName))
        {
            return pools[prefabName].Get();
        }

        Debug.LogError($"No pool found for prefab: {prefabName}");
        return null;
    }

    /// <summary>
    /// ������Ʈ�� Ǯ�� ��ȯ
    /// </summary>
    /// <param name="prefabName">������ �̸�</param>
    /// <param name="objectToRelease">��ȯ�� ������Ʈ</param>
    public void ReleaseToPool(string prefabName, GameObject objectToRelease)
    {
        if (pools.ContainsKey(prefabName))
        {
            pools[prefabName].Release(objectToRelease);
        }
        else
        {
            Debug.LogError($"No pool found for prefab: {prefabName}");
        }
    }

    /// <summary>
    /// ������Ʈ ���� ����
    /// </summary>
    private GameObject CreatePooledItem(GameObject prefab)
    {
        GameObject poolGo = Instantiate(prefab);
        poolGo.name = prefab.name; // �̸��� �����ϰ� ����
        return poolGo;
    }

    private void OnTakeFromPool(GameObject poolGo)
    {
        poolGo.SetActive(true);
    }

    private void OnReturnedToPool(GameObject poolGo)
    {
        poolGo.SetActive(false);
    }

    private void OnDestroyPoolObject(GameObject poolGo)
    {
        Destroy(poolGo);
    }
}
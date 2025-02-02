using Play;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ObjectPoolManager : SingleTon<ObjectPoolManager>
{
    private int defaultCapacity = 10;
    private int maxPoolSize = 20;

    // 풀을 저장하는 딕셔너리 (key: prefab 이름, value: IObjectPool<GameObject>)
    private Dictionary<string, IObjectPool<GameObject>> pools = new Dictionary<string, IObjectPool<GameObject>>();

    /// <summary>
    /// 특정 프리팹에 대한 오브젝트 풀 생성 및 초기화
    /// </summary>
    /// <param name="prefab">풀링할 프리팹</param>
    public void CreatePool(GameObject prefab)
    {
        string key = prefab.name;

        if (pools.ContainsKey(key))
        {
            Debug.LogWarning($"Pool for {key} already exists.");
            return;
        }

        // 새로운 풀 생성
        pools[key] = new ObjectPool<GameObject>(
            () => CreatePooledItem(prefab), // 오브젝트 생성 로직
            OnTakeFromPool, // 풀에서 가져올 때 호출
            OnReturnedToPool, // 풀로 반환될 때 호출
            OnDestroyPoolObject, // 오브젝트 제거 로직
            true, // 콜백 호출 여부
            defaultCapacity, // 초기 생성 개수
            maxPoolSize // 최대 풀 크기
        );

        // 미리 오브젝트 생성
        for (int i = 0; i < defaultCapacity; i++)
        {
            GameObject pooledItem = CreatePooledItem(prefab);
            pools[key].Release(pooledItem);
        }
    }

    /// <summary>
    /// 풀에서 오브젝트 가져오기
    /// </summary>
    /// <param name="prefabName">프리팹 이름</param>
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
    /// 오브젝트를 풀로 반환
    /// </summary>
    /// <param name="prefabName">프리팹 이름</param>
    /// <param name="objectToRelease">반환할 오브젝트</param>
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
    /// 오브젝트 생성 로직
    /// </summary>
    private GameObject CreatePooledItem(GameObject prefab)
    {
        GameObject poolGo = Instantiate(prefab);
        poolGo.name = prefab.name; // 이름을 동일하게 설정
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
using UnityEngine;
using UnityEngine.Pool;

public class ObjectPool : SingleTon<ObjectPool>
{
    private int defaultCapacity = 10;
    private int maxPoolSize = 20;
    private GameObject notePrefab;

    public IObjectPool<GameObject> Pool { get; private set; }

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        Pool = new ObjectPool<GameObject>(CreatePooledItem, OnTakeFromPool, OnReturnedToPool,
        OnDestroyPoolObject, true, defaultCapacity, maxPoolSize);

        // 미리 오브젝트 생성 해놓기
        for (int i = 0; i < defaultCapacity; i++)
        {
            Play.Note note = CreatePooledItem().GetComponent<Play.Note>();
            note.Pool.Release(note.gameObject);
        }
    }

    private GameObject CreatePooledItem()
    {
        GameObject poolGo = Instantiate(notePrefab);
        poolGo.GetComponent<Play.Note>().Pool = Pool;
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

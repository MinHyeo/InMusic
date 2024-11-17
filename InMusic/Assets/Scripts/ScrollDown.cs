using UnityEngine;

public class ScrollDown : MonoBehaviour
{
    public float scrollSpeed = 5f;
    public float despawnYPosition = -2.1f; // 사라질 Y 위치

    public ObjectPool objectPool;
    void Start()
    {
        objectPool = GetComponentInParent<ObjectPool>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        MoveDown();
    }

    void MoveDown()
    {
        transform.Translate(Vector3.down * scrollSpeed * Time.deltaTime);

        if (transform.position.y < despawnYPosition)
        {
            objectPool.ReturnObject(gameObject);
        }
    }

    public void SetScrollSpeed(float speed)
    {
        scrollSpeed = speed;
    }
}

using UnityEngine;

public class ScrollDown : MonoBehaviour
{
    public float scrollSpeed = 5f;
    public float despawnYPosition = -2.0f; // 사라질 Y 위치
    public ObjectPool objectPool;

    public bool isMoving = false;

    private Note note;
    void Start()
    {
        objectPool = GetComponentInParent<ObjectPool>();
        scrollSpeed = NoteManager.Instance.baseScrollSpeed;
        note = GetComponent<Note>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (NoteManager.Instance.isMoving)
        {
            MoveDown();
        }
    }

    void MoveDown()
    {
        transform.Translate(Vector3.down * scrollSpeed * Time.deltaTime);

        if (transform.position.y < despawnYPosition)
        {

            if (note != null)
            {
                note.MissNote();
            }
            if(objectPool != null)
            {
                objectPool.ReturnObject(gameObject);
            }

        }
    }

    public void SetScrollSpeed(float speed)
    {
        scrollSpeed = speed;
    }
}

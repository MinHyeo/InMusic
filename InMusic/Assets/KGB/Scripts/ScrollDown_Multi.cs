using UnityEngine;

public class ScrollDown_Multi : MonoBehaviour
{
    public float scrollSpeed = 5f;
    public float despawnYPosition = -2.0f; // 사라질 Y 위치
    public ObjectPool objectPool;

    public bool isMoving = false;

    private bool isMultiplayer = false;

    private Note_Multi note;
    void Start()
    {
        objectPool = GetComponentInParent<ObjectPool>();
        scrollSpeed = NoteManager.Instance.baseScrollSpeed;
        note = GetComponent<Note_Multi>();

        isMultiplayer = (KGB_GameManager_Multi.Instance != null);
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        if (!IsGameActive())
        {
            return;
        }
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
            if (objectPool != null)
            {
                objectPool.ReturnObject(gameObject);
            }

        }
    }

    public void SetScrollSpeed(float speed)
    {
        scrollSpeed = speed;
    }

    bool IsGameActive()
    {
        return GameManagerProvider.Instance.IsGameActive;
    }
}

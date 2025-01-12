using UnityEngine;

public class SongStartPoint : MonoBehaviour
{
    Transform judgePos;
    void Start()
    {
        judgePos = NoteManager.Instance.judgeLinePos;
    }

    // Update is called once per frame
    void Update()
    {
        CheckPos();
    }

    void CheckPos()
    {
        if (transform.position.y <= judgePos.position.y)
        {
            GameManager.Instance.StartGame();
            gameObject.SetActive(false);
            Debug.Log("½ÃÀÛ");
        }
    }
}

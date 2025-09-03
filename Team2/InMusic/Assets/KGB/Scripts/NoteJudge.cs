using UnityEngine;
using UnityEngine.InputSystem;
using static Define;

public class NoteJudge : MonoBehaviour
{
    public BoxCollider2D[] lineColliders;


    private void Start()
    {
        // 키 입력 이벤트 구독
        Managers.Instance.Input.OnKeyPressed -= HandleKeyPress;
        Managers.Instance.Input.OnKeyPressed += HandleKeyPress;
    }
    private void OnDestroy()
    {
        // 구독 해제 
        if (Managers.Instance != null && Managers.Instance.Input != null)
            Managers.Instance.Input.OnKeyPressed -= HandleKeyPress;
    }
    private void HandleKeyPress(KeyCode key)
    {
        // 키가 라인에 해당되는지 판별
        for (int i = 0; i < 4; i++)
        {
            if (key == Managers.Instance.Key.GetKey((RhythmKey)i))
            {
                JudgeClosestNote(lineColliders[i]);
                break;
            }
        }
    }

    void JudgeClosestNote(BoxCollider2D lineCollider)
    {
        Collider2D[] notesInRange = Physics2D.OverlapBoxAll(
            lineCollider.bounds.center,
            lineCollider.bounds.size,
            0f
        );

        Note closestNote = null;
        float closestY = float.MaxValue;

        foreach (Collider2D col in notesInRange)
        {
            Note note = col.GetComponent<Note>();
            if (note && note.transform.position.y < closestY)
            {
                closestNote = note;
                closestY = note.transform.position.y;
            }
        }

        if (closestNote != null)
        {
            closestNote.JudgmentNote();
            closestNote.gameObject.SetActive(false);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        foreach (var col in lineColliders)
            Gizmos.DrawWireCube(col.bounds.center, col.bounds.size);
    }
}

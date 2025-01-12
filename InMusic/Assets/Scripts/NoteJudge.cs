using UnityEngine;

public class NoteJudge : MonoBehaviour
{
    public BoxCollider2D[] lineColliders; // 4개의 라인별 콜라이더 (D, F, J, K에 대응)
    public KeyCode[] keys = { KeyCode.D, KeyCode.F, KeyCode.J, KeyCode.K }; // 각 라인에 대응하는 키

    void Update()
    {
        // 각 키에 대해 판정 수행
        for (int i = 0; i < keys.Length; i++)
        {
            if (Input.GetKeyDown(keys[i]))
            {
                JudgeClosestNote(lineColliders[i]); // 해당 라인의 콜라이더로 판정
            }
        }
    }

    void JudgeClosestNote(BoxCollider2D lineCollider)
    {
        // 지정된 콜라이더 안의 모든 노트를 감지
        Collider2D[] notesInRange = Physics2D.OverlapBoxAll(
            lineCollider.bounds.center,
            lineCollider.bounds.size,
            0f
        );

        Note closestNote = null;
        float closestY = float.MaxValue;

        // 감지된 노트 중 가장 Y값이 낮은 노트 찾기
        foreach (Collider2D collider in notesInRange)
        {
            Note note = collider.GetComponent<Note>();
            if (note != null)
            {
                if (note.transform.position.y < closestY)
                {
                    closestNote = note;
                    closestY = note.transform.position.y;
                }
            }
        }

        // 가장 가까운 노트만 판정 수행
        if (closestNote != null)
        {
            closestNote.JudgmentNote(); // 판정 수행
            closestNote.gameObject.SetActive(false); // 판정 후 비활성화
        }
        else
        {
            Debug.Log("No note found in range.");
        }
    }

    // 디버그용: 모든 박스 콜라이더 영역을 Scene 창에 시각적으로 표시
    void OnDrawGizmos()
    {
        if (lineColliders != null)
        {
            Gizmos.color = Color.red;
            foreach (BoxCollider2D lineCollider in lineColliders)
            {
                Gizmos.DrawWireCube(lineCollider.bounds.center, lineCollider.bounds.size);
            }
        }
    }
}

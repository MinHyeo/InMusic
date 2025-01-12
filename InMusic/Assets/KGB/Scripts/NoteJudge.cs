using UnityEngine;

public class NoteJudge : MonoBehaviour
{
    public BoxCollider2D[] lineColliders; // 4���� ���κ� �ݶ��̴� (D, F, J, K�� ����)
    public KeyCode[] keys = { KeyCode.D, KeyCode.F, KeyCode.J, KeyCode.K }; // �� ���ο� �����ϴ� Ű

    void Update()
    {
        // �� Ű�� ���� ���� ����
        for (int i = 0; i < keys.Length; i++)
        {
            if (Input.GetKeyDown(keys[i]))
            {
                JudgeClosestNote(lineColliders[i]); // �ش� ������ �ݶ��̴��� ����
            }
        }
    }

    void JudgeClosestNote(BoxCollider2D lineCollider)
    {
        // ������ �ݶ��̴� ���� ��� ��Ʈ�� ����
        Collider2D[] notesInRange = Physics2D.OverlapBoxAll(
            lineCollider.bounds.center,
            lineCollider.bounds.size,
            0f
        );

        Note closestNote = null;
        float closestY = float.MaxValue;

        // ������ ��Ʈ �� ���� Y���� ���� ��Ʈ ã��
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

        // ���� ����� ��Ʈ�� ���� ����
        if (closestNote != null)
        {
            closestNote.JudgmentNote(); // ���� ����
            closestNote.gameObject.SetActive(false); // ���� �� ��Ȱ��ȭ
        }
        else
        {
            Debug.Log("No note found in range.");
        }
    }

    // ����׿�: ��� �ڽ� �ݶ��̴� ������ Scene â�� �ð������� ǥ��
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

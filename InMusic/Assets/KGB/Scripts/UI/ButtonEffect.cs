using UnityEngine;
using UnityEngine.UI;

public class ButtonEffect : MonoBehaviour
{
    public Image[] buttonImages; // ��ư �̹��� �迭 (D, F, J, K ����)
    public Sprite[] pressedSprites; // Ű�� ������ ���� �̹���
    public Sprite[] defaultSprites; // �⺻ �̹���
    public GameObject[] lightEffects; // �� ȿ�� ������Ʈ �迭 (D, F, J, K ����)

    private KeyCode[] keys = { KeyCode.D, KeyCode.F, KeyCode.J, KeyCode.K }; // Ű �迭

    void Update()
    {
        // �� Ű�� ���� ���� Ȯ��
        for (int i = 0; i < keys.Length; i++)
        {
            if (Input.GetKeyDown(keys[i]))
            {
                OnKeyDown(i); // Ű�� ������ ���� ȿ��
            }
            if (Input.GetKeyUp(keys[i]))
            {
                OnKeyUp(i); // Ű�� ���� ���� ȿ��
            }
        }
    }

    void OnKeyDown(int index)
    {
        // ��ư �̹��� ����
        buttonImages[index].sprite = pressedSprites[index];

        // �� ȿ�� Ȱ��ȭ
        lightEffects[index].SetActive(true);
    }

    void OnKeyUp(int index)
    {
        // ��ư �̹����� �⺻ �̹����� ����
        buttonImages[index].sprite = defaultSprites[index];

        // �� ȿ�� ��Ȱ��ȭ
        lightEffects[index].SetActive(false);
    }
}

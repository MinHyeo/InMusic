using UnityEngine;
using UnityEngine.UI;
using static Define;

public class ButtonEffect : MonoBehaviour
{
    public Image[] buttonImages; // ��ư �̹��� �迭 (Key1 ~ Key4 ����)
    public Sprite[] pressedSprites; // Ű�� ������ ���� �̹���
    public Sprite[] defaultSprites; // �⺻ �̹���
    public GameObject[] lightEffects; // �� ȿ�� ������Ʈ �迭 (Key1 ~ Key4 ����)

    private KeyCode[] keyBindings = new KeyCode[4];

    void Start()
    {
        UpdateKeyBindings();
        Managers.Instance.Input.OnKeyPressed -= HandleKeyPressed;
        Managers.Instance.Input.OnKeyPressed += HandleKeyPressed;

    }
    void OnDestroy()
    {
        // �̺�Ʈ ���� (�߿�!)
        Managers.Instance.Input.OnKeyPressed -= HandleKeyPressed;
    }
    private void HandleKeyPressed(KeyCode key)
    {
        Debug.Log($"Key Pressed: {key}");
        for (int i = 0; i < keyBindings.Length; i++)
        {
            if (key == Managers.Instance.Key.GetKey((RhythmKey)i))
            {
                OnKeyDown(i);
            }
        }
    }

    void Update()
    {
        for (int i = 0; i < keyBindings.Length; i++)
        {
            if (Managers.Instance.Input.GetKeyUp(keyBindings[i]))
            {
                OnKeyUp(i);
            }
        }
    }

    void UpdateKeyBindings()
    {
        keyBindings[0] = Managers.Instance.Key.GetKey(RhythmKey.Key1);
        keyBindings[1] = Managers.Instance.Key.GetKey(RhythmKey.Key2);
        keyBindings[2] = Managers.Instance.Key.GetKey(RhythmKey.Key3);
        keyBindings[3] = Managers.Instance.Key.GetKey(RhythmKey.Key4);
    }

    void OnKeyDown(int index)
    {
        if (buttonImages[index] != null)
            buttonImages[index].sprite = pressedSprites[index];

        if (lightEffects[index] != null)
            lightEffects[index].SetActive(true);
    }

    void OnKeyUp(int index)
    {
        if (buttonImages[index] != null)
            buttonImages[index].sprite = defaultSprites[index];

        if (lightEffects[index] != null)
            lightEffects[index].SetActive(false);
    }
}

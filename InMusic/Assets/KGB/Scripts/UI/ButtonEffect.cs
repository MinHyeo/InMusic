using UnityEngine;
using UnityEngine.UI;
using static Define;

public class ButtonEffect : MonoBehaviour
{
    public Image[] buttonImages; // 버튼 이미지 배열 (Key1 ~ Key4 순서)
    public Sprite[] pressedSprites; // 키를 눌렀을 때의 이미지
    public Sprite[] defaultSprites; // 기본 이미지
    public GameObject[] lightEffects; // 빛 효과 오브젝트 배열 (Key1 ~ Key4 순서)

    private KeyCode[] keyBindings = new KeyCode[4];

    void Start()
    {
        UpdateKeyBindings();
        Managers.Instance.Input.OnKeyPressed -= HandleKeyPressed;
        Managers.Instance.Input.OnKeyPressed += HandleKeyPressed;

    }
    void OnDestroy()
    {
        // 이벤트 해제 (중요!)
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

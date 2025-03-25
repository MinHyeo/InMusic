using UnityEngine;
using UnityEngine.UI;

public class KeyBindingManager : MonoBehaviour
{
    private enum KeyBindingState
    {
        Default,
        WaitingForKey
    }

    [Header("UI Elements")]
    public Button[] keyButtons;
    public Text[] keyTexts;
    public GameObject waitingImage;

    [Header("Key Bindings")]
    private KeyCode[] keyBindings = new KeyCode[4];

    private KeyBindingState currentState = KeyBindingState.Default;
    private int waitingKeyIndex = -1;

    void Start()
    {
        // Ű ���� �ʱ�ȭ
        keyBindings[0] = KeyCode.W;
        keyBindings[1] = KeyCode.A;
        keyBindings[2] = KeyCode.S;
        keyBindings[3] = KeyCode.D;

        // UI ��ư �ʱ�ȭ
        for (int i = 0; i < keyButtons.Length; i++)
        {
            int index = i;
            keyButtons[i].onClick.AddListener(() => StartKeyBinding(index));
            UpdateKeyText(index);
        }

        waitingImage.SetActive(false);
    }

    void Update()
    {
        if (currentState == KeyBindingState.WaitingForKey && Input.anyKeyDown)
        {
            foreach (KeyCode keyCode in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(keyCode))
                {
                    HandleKeyInput(keyCode);
                    break;
                }
            }
        }
    }

    private void StartKeyBinding(int index)
    {
        if (currentState == KeyBindingState.WaitingForKey) return;

        currentState = KeyBindingState.WaitingForKey;
        waitingKeyIndex = index;

        keyTexts[index].color = Color.red;
        waitingImage.SetActive(true);
    }

    private void HandleKeyInput(KeyCode newKey)
    {
        if (IsKeyAlreadyBound(newKey))
        {
            Debug.Log($"Ű {newKey}�� �̹� ��� ���Դϴ�!");
        }
        else
        {
            keyBindings[waitingKeyIndex] = newKey;
            Debug.Log($"Ű {newKey}�� ����Ǿ����ϴ�!");
            UpdateKeyText(waitingKeyIndex);
        }

        ResetKeyBindingState();
    }

    private bool IsKeyAlreadyBound(KeyCode key)
    {
        foreach (KeyCode boundKey in keyBindings)
        {
            if (boundKey == key) return true;
        }
        return false;
    }

    private void UpdateKeyText(int index)
    {
        keyTexts[index].text = keyBindings[index].ToString();
        keyTexts[index].color = Color.black;
    }

    private void ResetKeyBindingState()
    {
        currentState = KeyBindingState.Default;
        waitingKeyIndex = -1;
        waitingImage.SetActive(false);

        foreach (Text keyText in keyTexts)
        {
            keyText.color = Color.black;
        }
    }
}
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InputManager : MonoBehaviour
{
    [Header("Fixed Keys")]
    public KeyCode moveLeft = KeyCode.LeftArrow;
    public KeyCode moveRight = KeyCode.RightArrow;
    public KeyCode moveUp = KeyCode.UpArrow;
    public KeyCode moveDown = KeyCode.DownArrow;
    public KeyCode selectKey = KeyCode.Return;
    public KeyCode optionKey = KeyCode.F10;
    public KeyCode startKey = KeyCode.F5;
    public KeyCode backKey = KeyCode.Escape;

    [Header("Gameplay Keys")]
    public List<KeyCode> gameplayKeys = new List<KeyCode> { KeyCode.W, KeyCode.A, KeyCode.S, KeyCode.D };

    [Header("UI Buttons")]
    public Button moveLeftButton;
    public Button moveRightButton;
    public Button moveUpButton;
    public Button moveDownButton;
    public Button selectButton;
    public Button optionButton;
    public Button startButton;
    public Button backButton;

    private void Start()
    {
        // 버튼 클릭 이벤트 등록
        moveLeftButton.onClick.AddListener(() => HandleMoveLeft());
        moveRightButton.onClick.AddListener(() => HandleMoveRight());
        moveUpButton.onClick.AddListener(() => HandleMoveUp());
        moveDownButton.onClick.AddListener(() => HandleMoveDown());
        selectButton.onClick.AddListener(() => HandleSelect());
        optionButton.onClick.AddListener(() => HandleOption());
        startButton.onClick.AddListener(() => HandleStart());
        backButton.onClick.AddListener(() => HandleBack());
    }

    private void Update()
    {
        // 키 입력 감지 및 처리
        if (Input.GetKeyDown(moveLeft)) HandleMoveLeft();
        if (Input.GetKeyDown(moveRight)) HandleMoveRight();
        if (Input.GetKeyDown(moveUp)) HandleMoveUp();
        if (Input.GetKeyDown(moveDown)) HandleMoveDown();
        if (Input.GetKeyDown(selectKey)) HandleSelect();
        if (Input.GetKeyDown(optionKey)) HandleOption();
        if (Input.GetKeyDown(startKey)) HandleStart();
        if (Input.GetKeyDown(backKey)) HandleBack();

        // 리듬게임 플레이 키 입력 감지
        foreach (KeyCode key in gameplayKeys)
        {
            if (Input.GetKeyDown(key))
            {
                HandleGameplayKey(key);
            }
        }
    }

    private void HandleMoveLeft() => Debug.Log("Move Left");
    private void HandleMoveRight() => Debug.Log("Move Right");
    private void HandleMoveUp() => Debug.Log("Move Up");
    private void HandleMoveDown() => Debug.Log("Move Down");
    private void HandleSelect() => Debug.Log("Select / Confirm");
    private void HandleOption() => Debug.Log("Open Options");
    private void HandleStart() => Debug.Log("Start / Ready");
    private void HandleBack() => Debug.Log("Go Back");
    private void HandleGameplayKey(KeyCode key) => Debug.Log($"Gameplay Key Pressed: {key}");
}

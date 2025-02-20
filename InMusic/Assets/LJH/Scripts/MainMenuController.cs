using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MainMenuController : MonoBehaviour
{
    public Button multiButton;
    public Button soloButton;
    public Button leftButton;
    public Button rightButton;

    private enum MainMenuButton { Multi, Solo }
    private MainMenuButton currentButton;

    private Animator multiAnimator;
    private Animator soloAnimator;

    private void Start()
    {
        multiAnimator = multiButton.GetComponent<Animator>();
        soloAnimator = soloButton.GetComponent<Animator>();

        currentButton = MainMenuButton.Solo;
        InitializeButtons();

        leftButton.onClick.AddListener(() => ChangeButton(-1));
        rightButton.onClick.AddListener(() => ChangeButton(1));
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            ChangeButton(-1);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            ChangeButton(1);
        }
    }

    private void InitializeButtons()
    {
        multiButton.gameObject.SetActive(false);
        soloButton.gameObject.SetActive(true);

        UpdateArrowStates();
    }

    private void ChangeButton(int direction)
    {
        int nextIndex = (int)currentButton + direction;
        if (nextIndex < 0 || nextIndex >= System.Enum.GetValues(typeof(MainMenuButton)).Length)
            return;

        MainMenuButton nextButton = (MainMenuButton)nextIndex;

        Button previousButton = GetButtonFromEnum(currentButton);
        Button newButton = GetButtonFromEnum(nextButton);
        Animator previousAnimator = GetAnimatorFromEnum(currentButton);
        Animator newAnimator = GetAnimatorFromEnum(nextButton);

        // Animation Trigger 설정
        string trigger = direction == -1 ? "MoveRight" : "MoveLeft";

        newButton.gameObject.SetActive(true);
        newAnimator.SetTrigger(trigger);
        previousAnimator.SetTrigger(trigger);

        // 현재 버튼 갱신 후 UpdateArrowStates() 호출
        currentButton = nextButton;
        UpdateArrowStates();

        StartCoroutine(DisableButtonAfterAnimation(previousButton, previousAnimator));
    }

    private void UpdateArrowStates()
    {
        leftButton.interactable = currentButton > MainMenuButton.Multi;
        rightButton.interactable = currentButton < MainMenuButton.Solo;
    }

    private Button GetButtonFromEnum(MainMenuButton menuButton)
    {
        return menuButton == MainMenuButton.Solo ? soloButton : multiButton;
    }

    private Animator GetAnimatorFromEnum(MainMenuButton menuButton)
    {
        return menuButton == MainMenuButton.Solo ? soloAnimator : multiAnimator;
    }

    private IEnumerator DisableButtonAfterAnimation(Button button, Animator animator)
    {
        if (animator != null)
        {
            yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        }
        if (button != GetButtonFromEnum(currentButton))
        {
            button.gameObject.SetActive(false);
        }
        // 애니메이션 종료 후 다시 상태 업데이트
        UpdateArrowStates();
    }
}

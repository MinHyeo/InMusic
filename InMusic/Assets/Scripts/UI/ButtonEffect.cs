using UnityEngine;
using UnityEngine.UI;

public class ButtonEffect : MonoBehaviour
{
    public Image[] buttonImages; // 버튼 이미지 배열 (D, F, J, K 순서)
    public Sprite[] pressedSprites; // 키를 눌렀을 때의 이미지
    public Sprite[] defaultSprites; // 기본 이미지
    public GameObject[] lightEffects; // 빛 효과 오브젝트 배열 (D, F, J, K 순서)

    private KeyCode[] keys = { KeyCode.D, KeyCode.F, KeyCode.J, KeyCode.K }; // 키 배열

    void Update()
    {
        // 각 키에 대해 상태 확인
        for (int i = 0; i < keys.Length; i++)
        {
            if (Input.GetKeyDown(keys[i]))
            {
                OnKeyDown(i); // 키를 눌렀을 때의 효과
            }
            if (Input.GetKeyUp(keys[i]))
            {
                OnKeyUp(i); // 키를 뗐을 때의 효과
            }
        }
    }

    void OnKeyDown(int index)
    {
        // 버튼 이미지 변경
        buttonImages[index].sprite = pressedSprites[index];

        // 빛 효과 활성화
        lightEffects[index].SetActive(true);
    }

    void OnKeyUp(int index)
    {
        // 버튼 이미지를 기본 이미지로 복원
        buttonImages[index].sprite = defaultSprites[index];

        // 빛 효과 비활성화
        lightEffects[index].SetActive(false);
    }
}

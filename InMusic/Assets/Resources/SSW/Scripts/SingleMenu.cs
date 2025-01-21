using UnityEngine;
using UnityEngine.SceneManagement;

public class SingleMenu : MonoBehaviour
{
    // 전체 기능 구현 완료 후 Manager를 추가하여 수정 예정
    public void OnClickStart()
    {
        SceneManager.LoadScene("Next_Scene");
        Debug.Log("Start Button Clicked");
    }

    public void OnClickBack()
    {
        Debug.Log("Back Button Clicked");
    }

    public void OnClickOption()
    {
        Debug.Log("Option Button Clicked.");
    }
}

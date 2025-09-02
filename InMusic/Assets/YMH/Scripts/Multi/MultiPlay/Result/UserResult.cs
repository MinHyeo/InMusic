using UnityEngine;
using UnityEngine.UI;

public class UserResult : MonoBehaviour
{
    [SerializeField]
    private Sprite[] userBackgroundSprites;

    [SerializeField]
    private Text[] userNameText;
    [SerializeField]
    private Image[] userBackgroundImage;
    [SerializeField]
    private GameObject[] userSelectImage;

    public void SetUserResult(string userName, bool isRed, int index)
    {
        userNameText[index].text = userName;
        Debug.Log(userName + " " + isRed);
        userBackgroundImage[index].sprite = userBackgroundSprites[isRed ? 0 : 1];
    }

    public void Select(int index)
    {
        userSelectImage[index].SetActive(true);
        userSelectImage[1 - index].SetActive(false);
    }
}

using UnityEngine;
using UnityEngine.UI;

public class UserResult : MonoBehaviour
{
    [SerializeField]
    private Text[] userNameText;
    [SerializeField]
    private Image[] userBackggroundImage;
    [SerializeField]
    private GameObject[] userSelectImage;

    public void SetUserResult(string userName)
    {
        userSelectImage[1].SetActive(false);
        userSelectImage[0].SetActive(true);

        
    }

    public void Select(int index)
    {
        userSelectImage[index].SetActive(true);
        userSelectImage[1 - index].SetActive(false);
    }
}

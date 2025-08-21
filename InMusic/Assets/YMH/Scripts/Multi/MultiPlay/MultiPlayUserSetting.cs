using Managers;
using UnityEngine;
using UnityEngine.UI;

public class MultiPlayUserSetting : SingleTon<MultiPlayUserSetting>
{
    [Header("User Color")]
    [SerializeField]
    private Sprite[] userColors;

    [Header("User Color Images")]
    [SerializeField]
    private Image[] userColorImages;
    [SerializeField]
    private Image[] resultUserColorImages;

    [Header("User Name Texts")]
    [SerializeField]
    private Text[] userNameTexts;
    [SerializeField]
    private Text[] resultUserNameTexts;

    public void SetUserSetting(string userName, bool isRed, bool isMine)
    {
        // 플레이 영역 유저 세팅
        int index = isMine ? 0 : 1;
        userNameTexts[index].text = userName;
        userColorImages[index].sprite = userColors[isRed ? 0 : 1];
    }

    public void SetResultUserSetting(int userIndex, int index)
    {
        resultUserNameTexts[index].text = userNameTexts[userIndex].text;
        resultUserColorImages[index].sprite = userColorImages[userIndex].sprite;
    }
}   

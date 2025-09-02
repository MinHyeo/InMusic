using Managers;
using UnityEngine;
using UnityEngine.UI;

public class MultiPlayUserSetting : Singleton<MultiPlayUserSetting>
{
    [Header("User Color")]
    [SerializeField]
    private Sprite[] userColors;

    [Header("User Color Images")]
    [SerializeField]
    private Image[] userColorImages;

    [Header("User Name Texts")]
    [SerializeField]
    private Text[] userNameTexts;

    private bool[] isRed = new bool[2];

    public string GetUserName(int index)
    {
        return userNameTexts[index].text;
    }

    public bool GetIsRed(int index)
    {
        return isRed[index];
    }

    public void SetUserSetting(string userName, bool isRed, bool isMine)
    {
        // 플레이 영역 유저 세팅
        int index = isMine ? 0 : 1;
        userNameTexts[index].text = userName;
        userColorImages[index].sprite = userColors[isRed ? 0 : 1];
        this.isRed[index] = isRed;
    }
}   

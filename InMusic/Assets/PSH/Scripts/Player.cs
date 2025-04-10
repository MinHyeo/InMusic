using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private string playerID = "";    //½ºÆÀ ID
    [SerializeField] private string playerName = "";  //½ºÆÀ ÇÁ·ÎÇÊ ´Ð³×ÀÓ

    #region Get/Set
    public string PlayerID { get {return playerID; } set { playerID = value; } }
    public string PlayerName { get { return playerName; } set { playerName = value; } }
    #endregion
}

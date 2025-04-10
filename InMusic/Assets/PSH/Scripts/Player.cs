using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private string playerID = "";    //���� ID
    [SerializeField] private string playerName = "";  //���� ������ �г���

    #region Get/Set
    public string PlayerID { get {return playerID; } set { playerID = value; } }
    public string PlayerName { get { return playerName; } set { playerName = value; } }
    #endregion
}

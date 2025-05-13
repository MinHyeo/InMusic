using UnityEngine;

public class PlayerData : MonoBehaviour
{
    [SerializeField] private string playerID = "";    //���� ID
    [SerializeField] private string playerName = "";  //���� ������ �г���
    [SerializeField] private bool isHost = false;

    #region Get/Set
    public string PlayerID { get {return playerID; } set { playerID = value; } }
    public string PlayerName { get { return playerName; } set { playerName = value; } }
    public bool IsHost { get { return isHost; } set { isHost = value; } }
    #endregion
}

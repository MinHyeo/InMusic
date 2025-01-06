using UnityEngine;

public class Change_Signal : MonoBehaviour
{
    public void ChangeSignal()
    {
        transform.parent.parent.GetComponent<Main_Lobby_UI>().GetChangeSignal();
    }
}

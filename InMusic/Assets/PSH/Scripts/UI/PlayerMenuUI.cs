using Unity.VisualScripting;
using UnityEngine;

public class PlayerMenuUI : MonoBehaviour
{
    [SerializeField] Waiting_Room_UI waitUI;
    [SerializeField] int uITarget;

    public void SetTarget(int target) {
        uITarget = target;
    }

    public void OnOwnerButton() {
        if (uITarget == 1) {
            waitUI.SetOwner(true);
        }
        else
        {
            waitUI.SetOwner(false);
        }
            gameObject.SetActive(false);
    }

    public void OnKickButton() {
        Debug.Log("아직 미구현");

        gameObject.SetActive(false);
    }
}

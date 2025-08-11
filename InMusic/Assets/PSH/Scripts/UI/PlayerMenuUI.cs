using UnityEngine;

public class PlayerMenuUI : MonoBehaviour
{
    [SerializeField] PlayerInfo targetPlayer;

    public void SetTarget(PlayerInfo target) {
        targetPlayer = target;
    }

    public void OnOwnerButton() {
        targetPlayer.IsOwner = true;

        gameObject.SetActive(false);
    }

    public void OnKickButton() {
        Debug.Log("���� �̱���");

        gameObject.SetActive(false);
    }
}

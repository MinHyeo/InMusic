using UnityEngine;
using UnityEngine.UI;


public class ClientSongListBlock : MonoBehaviour
{
    [SerializeField] private Image blockImage;
    private void Awake()
    {
        if (blockImage == null)
        {
            blockImage = GetComponent<Image>();
            Debug.LogWarning("BlockImage is not assigned, using Image component from GameObject.");
        }
    }

    private void Update()
    {
        bool isMaster = SharedModeMasterClientTracker.IsLocalPlayerSharedModeMasterClient();
        if (blockImage != null)
            blockImage.enabled = !isMaster;
    }
}

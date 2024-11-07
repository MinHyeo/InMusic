using UnityEngine;
using static UnityEditor.Progress;

public abstract class UI_Base : MonoBehaviour
{
    [SerializeField] public ResourceManager rtemp = new ResourceManager();

    //Setting UI Call
    public void Gear() {
        Debug.Log("Gear function is not implemented");
    }

    public void Guide() {
        rtemp.Instantiate("KeyGuide");
    }
}

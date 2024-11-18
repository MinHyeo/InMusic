using UnityEngine;
using static UnityEditor.Progress;

namespace UI_BASE_PSH
{
    public abstract class UI_Base : MonoBehaviour
    {
        [SerializeField] public ResourceManager rtemp = new ResourceManager();
        [SerializeField] public GameObject curUI = null;

        //Setting UI Call
        public void Gear()
        {
            curUI = rtemp.Instantiate("SoundSetting_UI");
        }

        public void Guide()
        {
            curUI = rtemp.Instantiate("KeyGuide_UI");
        }
    }
}

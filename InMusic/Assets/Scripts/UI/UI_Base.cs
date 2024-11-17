using UnityEngine;
using static UnityEditor.Progress;

namespace UI_BASE_PSH
{
    public abstract class UI_Base : MonoBehaviour
    {
        [SerializeField] public ResourceManager rtemp = new ResourceManager();

        //Setting UI Call
        public void Gear()
        {
            rtemp.Instantiate("SoundSetting_UI");
        }

        public void Guide()
        {
            rtemp.Instantiate("KeyGuide_UI");
        }
    }
}

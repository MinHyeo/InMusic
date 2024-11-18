using UnityEngine;
using static UnityEditor.Progress;

namespace UI_BASE_PSH
{
    public abstract class UI_Base : MonoBehaviour
    {
        [SerializeField] public ResourceManager rtemp = new ResourceManager();
        [SerializeField] public GameObject curSetUI = null;
        [SerializeField] public GameObject guideUI = null;

        //Setting UI Call
        public void Gear()
        {
            curSetUI = rtemp.Instantiate("SoundSetting_UI");
        }

        public void Guide()
        {
            guideUI = rtemp.Instantiate("KeyGuide_UI");
        }
    }
}

using UnityEngine;
using static UnityEditor.Progress;

namespace UI_BASE_PSH
{
    public abstract class UI_Base : MonoBehaviour
    {
        [SerializeField] public GameObject curSetUI = null;
        [SerializeField] public GameObject guideUI = null;

        //Setting UI Call
        public void Gear()
        {
            curSetUI = GameManager.Resource.Instantiate("SoundSetting_UI");
        }

        public void Guide()
        {
            guideUI = GameManager.Resource.Instantiate("KeyGuide_UI");
        }
    }
}

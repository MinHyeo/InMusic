using UnityEngine;

namespace UI_BASE_PSH
{
    public abstract class UI_Base : MonoBehaviour
    {
        [SerializeField] public GameObject SettingUI = null;
        [SerializeField] public GameObject guideUI = null;

        //Setting UI Call
        public void Gear()
        {
            SettingUI = GameManager_PSH.Resource.Instantiate("SoundSetting_UI");
        }

        public void Guide()
        {
            guideUI = GameManager_PSH.Resource.Instantiate("KeyGuide_UI");
        }
    }
}

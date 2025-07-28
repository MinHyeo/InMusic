using UnityEngine;
using UnityEngine.UI;

namespace Play
{
    public class MultiAccuracy : MonoBehaviour
    {
        [SerializeField]
        private Text myAccuracyText;
        [SerializeField]
        private Text matchAccuracyText;

        public void Init()
        {
            if (myAccuracyText == null || matchAccuracyText == null)
            {
                Debug.LogError("Accuracy Text components are not assigned in MultiAccuracy.");
                return;
            }

            myAccuracyText.text = "0.00%";
            matchAccuracyText.text = "0.00%";
        }

        public void UpdateAccuracy(float myAccuracy, float matchAccuracy)
        {
            if (myAccuracyText == null || matchAccuracyText == null)
            {
                Debug.LogError("Accuracy Text components are not assigned in MultiAccuracy.");
                return;
            }
            myAccuracyText.text = $"{myAccuracy:F2}%";
            matchAccuracyText.text = $"{matchAccuracy:F2}%";
        }
    }    
}


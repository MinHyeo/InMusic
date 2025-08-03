using UnityEngine;
using UnityEngine.UI;

public class HpBar : MonoBehaviour
{
    private Slider hpSlider;

    private void Awake()
    {
        hpSlider = GetComponent<Slider>();
    }

    public bool SetHp(float addValue)
    {
        float hpValue = Mathf.Clamp(hpSlider.value + (addValue / 100f), 0f, 1f);
        hpSlider.value = hpValue;

        bool isDead = hpValue <= 0f;
        return isDead;
    }
}

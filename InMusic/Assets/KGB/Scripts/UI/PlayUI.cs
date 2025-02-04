using TMPro;
using Unity.Burst.Intrinsics;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class PlayUI : MonoBehaviour
{
    [SerializeField] Slider hpBar;
    [SerializeField] TextMeshProUGUI comboText; // 콤보 표시 텍스트
    [SerializeField] GameObject[] judgeText;
    public Image BackImage;
    public TextMeshProUGUI countText;
    

    void Start()
    {
        hpBar.value = GameManager.Instance.maxHP;
        comboText.text = GameManager.Instance.combo.ToString();
        countText.text = "";
    }

    void Update()
    {
        
    }

    public void UpdatePlayUI()
    {
        comboText.text = GameManager.Instance.combo.ToString();
        hpBar.value = GameManager.Instance.curHP/100f;
    }

    public void JudgeTextUpdate(string judgement)
    {
        foreach (GameObject text in judgeText)
        {
            text.SetActive(false); // 이전에 활성화된 텍스트도 포함해 모두 비활성화
        }
        switch (judgement)
        {
            case "Great":
                judgeText[0].SetActive(true); // Great 활성화
                break;
            case "Good":
                judgeText[1].SetActive(true); // Good 활성화
                break;
            case "Bad":
                judgeText[2].SetActive(true); // Bad 활성화
                break;
            case "Miss":
                judgeText[3].SetActive(true); // Miss 활성화
                break;
        }
    }
}

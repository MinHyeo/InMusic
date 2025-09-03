using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayUI_Multi : MonoBehaviour
{
    [SerializeField] Slider hpBar; //체력 표시 슬라이드바
    [SerializeField] TextMeshProUGUI comboText; // 콤보 표시 텍스트
    public TextMeshProUGUI scoreText; // 점수 표시 텍스트
    public TextMeshProUGUI accuracyText; // 정확도 표시 텍스트
    [SerializeField] GameObject[] judgeText;
    public TextMeshProUGUI countText;
    void Start()
    {
        //hpBar.value = GameManagerProvider.Instance.MaxHP;
        //comboText.text = GameManagerProvider.Instance.Combo.ToString();
        countText.text = "";
    }

    public void UpdatePlayUI_Multi(ScoreData data)
    {
        //comboText.text = GameManagerProvider.Instance.Combo.ToString();
        //hpBar.value = GameManagerProvider.Instance.CurHP / 100f;

        //체력, 점수, 정확도, 콤보
        hpBar.value = data.curHp;
        scoreText.text = $"{data.totalScore:F0}";
        accuracyText.text = $"{data.accuracy:F2}%";
        comboText.text = $"{data.combo}";
        JudgeTextUpdate(data.judgement);
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

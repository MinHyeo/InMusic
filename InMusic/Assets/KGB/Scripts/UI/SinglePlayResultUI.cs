using TMPro;
using UnityEngine;

public class SinglePlayResultUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI score;
    [SerializeField] private TextMeshProUGUI great;
    [SerializeField] private TextMeshProUGUI good;
    [SerializeField] private TextMeshProUGUI bad;
    [SerializeField] private TextMeshProUGUI miss;
    [SerializeField] private TextMeshProUGUI accuracy;
    [SerializeField] private TextMeshProUGUI combo;
    [SerializeField] private TextMeshProUGUI rate;
    [SerializeField] private GameObject fullCombo;

    [SerializeField] private GameObject resultUI;
    [SerializeField] TMP_ColorGradient fullComboColor;
    [SerializeField] TMP_ColorGradient rateColor;

    void Start()
    {
        fullCombo.SetActive(false);
    }

    
    void Update()
    {
        
    }

    public void UpdateResult()
    {
        score.text = GameManager.Instance.totalScore.ToString();
        great.text = GameManager.Instance.greatCount.ToString();
        good.text = GameManager.Instance.goodCount.ToString();
        bad.text = GameManager.Instance.badCount.ToString();
        miss.text = GameManager.Instance.missCount.ToString();
        accuracy.text = GameManager.Instance.accuracy.ToString();
        combo.text = GameManager.Instance.maxCombo.ToString();
        float percentage = (float)GameManager.Instance.totalScore / 1000000f * 100f;
        if (percentage >= 95f)
        {
            rate.text = "S";
        }
        else if (percentage >= 90f)
        {
            rate.text = "A";
        }
        else if (percentage >= 80f)
        {
            rate.text = "B";
        }
        else if (percentage >= 70f)
        {
            rate.text = "C";
        }
        else
        {
            rate.text = "D";
        }
        if (GameManager.Instance.combo == GameManager.Instance.totalNotes)
        {
            Debug.Log("Ç®ÄÞº¸");
            fullCombo.SetActive(true);
            rate.colorGradientPreset = fullComboColor;
        }
        else
        {
            rate.colorGradientPreset = rateColor;
        }
    }

    public void InitResult()
    {
        UpdateResult();
        resultUI.SetActive(true);
    }
}

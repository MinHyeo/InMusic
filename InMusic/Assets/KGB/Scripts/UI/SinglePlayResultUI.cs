using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    private PlayResultData playResult;

    void Start()
    {
        resultUI.SetActive(false);
        fullCombo.SetActive(false);
    }

    
    void Update()
    {
        
    }

    public void UpdateResult()
    {
        playResult = new PlayResultData
        {
            Score = (int)GameManager.Instance.totalScore,
            Great = GameManager.Instance.greatCount,
            Good = GameManager.Instance.goodCount,
            Bad = GameManager.Instance.badCount,
            Miss = GameManager.Instance.missCount,
            Accuracy = GameManager.Instance.accuracy,
            Combo = GameManager.Instance.maxCombo,
            Rank = (float)GameManager.Instance.totalScore / 1000000f * 100f,
            FullCombo = (GameManager.Instance.combo == GameManager.Instance.totalNotes)
        };

        //score.text = GameManager.Instance.totalScore.ToString();
        //great.text = GameManager.Instance.greatCount.ToString();
        //good.text = GameManager.Instance.goodCount.ToString();
        //bad.text = GameManager.Instance.badCount.ToString();
        //miss.text = GameManager.Instance.missCount.ToString();
        //accuracy.text = GameManager.Instance.accuracy.ToString();
        //combo.text = GameManager.Instance.maxCombo.ToString();

        score.text = playResult.Score.ToString();
        great.text = playResult.Great.ToString();
        good.text = playResult.Good.ToString();
        bad.text = playResult.Bad.ToString();
        miss.text = playResult.Miss.ToString();
        accuracy.text = playResult.Accuracy.ToString();
        combo.text = playResult.Combo.ToString();

        //float percentage = (float)GameManager.Instance.totalScore / 1000000f * 100f;
        //if (percentage >= 95f)
        //{
        //    rate.text = "S";
        //}
        //else if (percentage >= 90f)
        //{
        //    rate.text = "A";
        //}
        //else if (percentage >= 80f)
        //{
        //    rate.text = "B";
        //}
        //else if (percentage >= 70f)
        //{
        //    rate.text = "C";
        //}
        //else
        //{
        //    rate.text = "D";
        //}
        if (playResult.Rank >= 95f) rate.text = "S";
        else if (playResult.Rank >= 90f) rate.text = "A";
        else if (playResult.Rank >= 80f) rate.text = "B";
        else if (playResult.Rank >= 70f) rate.text = "C";
        else rate.text = "D";

        if (playResult.FullCombo)
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

    public void NextButton()
    {
        string musicID = GameManager_PSH.Instance.GetComponent<MusicData>().MusicID;
        string userID = GameManager_PSH.Data.GetPlayerID();
        DBManager.Instance.StartCheckHighScore(userID, musicID, playResult.Score, playResult.Combo, playResult.Accuracy, rate.text);

        SceneManager.LoadScene(0);
        
    }
}

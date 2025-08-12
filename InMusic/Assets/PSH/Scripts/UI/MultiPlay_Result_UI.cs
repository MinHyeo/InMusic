using TMPro;
using UnityEngine;
/// <summary>
/// SinglePlayResultUI ÆÄÄí¸®
/// </summary>
public class MultiPlay_Result_UI : MonoBehaviour
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

    [SerializeField] PlayerStatusUIController statusUIController;

    private PlayResultData playResult1;
    private PlayResultData playResult2;

    void Start()
    {
        resultUI.SetActive(false);
        fullCombo.SetActive(false);
    }

    public void UpdateResult()
    {
        /*
        playResult = new PlayResultData
        {
            Score = (int)GameManagerProvider.Instance.TotalScore,
            Great = GameManagerProvider.Instance.GreatCount,
            Good = GameManagerProvider.Instance.GoodCount,
            Bad = GameManagerProvider.Instance.BadCount,
            Miss = GameManagerProvider.Instance.MissCount,
            Accuracy = GameManagerProvider.Instance.Accuracy,
            Combo = GameManagerProvider.Instance.MaxCombo,
            Rank = (float)GameManagerProvider.Instance.TotalScore / 1000000f * 100f,
            FullCombo = (GameManagerProvider.Instance.Combo == GameManagerProvider.Instance.TotalNotes)
        };
        score.text = playResult.Score.ToString();
        great.text = playResult.Great.ToString();
        good.text = playResult.Good.ToString();
        bad.text = playResult.Bad.ToString();
        miss.text = playResult.Miss.ToString();
        accuracy.text = playResult.Accuracy.ToString();
        combo.text = playResult.Combo.ToString();

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
        }*/
    }

    public void InitResult()
    {
        UpdateResult();
        resultUI.SetActive(true);
    }

    public void NextButton()
    {
        /*
        string musicID = GameManager_PSH.Instance.GetComponent<MusicData>().MusicID;
        string userID = GameManager_PSH.Data.GetPlayerID();
        DBManager.Instance.StartCheckHighScore(userID, musicID, playResult.Score, playResult.Combo, playResult.Accuracy, rate.text);

        LoadingScreen.Instance.LoadScene("Single_Lobby_PSH");*/

    }
}

using TMPro;
using UnityEngine;

/// <summary>
/// SinglePlayResultUI 파쿠리
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

    [SerializeField] TMP_ColorGradient fullComboColor;
    [SerializeField] TMP_ColorGradient rateColor;

    [SerializeField] Player_Result pResult;

    [Header("플레이 결과")]
    [SerializeField] private PlayResultData playResult1data;
    [SerializeField] private PlayResultData playResult2data;

    void Start()
    {
        fullCombo.SetActive(false);

        if (GameManager_PSH.PlayerRole)
        {
            InitHost();
        }
        else {
            InitClient();
        }
    }

    public void NextButton()
    {
        //결과 저장
        string musicID = GameManager_PSH.Instance.GetComponent<MusicData>().MusicID;
        string userID = GameManager_PSH.Data.GetPlayerID();
        if (GameManager_PSH.PlayerRole) {
            DBManager.Instance.StartCheckHighScore(userID, musicID, playResult1data.Score, playResult1data.Combo, playResult1data.Accuracy, rate.text);
        }
        else
        {
            DBManager.Instance.StartCheckHighScore(userID, musicID, playResult2data.Score, playResult2data.Combo, playResult2data.Accuracy, rate.text);
        }

        //세션 종료 및 방 목록 로비로 이동
        NetworkManager.runnerInstance.Shutdown();
        LoadingScreen.Instance.LoadScene("KGB_Multi_Lobby");
    }

    public void ShowP1ResulltButton() {
        pResult.SelectP1();
        //P1 결과 보여주기
        score.text = playResult1data.Score.ToString();
        great.text = playResult1data.Great.ToString();
        good.text = playResult1data.Good.ToString();
        bad.text = playResult1data.Bad.ToString();
        miss.text = playResult1data.Miss.ToString();
        accuracy.text = playResult1data.Accuracy.ToString();
        combo.text = playResult1data.Combo.ToString();
        CheckRank(playResult1data.Score, playResult1data.FullCombo);
    }

    public void ShowP2ResulltButton() {
        pResult.SelectP2();
        //P2 결과 보여주기
        score.text = playResult2data.Score.ToString();
        great.text = playResult2data.Great.ToString();
        good.text = playResult2data.Good.ToString();
        bad.text = playResult2data.Bad.ToString();
        miss.text = playResult2data.Miss.ToString();
        accuracy.text = playResult2data.Accuracy.ToString();
        combo.text = playResult2data.Combo.ToString();
        CheckRank(playResult2data.Score, playResult2data.FullCombo);
    }

    void InitHost() {
        playResult1data = new PlayResultData
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
        //TODO: P2 정보 가져오기

        CheckWinner();
        ShowP1ResulltButton();
    }

    void InitClient() {
        playResult2data = new PlayResultData
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
        //TODO: P1 정보 가져오기

        CheckWinner();
        ShowP2ResulltButton();
    }

    void CheckRank(int score, bool isFullCombo)
    {
        if (score >= 95f) rate.text = "S";
        else if (score >= 90f) rate.text = "A";
        else if (score >= 80f) rate.text = "B";
        else if (score >= 70f) rate.text = "C";
        else rate.text = "D";

        if (isFullCombo)
        {
            Debug.Log("풀콤보");
            fullCombo.SetActive(true);
            rate.colorGradientPreset = fullComboColor;
        }
        else
        {
            rate.colorGradientPreset = rateColor;
        }
        return;
    }

    void CheckWinner()
    {
        if (playResult1data.Score >= playResult2data.Score)
        {
            pResult.SetP1Win();
        }
        else
        {
            pResult.SetP2Win();
        }
    }
}

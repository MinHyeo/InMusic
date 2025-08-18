using Fusion;
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
    [SerializeField] PlayResultData myResult;
    [SerializeField] PlayResultData otherResult;


    void Start()
    {
        fullCombo.SetActive(false);
        InintMyResult();
    }

    public void SetOtherPlayerResult(int score, int great, int good, int bad, int miss, float acc, int combo, float rank, bool fullcom) {
        Debug.Log($"{score} {great} {good} {bad} {miss} {acc}");
        otherResult = new PlayResultData
        {
            Score = score,
            Great = great,
            Good = good,
            Bad = bad,
            Miss = miss,
            Accuracy = acc,
            Combo = combo,
            Rank = rank,
            FullCombo = fullcom
        };
        Debug.Log("상대방 정보 입력 완료");
        CheckWinner();
    }
    public void NextButton()
    {
        //결과 저장
        string musicID = GameManager_PSH.Instance.GetComponent<MusicData>().MusicID;
        string userID = GameManager_PSH.Data.GetPlayerID();
        if (GameManager_PSH.PlayerRole) {
            DBManager.Instance.StartCheckHighScore(userID, musicID, myResult.Score, myResult.Combo, myResult.Accuracy, rate.text);
        }
        else
        {
            DBManager.Instance.StartCheckHighScore(userID, musicID, myResult.Score, myResult.Combo, myResult.Accuracy, rate.text);
        }

        //세션 종료 및 방 목록 로비로 이동
        NetworkManager.runnerInstance.Shutdown();
        GameManager_PSH.PlayerRole = false;
        LoadingScreen.Instance.LoadScene("KGB_Multi_Lobby");
    }

    public void ShowP1ResulltButton() {
        pResult.SelectP1();
        //P1 결과 보여주기
        if (GameManager_PSH.PlayerRole)
        {
            CheckRank(myResult.Rank, myResult.FullCombo);
            score.text = otherResult.Score.ToString();
            great.text = otherResult.Great.ToString();
            good.text = otherResult.Good.ToString();
            bad.text = otherResult.Bad.ToString();
            miss.text = otherResult.Miss.ToString();
            accuracy.text = otherResult.Accuracy.ToString();
            combo.text = otherResult.Combo.ToString();
            CheckRank(otherResult.Rank, otherResult.FullCombo);
        }
        else
        {
            score.text = myResult.Score.ToString();
            great.text = myResult.Great.ToString();
            good.text = myResult.Good.ToString();
            bad.text = myResult.Bad.ToString();
            miss.text = myResult.Miss.ToString();
            accuracy.text = myResult.Accuracy.ToString();
            combo.text = myResult.Combo.ToString();
        }
    }

    public void ShowP2ResulltButton() {
        pResult.SelectP2();
        if (GameManager_PSH.PlayerRole) {
            score.text = otherResult.Score.ToString();
            great.text = otherResult.Great.ToString();
            good.text = otherResult.Good.ToString();
            bad.text = otherResult.Bad.ToString();
            miss.text = otherResult.Miss.ToString();
            accuracy.text = otherResult.Accuracy.ToString();
            combo.text = otherResult.Combo.ToString();
            CheckRank(otherResult.Rank, otherResult.FullCombo);
        }
        else
        {
            score.text = myResult.Score.ToString();
            great.text = myResult.Great.ToString();
            good.text = myResult.Good.ToString();
            bad.text = myResult.Bad.ToString();
            miss.text = myResult.Miss.ToString();
            accuracy.text = myResult.Accuracy.ToString();
            combo.text = myResult.Combo.ToString();
            CheckRank(myResult.Rank, myResult.FullCombo);
        }
    }
    void CheckWinner()
    {
        if (myResult.Score >= otherResult.Score)
        {
            pResult.SetP1Win();
        }
        else
        {
            pResult.SetP2Win();
        }
    }

    void CheckRank(float rank, bool isFullCombo)
    {
        if (rank >= 95f) rate.text = "S";
        else if (rank >= 90f) rate.text = "A";
        else if (rank >= 80f) rate.text = "B";
        else if (rank >= 70f) rate.text = "C";
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

    void InintMyResult() {
        myResult = new PlayResultData
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
        Debug.Log("본인 결과 설정 완료");
        //결과 전달
        ResultRCPManager.Instance.RPC_SendMyReult(myResult.Score, myResult.Great, myResult.Good, myResult.Bad, myResult.Miss,
                                                                                  myResult.Accuracy, myResult.Combo, myResult.Rank, myResult.FullCombo);

        //결과 확인
        if (GameManager_PSH.PlayerRole) {
            ShowP1ResulltButton();
        }
        else
        {
            ShowP2ResulltButton();
        }
    }
}

using Fusion;
using TMPro;
using UnityEngine;
using System.Collections;

/// <summary>
/// SinglePlayResultUI 파쿠리
/// </summary>
public class MultiPlay_Result_UI : MonoBehaviour
{
    [Header("결과창 UI 요소")]
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

    [SerializeField] Player_Result pResultUI;

    [Header("플레이 결과")]
    [SerializeField] PlayResultData myResult;
    [SerializeField] PlayResultData otherResult;


    void Start()
    {
        fullCombo.SetActive(false);
        InintMyResult();
        if (GameManager_PSH.PlayerRole) {
            pResultUI.SetP1Name(GameManager_PSH.Data.MyName);
            pResultUI.SetP2Name(GameManager_PSH.Data.OtherName);
        }
        else
        {
            pResultUI.SetP1Name(GameManager_PSH.Data.OtherName);
            pResultUI.SetP2Name(GameManager_PSH.Data.MyName);
        }
    }

    #region Buttons
    public void ShowP1ResulltButton() {
        pResultUI.SelectP1();
        //P1 결과 보여주기
        if (GameManager_PSH.PlayerRole)
        {
            SetResultToUI(myResult);
        }
        else
        {
            SetResultToUI(otherResult);
        }
    }

    public void ShowP2ResulltButton() {
        pResultUI.SelectP2();
        if (GameManager_PSH.PlayerRole) {
            SetResultToUI(otherResult);
        }
        else
        {
            SetResultToUI(myResult);
        }
    }
    public void NextButton()
    {
        //결과 저장
        string musicID = GameManager_PSH.Instance.GetComponent<MusicData>().MusicID;
        string userID = GameManager_PSH.Data.GetPlayerID();
        if (GameManager_PSH.PlayerRole)
        {
            DBManager.Instance.StartCheckHighScore(userID, musicID, myResult.Score, myResult.Combo, myResult.Accuracy, rate.text);
        }
        else
        {
            DBManager.Instance.StartCheckHighScore(userID, musicID, myResult.Score, myResult.Combo, myResult.Accuracy, rate.text);
        }

        //세션 종료 후목록 로비로 이동
        NetworkManager.runnerInstance.Shutdown();
        GameManager_PSH.PlayerRole = false;
        GameManager_PSH.Data.OtherName = "Who?";
        LoadingScreen.Instance.LoadScene("KGB_Multi_Lobby");
    }

    #endregion

    /// <summary>
    /// PlayResultData 값을 UI에 할당
    /// </summary>
    /// <param name="target"></param>
    void SetResultToUI(PlayResultData target)
    {
        score.text = target.Score.ToString();
        great.text = target.Great.ToString();
        good.text = target.Good.ToString();
        bad.text = target.Bad.ToString();
        miss.text = target.Miss.ToString();
        accuracy.text = target.Accuracy.ToString();
        combo.text = target.Combo.ToString();
        CheckRank(target.Rank, myResult.FullCombo);
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

    void CheckWinner()
    {
        if (GameManager_PSH.PlayerRole)
        {
            if (myResult.Score >= otherResult.Score)
            {
                pResultUI.SetP1Win();
            }
            else
            {
                pResultUI.SetP2Win();
            }
        }
        else
        {
            if (myResult.Score >= otherResult.Score)
            {
                pResultUI.SetP2Win();
            }
            else
            {
                pResultUI.SetP1Win();
            }
        }
    }

    #region SetResultData

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
        StartCoroutine(Ready());

        //결과 확인
        if (GameManager_PSH.PlayerRole) {
            ShowP1ResulltButton();
        }
        else
        {
            ShowP2ResulltButton();
        }
    }

    public void SetOtherPlayerResult(int score, int great, int good, int bad, int miss, float acc, int combo, float rank, bool fullcom)
    {
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
    public IEnumerator Ready()
    {
        Debug.Log("레디 기다리는중");
        yield return new WaitUntil(() => ResultRCPManager.Instance != null);
        ResultRCPManager.Instance.RPC_SendMyReult(myResult.Score, myResult.Great, myResult.Good, myResult.Bad, myResult.Miss,
                                                                          myResult.Accuracy, myResult.Combo, myResult.Rank, myResult.FullCombo);
    }

    #endregion
}

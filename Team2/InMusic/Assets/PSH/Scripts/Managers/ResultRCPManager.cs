using UnityEngine;
using Fusion;

public class ResultRCPManager : NetworkBehaviour
{
    /// <summary>
    /// 싱글톤
    /// </summary>
    public static ResultRCPManager Instance { get; private set; }
    [SerializeField] MultiPlay_Result_UI result_UI;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
    }


    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_SendMyReult(int score, int great, int good, int bad, int miss, float acc, int combo, float rank, bool fullcom, RpcInfo info = default)
    {
        // 내가 보낸 메시지는 무시
        if (info.Source == NetworkManager.runnerInstance.LocalPlayer)
        {
            return;
        }
        Debug.Log("상대방 결과 정보 받고 UI에 보내기");
        result_UI.SetOtherPlayerResult(score, great, good, bad, miss, acc, combo, rank, fullcom);
    }
}

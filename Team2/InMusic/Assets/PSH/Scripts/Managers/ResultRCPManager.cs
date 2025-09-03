using UnityEngine;
using Fusion;

public class ResultRCPManager : NetworkBehaviour
{
    /// <summary>
    /// �̱���
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
        // ���� ���� �޽����� ����
        if (info.Source == NetworkManager.runnerInstance.LocalPlayer)
        {
            return;
        }
        Debug.Log("���� ��� ���� �ް� UI�� ������");
        result_UI.SetOtherPlayerResult(score, great, good, bad, miss, acc, combo, rank, fullcom);
    }
}

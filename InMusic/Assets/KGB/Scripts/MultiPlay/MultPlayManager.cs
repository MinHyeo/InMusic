using Fusion;
using UnityEngine;

public class MultPlayManager : NetworkBehaviour
{
    public static MultPlayManager Instance { get; private set; }

    private void Awake()
    {
        // �̱��� �ʱ�ȭ (�ߺ� ����)
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
        //DontDestroyOnLoad(this.gameObject); // �� ��ȯ �� ���� (���û���)
    }
    private void OnDestroy()
    {
        // �� �̵��̳� ���� �ı� �� ����
        if (Instance == this)
        {
            Instance = null;
        }
    }


    // ���� ������ ��뿡�� ������ RPC
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_SendNoteJudgement(int noteId, string judgement, int keyIndex, float percent, RpcInfo info = default)
    {
        // ���� ���� �޽����� ����
        Debug.Log($"[RPC]��Ʈ ���� ����: noteId={noteId} , judgement={judgement}, key={keyIndex}, percent={percent}");
        if (info.Source == NetworkManager.runnerInstance.LocalPlayer)
        {
            return;
        }

        Debug.Log($"[RPC] ����� ��Ʈ ���� ó��: noteId={noteId} , judgement={judgement}, key={keyIndex}, percent={percent}");
        //����ó�� (�÷��̾�1 �� ������ �÷��̾�2�� ���� ���� -> �ش簪�� ������ �޼���(��)���� ����)
        var multiNoteManager =  MultiNoteManager.Instance;
        if (multiNoteManager == null)
        {
            Debug.LogWarning("[RPC] MultiNoteManager �ν��Ͻ��� ã�� �� �����ϴ�.");
            return;
        }
        // ��� ��Ʈ ã��
        if (multiNoteManager.TryGetNoteByIndex(noteId, out Note_Multi note))
        {
            // ������ �´� ó��
            note.JudgmentSimulateNote(judgement, percent);
        }
        else
        {
            Debug.LogWarning($"[RPC] �ش� noteId({noteId})�� ã�� �� �����ϴ�.");
        }


    }


}

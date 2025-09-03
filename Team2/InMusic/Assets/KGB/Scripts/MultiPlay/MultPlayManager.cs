using Fusion;
using UnityEngine;

public class MultPlayManager : NetworkBehaviour
{
    public static MultPlayManager Instance { get; private set; }

    public bool p1_ready = false;
    public bool p2_ready = false;
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
    public void RPC_SendNoteJudgement(int noteId, int keyIndex, float percent, float curHp, float totalScore, int combo, int missCount, string judgement, RpcInfo info = default)
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
            //note.JudgmentSimulateNote(judgement, percent);
            MultiNoteManager.Instance.InsertJudgement(noteId, judgement, keyIndex, percent);
            MultiNoteManager.Instance.InsertScoreData(noteId, percent, curHp, totalScore, combo, missCount, judgement);
        }
        else
        {
            Debug.LogWarning($"[RPC] �ش� noteId({noteId})�� ã�� �� �����ϴ�.");
        }
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_CheckReady(RpcInfo info = default)
    {
        
        // ���� ���� �޽����� ����
        if (info.Source == NetworkManager.runnerInstance.LocalPlayer)
        {
            Debug.Log("���� üũ ����");
            p1_ready = true;
            if (p1_ready && p2_ready)
            {
                Debug.Log("�Ѵ� ���� üũ��");
                RPC_GameStart();
            }

            return;
        }
        else
        {
            p2_ready = true;
            Debug.Log("��� ���� üũ ����");
        }


        if (p1_ready && p2_ready&& info.Source == NetworkManager.runnerInstance.LocalPlayer)
        {
            Debug.Log("�Ѵ� ���� üũ��");
            RPC_GameStart();
        }

    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_GameStart(RpcInfo info = default)
    {
        KGB_GameManager_Multi.Instance.StartGame();
        Debug.Log("���ӽ��� RPC");

    }




}

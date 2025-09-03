using Fusion;
using UnityEngine;

public class MultPlayManager : NetworkBehaviour
{
    public static MultPlayManager Instance { get; private set; }

    public bool p1_ready = false;
    public bool p2_ready = false;
    private void Awake()
    {
        // 싱글톤 초기화 (중복 방지)
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
        //DontDestroyOnLoad(this.gameObject); // 씬 전환 시 유지 (선택사항)
    }
    private void OnDestroy()
    {
        // 씬 이동이나 수동 파괴 시 정리
        if (Instance == this)
        {
            Instance = null;
        }
    }


    // 판정 정보를 상대에게 보내는 RPC
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_SendNoteJudgement(int noteId, int keyIndex, float percent, float curHp, float totalScore, int combo, int missCount, string judgement, RpcInfo info = default)
    {
        // 내가 보낸 메시지는 무시
        Debug.Log($"[RPC]노트 판정 보냄: noteId={noteId} , judgement={judgement}, key={keyIndex}, percent={percent}");
        if (info.Source == NetworkManager.runnerInstance.LocalPlayer)
        {
            return;
        }

        Debug.Log($"[RPC] 상대의 노트 판정 처리: noteId={noteId} , judgement={judgement}, key={keyIndex}, percent={percent}");
        //판정처리 (플레이어1 이 보내면 플레이어2가 여기 실행 -> 해당값을 가지고 메서드(값)으로 진행)
        var multiNoteManager =  MultiNoteManager.Instance;
        if (multiNoteManager == null)
        {
            Debug.LogWarning("[RPC] MultiNoteManager 인스턴스를 찾을 수 없습니다.");
            return;
        }
        // 상대 노트 찾기
        if (multiNoteManager.TryGetNoteByIndex(noteId, out Note_Multi note))
        {
            // 판정에 맞는 처리
            //note.JudgmentSimulateNote(judgement, percent);
            MultiNoteManager.Instance.InsertJudgement(noteId, judgement, keyIndex, percent);
            MultiNoteManager.Instance.InsertScoreData(noteId, percent, curHp, totalScore, combo, missCount, judgement);
        }
        else
        {
            Debug.LogWarning($"[RPC] 해당 noteId({noteId})를 찾을 수 없습니다.");
        }
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_CheckReady(RpcInfo info = default)
    {
        
        // 내가 보낸 메시지는 무시
        if (info.Source == NetworkManager.runnerInstance.LocalPlayer)
        {
            Debug.Log("레디 체크 보냄");
            p1_ready = true;
            if (p1_ready && p2_ready)
            {
                Debug.Log("둘다 레디 체크함");
                RPC_GameStart();
            }

            return;
        }
        else
        {
            p2_ready = true;
            Debug.Log("상대 레디 체크 받음");
        }


        if (p1_ready && p2_ready&& info.Source == NetworkManager.runnerInstance.LocalPlayer)
        {
            Debug.Log("둘다 레디 체크함");
            RPC_GameStart();
        }

    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_GameStart(RpcInfo info = default)
    {
        KGB_GameManager_Multi.Instance.StartGame();
        Debug.Log("게임시작 RPC");

    }




}

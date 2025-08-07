using Fusion;
using UnityEngine;

public class MultPlayManager : NetworkBehaviour
{
    public static MultPlayManager Instance { get; private set; }

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
    public void RPC_SendNoteJudgement(int noteId, string judgement, int keyIndex, float percent, RpcInfo info = default)
    {
        // 내가 보낸 메시지는 무시
        Debug.Log($"[RPC]노트 판정 보냄: noteId={noteId} , judgement={judgement}, key={keyIndex}, percent={percent}");
        if (info.Source == NetworkManager.runnerInstance.LocalPlayer)
        {
            return;
        }

        Debug.Log($"[RPC] 상대의 노트 판정 처리: noteId={noteId} , judgement={judgement}, key={keyIndex}, percent={percent}");
        //판정처리 (플레이어1 이 보내면 플레이어2가 여기 실행 -> 해당값을 가지고 메서드(값)으로 진행)
    }


}

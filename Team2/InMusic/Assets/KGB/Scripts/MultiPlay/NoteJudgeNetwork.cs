using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NoteJudgeNetwork : NetworkBehaviour
{
    public static NoteJudgeNetwork Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RpcSendJudge(int noteId, string judge)
    {
        Debug.Log($"노트 {noteId} 판정: {judge}");
    }

    public void SendJudge(int noteId, string judge)
    {
        RpcSendJudge(noteId, judge);
    }
}

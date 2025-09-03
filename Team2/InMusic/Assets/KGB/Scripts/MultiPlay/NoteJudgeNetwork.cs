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
        Debug.Log($"��Ʈ {noteId} ����: {judge}");
    }

    public void SendJudge(int noteId, string judge)
    {
        RpcSendJudge(noteId, judge);
    }
}

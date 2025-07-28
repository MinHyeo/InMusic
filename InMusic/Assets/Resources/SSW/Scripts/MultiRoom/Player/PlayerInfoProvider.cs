using UnityEngine;
using Steamworks;

public static class PlayerInfoProvider
{
    public static string GetUserId()
    {
#if UNITY_EDITOR
        // ParrelSync 클론 감지 (여러 방법 시도)
        bool isClone = false;
        string cloneId = "";
        
        // 방법 1: EditorPrefs
        if (UnityEditor.EditorPrefs.GetBool("ParrelSync_IsClone"))
        {
            cloneId = UnityEditor.EditorPrefs.GetString("ParrelSyncProjectPath");
            isClone = true;
        }
        
        // 방법 2: 환경 변수
        string envProjectId = System.Environment.GetEnvironmentVariable("PARRELSYNC_PROJECT_ID");
        if (!string.IsNullOrEmpty(envProjectId))
        {
            cloneId = envProjectId;
            isClone = true;
        }
        
        // 방법 3: 프로젝트 경로
        string projectPath = UnityEngine.Application.dataPath;
        if (projectPath.Contains("_clone") || projectPath.Contains("Clone"))
        {
            cloneId = projectPath.GetHashCode().ToString();
            isClone = true;
        }
        
        if (isClone)
        {
            return "EditorClone_" + cloneId.GetHashCode();
        }
        
        return "EditorMain";
#else
        return Steamworks.SteamUser.GetSteamID().ToString();
#endif
    }

    public static string GetUserNickname()
    {
#if UNITY_EDITOR
        // ParrelSync 환경 변수 디버깅
        Debug.Log($"[PlayerInfoProvider] Checking ParrelSync...");
        Debug.Log($"[PlayerInfoProvider] ParrelSync_IsClone: {UnityEditor.EditorPrefs.GetBool("ParrelSync_IsClone")}");
        Debug.Log($"[PlayerInfoProvider] ParrelSyncProjectPath: {UnityEditor.EditorPrefs.GetString("ParrelSyncProjectPath")}");
        Debug.Log($"[PlayerInfoProvider] PARRELSYNC_PROJECT_ID env: {System.Environment.GetEnvironmentVariable("PARRELSYNC_PROJECT_ID")}");
        Debug.Log($"[PlayerInfoProvider] PARRELSYNC_PROJECT_PATH env: {System.Environment.GetEnvironmentVariable("PARRELSYNC_PROJECT_PATH")}");
        
        // 여러 방법으로 클론 감지 시도
        bool isClone = false;
        string cloneId = "";
        
        // 방법 1: EditorPrefs 확인
        if (UnityEditor.EditorPrefs.GetBool("ParrelSync_IsClone"))
        {
            cloneId = UnityEditor.EditorPrefs.GetString("ParrelSyncProjectPath");
            isClone = true;
            Debug.Log($"[PlayerInfoProvider] Clone detected via EditorPrefs: {cloneId}");
        }
        
        // 방법 2: 환경 변수 확인
        string envProjectId = System.Environment.GetEnvironmentVariable("PARRELSYNC_PROJECT_ID");
        if (!string.IsNullOrEmpty(envProjectId))
        {
            cloneId = envProjectId;
            isClone = true;
            Debug.Log($"[PlayerInfoProvider] Clone detected via Environment Variable: {cloneId}");
        }
        
        // 방법 3: 프로젝트 경로로 클론 감지
        string projectPath = UnityEngine.Application.dataPath;
        if (projectPath.Contains("_clone") || projectPath.Contains("Clone"))
        {
            cloneId = projectPath.GetHashCode().ToString();
            isClone = true;
            Debug.Log($"[PlayerInfoProvider] Clone detected via project path: {projectPath}");
        }
        
        if (isClone)
        {
            string cloneName = "Tester_" + cloneId.GetHashCode();
            Debug.Log($"[PlayerInfoProvider] ParrelSync Clone Nickname: {cloneName}");
            return cloneName;
        }
        
        string hostName = "EditorHost";
        Debug.Log($"[PlayerInfoProvider] Editor Host Nickname: {hostName}");
        return hostName;
#else
        try 
        {
            if (SteamManager.Initialized)
            {
                string steamName = SteamFriends.GetPersonaName();
                Debug.Log($"[PlayerInfoProvider] Steam Nickname: {steamName}");
                return string.IsNullOrEmpty(steamName) ? "SteamUser" : steamName;
            }
            else
            {
                Debug.LogWarning("[PlayerInfoProvider] Steam not initialized, using default name");
                return "SteamUser";
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[PlayerInfoProvider] Steam error: {e.Message}");
            return "SteamUser";
        }
#endif
    }
}
using UnityEngine;

public static class PlayerInfoProvider
{
    public static string GetUserId()
    {
#if UNITY_EDITOR
        if (UnityEditor.EditorPrefs.GetBool("ParrelSync_IsClone"))
        {
            string tag = UnityEditor.EditorPrefs.GetString("ParrelSyncProjectPath");
            return "EditorClone_" + tag.GetHashCode();
        }
        return "EditorMain";
#else
        return Steamworks.SteamUser.GetSteamID().ToString();
#endif
    }

    public static string GetUserNickname()
    {
#if UNITY_EDITOR
        if (UnityEditor.EditorPrefs.GetBool("ParrelSync_IsClone"))
        {
            string tag = UnityEditor.EditorPrefs.GetString("ParrelSyncProjectPath");
            string cloneName = "Tester_" + tag.GetHashCode();
            Debug.Log($"[PlayerInfoProvider] ParrelSync Clone Nickname: {cloneName}");
            return cloneName;
        }
        string hostName = "EditorHost";
        Debug.Log($"[PlayerInfoProvider] Editor Host Nickname: {hostName}");
        return hostName;
#else
        try 
        {
            if (Steamworks.SteamManager.Initialized)
            {
                string steamName = Steamworks.SteamFriends.GetPersonaName();
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
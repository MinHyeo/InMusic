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
            return "Tester_" + tag.GetHashCode();
        }
        return "EditorHost";
#else
        return Steamworks.SteamFriends.GetPersonaName();
#endif
    }
}
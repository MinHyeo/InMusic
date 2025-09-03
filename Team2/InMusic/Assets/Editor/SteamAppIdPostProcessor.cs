using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.IO;

public class SteamAppIdPostProcessor
{
    [PostProcessBuild(1)]
    public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
    {
        if (target == BuildTarget.StandaloneWindows64 || target == BuildTarget.StandaloneWindows)
        {
            // �ҽ� ���� ��� (������Ʈ ��Ʈ�� steam_appid.txt)
            string sourceFile = Path.Combine(Application.dataPath, "..", "steam_appid.txt");

            // ������ ��� (����� ���� ���ϰ� ���� ����)
            string buildFolder = Path.GetDirectoryName(pathToBuiltProject);
            string destinationFile = Path.Combine(buildFolder, "steam_appid.txt");

            try
            {
                if (File.Exists(sourceFile))
                {
                    File.Copy(sourceFile, destinationFile, true);
                    Debug.Log($"[SteamAppIdPostProcessor] Copied steam_appid.txt to: {destinationFile}");
                }
                else
                {
                    Debug.LogError($"[SteamAppIdPostProcessor] Source file not found: {sourceFile}");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[SteamAppIdPostProcessor] Failed to copy steam_appid.txt: {e.Message}");
            }
        }
    }
}

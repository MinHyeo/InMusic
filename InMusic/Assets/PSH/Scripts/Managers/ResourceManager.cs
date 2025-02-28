using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Video;

public class ResourceManager
{
    string appPath = Application.dataPath;
    string mDataPath = "Assets/Resources/Music";
    GameObject musicDataRoot;

    public T Load<T>(string path) where T : Object
    {
        if (typeof(T) == typeof(GameObject))
        {
            string name = path;
            int index = name.LastIndexOf('/');
            if (index >= 0)
            {
                name = name.Substring(index + 1);
            }
        }

        return Resources.Load<T>(path);
    }

    public GameObject Instantiate(string path, Transform parent = null)
    {

        GameObject original = Load<GameObject>($"Prefabs/PSH/{path}"); //Prefab
        if (original == null)
        {
            UnityEngine.Debug.Log($"Fail to load Prefab : {path}");
            return null;
        }

        GameObject gameObject = Object.Instantiate(original, parent); //GameObject
        gameObject.name = original.name;

        return gameObject;
    }

    public List<MusicData> GetMusicList() {
        musicDataRoot = new GameObject("MusicDataRoot");

        List<MusicData> mList = new List<MusicData>();
        //��� ����
        string fullPath = appPath + mDataPath.Replace("Assets", "");
        //���� ����
        int min = 17; //�ּҰ�
        int numOfMusic = 0;  //���丮 ���� == ���� ���� ����
        int result;
        //���� �ʿ�
        string[] mTitles = new string[17]; //����(���丮 �̸�)

        if (Directory.Exists(fullPath)) 
        {
            //���� ������ �ƴ� ���丮 ������ ����
            mTitles = Directory.GetDirectories(fullPath);
            numOfMusic = mTitles.Length;
        }
        else
        {
            UnityEngine.Debug.Log("���� ���丮 ����");
            return null;
        }

        result = min > numOfMusic ? min : numOfMusic;

        for (int i = 0; i < result; i++)
        {
            GameObject item = Instantiate("MusicDataBox", musicDataRoot.transform);
            MusicData tmpMusic = item.GetComponent<MusicData>();
            //MusicData tmpMusic = new MusicData();

            //�ְ����� ������ ���� ������ ���� Load ����
            if (i < numOfMusic) {
                //��� ��������
                tmpMusic.DirPath = mTitles[i].Replace("\\","/");
                //���ϵ� �̸� ��������
                string[] files = Directory.GetFiles(mTitles[i])
                                          //LINQ�� (Language-Integrated Query)
                                          .Select(file => file.Replace("\\", "/"))
                                          .Where(file => !file.EndsWith(".meta")) //.meta���� ����
                                          .ToArray();
                Dictionary<string, string> fileMap = FileMapping(files);

                /*for (int j = 0; j < files.Length; j++) {
                    UnityEngine.Debug.Log($"{j}��° ���� �̸�: {files[j]}");*/

                //1. BMS ���� ���� (�ʼ�)
                if (fileMap.ContainsKey("bms")) {
                    tmpMusic.BMS = GameManager_PSH.BMS.ParseBMS(fileMap["bms"]);
                    //UnityEngine.Debug.Log(tmpMusic.Title);
                    tmpMusic.HasBMS = true;
                }

                //else
                //{
                //    tmpMusic.BMS = new BMSData();
                //    tmpMusic.BMS.header.title = $"{i + 1}��° ����";
                //    UnityEngine.Debug.Log(tmpMusic.BMS.header.title);
                //    tmpMusic.BMS.header.artist = $"{i + 1}��° �۰";
                //}

                //2. ���� ���� ���� (�ʼ�)
                if (fileMap.ContainsKey("audio")) { 
                    tmpMusic.Audio = Load<AudioClip>(fileMap["audio"]);
                    float audioLength = tmpMusic.Audio.length;
                    int minutes = Mathf.FloorToInt(audioLength / 60);
                    int seconds = Mathf.FloorToInt(audioLength % 60);
                    tmpMusic.Length = string.Format("{0:00}:{1:00}", minutes, seconds);
                }

                //3. ���� ���� ���� (����)
                if (fileMap.ContainsKey("album"))
                    tmpMusic.Album = Load<Sprite>(fileMap["album"]);
                else //������ ������ �⺻ ���� ���
                    tmpMusic.Album = Load<Sprite>("Default_IMG");

                //4. ��� ���� ���� (����)
                if (fileMap.ContainsKey("log")) {
                    MusicLog tmpLog = LoadLog(fileMap["log"]);
                    tmpMusic.Score = tmpLog.Score;
                    tmpMusic.Accuracy = tmpLog.Accuracy;
                    tmpMusic.Combo = tmpLog.Combo;
                    tmpMusic.Rank = tmpLog.Rank;
                }
                
                //5. �º� ���� ���� (����)
                if (fileMap.ContainsKey("video")){
                    tmpMusic.MuVi = Load<VideoClip>(fileMap["video"]);
                    tmpMusic.HasMV = true;
                }

                UnityEngine.Debug.Log($"{i + 1}��° ���� Ȯ�� �Ϸ�\n");
            }
            //���丮�� ���� ���� Item�� ���� MusicData
            else
            {
                tmpMusic.Album = Load<Sprite>("Default_IMG");
            }

            mList.Add(tmpMusic);
        }
        return mList;
    }

    //���ϰ�ο� Ȯ���� �з�
    Dictionary<string, string> FileMapping(string[] fileList) { 
        Dictionary<string, string> fileMap = new Dictionary<string, string>();

        foreach (string fullFilePath in fileList)
        {
            //Ȯ���ڸ� ��������
            string fileExt = Path.GetExtension(fullFilePath);
            //"Music/�뷡�̸�/�����̸�"������ ��� �����
            string filePath = fullFilePath.Replace(appPath + "/Resources/" , "");
            //Ȯ���� ����
            filePath = filePath.Replace(fileExt , "");
            //UnityEngine.Debug.Log(filePath);

            //BMS
            if (fileExt == ".txt")
                fileMap["bms"] = filePath;
            //���� ����
            else if (fileExt == ".wav" || fileExt == ".mp3" || fileExt == ".ogg")
                fileMap["audio"] = filePath;
            //���� ����
            else if (fileExt == ".png" || fileExt == ".jpg" || fullFilePath == ".jpeg")
                fileMap["album"] = filePath;
            //��� ����
            else if (fileExt == ".json")
                fileMap["log"] = filePath + ".json";
            //�º� ����
            else if (fileExt == ".mp4" || fileExt == ".avi" || fileExt == ".mov")
                fileMap["video"] = filePath;
        }
        return fileMap;
    }

    MusicLog LoadLog(string path)
    {
        Debug.Log(path);
        string jsonText = File.ReadAllText(Path.Combine(appPath + "\\Resources\\", path));
        //������ ��ü�� ���� ����
        return JsonUtility.FromJson<MusicLog>(jsonText);
    }
}
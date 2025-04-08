using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Video;

public class ResourceManager
{
    string appPath = Application.dataPath;
    string mDataPath = "Assets/Resources/Music";
    List<MusicData> musicDataList = new List<MusicData>();

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


    /// <summary>
    /// musicData 1�� ����: ���ϵ� �о musicData List ����
    /// </summary>
    public void CheckMusic()
    {
        Debug.Log("���� ����ȭ �� MusicData  ����");
        //��� ����
        string fullPath = appPath + mDataPath.Replace("Assets", "");
        //���� ����
        int min = 17; //�ּҰ�
        int numOfMusic = 0;  //���丮 ���� == ���� ���� ����
        int result;
        //���� �ʿ�
        string[] musicFolders = new string[17]; //����(���丮 �̸�)

        if (Directory.Exists(fullPath))
        {
            //���� ������ �ƴ� ���丮 ������ ����
            musicFolders = Directory.GetDirectories(fullPath);
            numOfMusic = musicFolders.Length;
        }
        else
        {
            UnityEngine.Debug.Log("���� ���丮 ����");
            return;
        }

        result = min > numOfMusic ? min : numOfMusic;


        for (int i = 0; i < result; i++)
        {
            //��ü(MusicDataBox GameObject) ����
            GameObject dataobject = Instantiate("MusicDataBox", GameManager_PSH.Instance.DataRoot.transform);
             MusicData tmpMusic = dataobject.GetComponent<MusicData>();

            //�ְ����� ������ ���� ������ ���� Load ����
            if (i < numOfMusic)
            {
                //��� ��������
                tmpMusic.DirPath = musicFolders[i].Replace("\\", "/");

                //���� �� ���ϵ� ��������
                string[] files = Directory.GetFiles(musicFolders[i])
                                      .Select(file => file.Replace("\\", "/"))
                                      .Where(file => !file.EndsWith(".meta")) // .meta ���� ����
                                      .ToArray();

                Dictionary<string, string> fileMap = FileMapping(files);

                //1. BMS ���� �б� (�ʼ�)
                if (fileMap.ContainsKey("bms"))
                {
                    tmpMusic.BMS = GameManager_PSH.BMS.ParseBMS(fileMap["bms"]);
                    tmpMusic.HasBMS = true;
                }

                //musicID (����_��Ƽ��Ʈ) ����
                tmpMusic.MusicID = $"{tmpMusic.BMS.header.title}_{tmpMusic.BMS.header.artist}";
                tmpMusic.Title = tmpMusic.BMS.header.title;
                tmpMusic.Artist = tmpMusic.BMS.header.artist;

                //2. ���� ���� ���� (�ʼ�)
                if (fileMap.ContainsKey("audio"))
                {
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


                //4. �º� ���� ���� (����)
                if (fileMap.ContainsKey("video"))
                {
                    tmpMusic.MuVi = Load<VideoClip>(fileMap["video"]);
                    tmpMusic.HasMV = true;
                }
            }
            //���丮�� ���� ���� Item�� ���� MusicData
            else
            {
                tmpMusic.Album = Load<Sprite>("Default_IMG");
            }

            //5. musicDataList�� �߰�
            musicDataList.Add(tmpMusic);

        }

        //6. ���� ���ҽ� ��� ����ȭ
        GameManager_PSH.Web.CheckMusic(musicDataList);
    }

    /// <summary>
    /// musicData 2�� ����: musicData List�� log �κ� ���� |
    /// ������ �α� ���� �д� ��Ŀ��� ���� ������ �д� ������� ����
    /// </summary>
    /// <returns></returns>
    public List<MusicData> GetMusicList() {
        List<MusicLog> logs = GameManager_PSH.Data.getLogDataList();

        for (int i = 0; i < musicDataList.Count - 1; i++)
        {
            //4-1. ��� ���� ���� (����)
            /*if (fileMap.ContainsKey("log")) {
                 MusicLog tmpLog = LoadLog(fileMap["log"]);
                 tmpMusic.Score = tmpLog.Score;
                 tmpMusic.Accuracy = tmpLog.Accuracy;
                 tmpMusic.Combo = tmpLog.Combo;
                 tmpMusic.Rank = tmpLog.Rank;
             }*/
             //4-2. ��� �������� (�α׶� ���� �����ֱ�)
             foreach (MusicLog log in logs)
             {
                if (log.MusicID == musicDataList[i].MusicID)
                {
                    musicDataList[i].LogID = log.LogID;
                    musicDataList[i].MusicID = log.MusicID;
                    //���� ���࿣ ��� ��
                    musicDataList[i].Score = log.Score;
                    musicDataList[i].Accuracy = log.Accuracy;
                    musicDataList[i].Combo = log.Combo;
                    musicDataList[i].Rank = log.Rank;
                }
             }
        }

        return musicDataList;
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
            if (fileExt == ".bms")
                fileMap["bms"] = filePath;
            //���� ����
            else if (fileExt == ".wav" || fileExt == ".mp3" || fileExt == ".ogg")
                fileMap["audio"] = filePath;
            //���� ����
            else if (fileExt == ".png" || fileExt == ".jpg" || fullFilePath == ".jpeg")
                fileMap["album"] = filePath;
            ////��� ����
            //else if (fileExt == ".json")
            //    fileMap["log"] = filePath + ".json";
            //�º� ����
            else if (fileExt == ".mp4" || fileExt == ".avi" || fileExt == ".mov")
                fileMap["video"] = filePath;
        }
        return fileMap;
    }

    //������ �α� ����(json)�о ��ü ����
    MusicLog LoadLog(string path)
    {
        Debug.Log(path);
        string jsonText = File.ReadAllText(Path.Combine(appPath + "\\Resources\\", path));
        //������ ��ü�� ���� ����
        return JsonUtility.FromJson<MusicLog>(jsonText);
    }
}
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using System.IO;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Video;
using System.Linq;

public class ResourceManager
{
    string appPath = Application.dataPath;
    string mDataPath = "Assets/Resources/Music";
    GameObject ItemPool;

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

    MusicLog LoadLog(string path)
    {
        //Json���� �б�
        TextAsset jsonText = Load<TextAsset>(path);
        //������ ��ü�� ���� ����
        return JsonUtility.FromJson<MusicLog>(jsonText.text);
    }

    public List<Music_Item> GetMusicList() {
        ItemPool = new GameObject("ItemPool");

        List<Music_Item> mList = new List<Music_Item>();
        //��� ����
        string fullPath = appPath + mDataPath.Replace("Assets", "");
        //���� ����
        int min = 17; //�ּҰ�
        int numOfMusic = 0;  //���丮 ���� == ���� ���� ����
        int result;
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
            GameObject item = Instantiate("Music", ItemPool.transform);
            Music_Item tmpMusic = item.GetComponent<Music_Item>();

            //�ְ����� ������ ���� ������ ���� Load ����
            if (i < numOfMusic) {
                //���ϵ� �̸� ��������
                string[] files = Directory.GetFiles(mTitles[i])
                                          //LINQ�� (Language-Integrated Query)
                                          .Select(file=> file.Replace("\\", "/"))
                                          .Where(file => !file.EndsWith(".meta")) //.meta���� ����
                                          .ToArray();
                Dictionary<string, string> fileMap = FileMapping(files);
                
                /*for (int j = 0; j < files.Length; j++) {
                    UnityEngine.Debug.Log($"{j}��° ���� �̸�: {files[j]}");
                }*/

                //1. BMS ���� ���� (�ʼ�)
                if (fileMap.ContainsKey("bms")) {
                    //BMSData tmpBMS = BMSManager.Instance.ParseBMS(fileMap["bms"]);
                    //tmpMusic.Title.text = tmpBMS.header.title;
                    //tmpMusic.Artist.text = tmpBMS.header.artist;
                    //tmpMusic.Rank.text = tmpBMS.header.rank.ToString();
                }

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
                    tmpMusic.Album.sprite = Load<Sprite>(fileMap["album"]);
                else //������ ������ �⺻ ���� ���
                    tmpMusic.Album.sprite = Load<Sprite>("Default_IMG");


                //4. ��� ���� ���� (����)
                if (fileMap.ContainsKey("log")) {
                    //TODO
                    MusicLog tmpLog = LoadLog(fileMap["log"]);
                    tmpMusic.Score = tmpLog.Score;
                    tmpMusic.Accuracy = tmpLog.Accuracy;
                    tmpMusic.Rank = tmpMusic.Rank;
                }
                
                //5. �º� ���� ���� (����)
                if (fileMap.ContainsKey("video"))
                    tmpMusic.MuVi = Load<VideoClip>(fileMap["video"]);

                UnityEngine.Debug.Log($"{i + 1}��° ���� Ȯ�� �Ϸ�\n");
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
            if (fileExt == ".bms") 
                fileMap["bms"] = filePath;
            //���� ����
            else if (fileExt == ".wav" || fileExt == ".mp3" || fileExt == ".ogg")
                fileMap["audio"] = filePath;
            //���� ����
            else if (fileExt == ".png" || fileExt == ".jpg" || fullFilePath == ".jpeg")
                fileMap["album"] = filePath;
            //��� ����
            else if (fileExt == ".json")
                fileMap["log"] = filePath;
            //�º� ����
            else if (fileExt == ".mp4" || fileExt == ".avi" || fileExt == ".mov")
                fileMap["video"] = filePath;
        }
        return fileMap;
    }
}
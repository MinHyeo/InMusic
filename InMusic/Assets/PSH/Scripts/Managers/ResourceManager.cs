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
                string[] files = Directory.GetFiles(mTitles[i].Replace("\\", "/"))
                                          .Where(file => !file.EndsWith(".meta")) //.meta���� ����
                                          .ToArray();

                Dictionary<string, string> fileMap = FileMapping(files);


                int numOfFiles = files.Length;
                for (int j = 0; j < numOfFiles; j++) {
                    UnityEngine.Debug.Log($"{j}��° ���� �̸�: {files[j]}");
                }

                BMSData tmpBMS = null;
                
                
                //1. BMS ���� ����
                if (fileMap.ContainsKey("bms")) {
                    //tmpMusic.Title.text = tmpBMS.header.title;
                    //tmpMusic.Artist.text = tmpBMS.header.artist;
                }

                //2. ���� ���� ����
                if (fileMap.ContainsKey("audio")) { 
                    tmpMusic.Audio = LoadAudioClip(fileMap["audio"]);
                }

                //3. ���� ���� ����
                if (fileMap.ContainsKey("album"))
                {
                    tmpMusic.Album.sprite = LoadAlbumArt(fileMap["album"]);
                }
                
                //4. ��� ���� ����
                if (fileMap.ContainsKey("log")) {
                    //files[4];
                }
                
                //5. ������ ���� ����
                if (fileMap.ContainsKey("video"))
                {
                    tmpMusic.MuVi = LoadMusicVideo(fileMap["video"]);
                }

                UnityEngine.Debug.Log($"{i + 1}��° ���� Ȯ�� �Ϸ�");
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
            //"Music/�뷡�̸�/�����̸�"������ ��� �����
            string filePath = fullFilePath.Replace(appPath + "/Resources/" , "");
            //Ȯ���ڸ� ��������
            string fileExt = Path.GetExtension(fullFilePath);

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
            else if (fileExt == ".txt")
                fileMap["log"] = filePath;
            //�º� ����
            else if (fileExt == ".mp4" || fileExt == ".avi" || fileExt == ".mov")
                fileMap["video"] = filePath;
        }
        return fileMap;
    }

    #region FileLoader
    //BMS ���� Load
    public BMSData LoadBMS(string path) {
        return BMSManager.Instance.ParseBMS(path);
    }

    //���� ���� Load
    public AudioClip LoadAudioClip(string path)
    {
        return Load<AudioClip>(path);
    }

    //�ٹ� ���� Load
    public Sprite LoadAlbumArt(string path) {
        return Load<Sprite>(path);
    }

    //�º� Load
    public VideoClip LoadMusicVideo(string path) {
        return Load<VideoClip>(path);
    }
    #endregion
}
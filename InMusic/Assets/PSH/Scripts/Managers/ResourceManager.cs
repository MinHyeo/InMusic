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
                //5���� ���ϵ� �̸� ��������
                string[] files = Directory.GetFiles(mTitles[i].Replace("\\", "/"))
                                          .Where(file => !file.EndsWith(".meta")) //.meta���� ����
                                          .ToArray();

                for (int j = 0; j < files.Length; j++) {
                    UnityEngine.Debug.Log($"���� �̸�: {files[j]}");
                }
                BMSData tmpBMS = null;
                /*
                //1. BMS ���� ����
                if ((tmpBMS = LoadBMS(files[0]))!= null) {
                    tmpMusic.Title.text = tmpBMS.header.title;
                    tmpMusic.Artist.text = tmpBMS.header.artist;
                }
                //2. ���� ���� ����
                if (LoadAudioClip(files[1])!= null) { 
                    tmpMusic.Audio = LoadAudioClip(files[1]);
                }
                //3. ���� ���� ����
                if (LoadAlbumArt(files[2]) != null)
                {
                    tmpMusic.Album.sprite = LoadAlbumArt(files[2]);
                }
                //4. ������ ���� ����
                if (LoadMusicVideo(files[3]) != null)
                {
                    tmpMusic.MuVi = LoadMusicVideo(files[3]);
                }
                //5. ��� ���� ����
                if (false) {
                    //files[4];
                }*/

                UnityEngine.Debug.Log($"{i + 1}��° ���� Ȯ�� �Ϸ�");
            }
            mList.Add(tmpMusic);
        }
        return mList;
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
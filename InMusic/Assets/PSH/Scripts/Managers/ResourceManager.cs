using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using System.IO;
using System.Diagnostics;
using UnityEngine;

public class ResourceManager
{
    string mDataPath = "Assets/Resources/Music";

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

    public List<string> GetMusicList() {
        List<Music_Item> mList = new List<Music_Item>();
        //��� ����
        string fullPath = Application.dataPath+ mDataPath.Replace("Assets", "");
        //���� ����(���丮 ����), ����(���丮 �̸�)
        int numOfmusic = 17;
        string[] mTitles = new string[17];

        if (Directory.Exists(fullPath)) 
        {
            mTitles = Directory.GetDirectories(fullPath);
            numOfmusic = mTitles.Length;
        }
        else
        {
            UnityEngine.Debug.Log("���� ���丮 ���� ����");
            return null;
        }

        
        /*for (int i = 0; i < numOfmusic; i++)
        {
            string mPath = fullPath + mTitles[i];
            //���� ���丮 ����
            Process.Start("explorer.exe", mPath);
            string[] files = Directory.GetFiles(fullPath);
            //1. BMS ���� ����
            //2. �ٹ� ���� ����
            //3. �º� ����
            //4. ��� ���� ����
            Music_Item temp = new Music_Item();

            mList.Add(temp);
        }*/
        

        //string ��� Music_Item ��ü ���
        List<string> list = new List<string>();
        //���� ��� ��������(���� �б�)
        for (int tmp = 0; tmp < 17; tmp++) {
            list.Add($"Title { tmp + 1 }");
        }
        return list;
    }
}
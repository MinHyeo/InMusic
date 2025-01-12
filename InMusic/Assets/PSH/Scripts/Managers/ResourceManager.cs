using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ResourceManager
{

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
            Debug.Log($"Fail to load Prefab : {path}");
            return null;
        }

        GameObject gameObject = Object.Instantiate(original, parent); //GameObject
        gameObject.name = original.name;

        return gameObject;
    }

    public List<string> GetMusicList() {
        //string ��� Music_Item ��ü ���
        List<string> list = new List<string>();
        //���� ��� ��������(���� �б�)
        for (int tmp = 0; tmp < 17; tmp++) {
            list.Add($"Title { tmp + 1 }");
        }
        return list;
    }
}
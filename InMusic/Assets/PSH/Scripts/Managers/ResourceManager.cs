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
        //경로 설정
        string fullPath = appPath + mDataPath.Replace("Assets", "");
        //음악 개수
        int min = 17; //최소값
        int numOfMusic = 0;  //디렉토리 개수 == 음악 폴더 개수
        int result;
        string[] mTitles = new string[17]; //제목(디렉토리 이름)

        if (Directory.Exists(fullPath)) 
        {
            //파일 갯수가 아닌 디렉토리 갯수를 구함
            mTitles = Directory.GetDirectories(fullPath);
            numOfMusic = mTitles.Length;
        }
        else
        {
            UnityEngine.Debug.Log("음악 디렉토리 없음");
            return null;
        }

        result = min > numOfMusic ? min : numOfMusic;
        for (int i = 0; i < result; i++)
        {
            GameObject item = Instantiate("Music", ItemPool.transform);
            Music_Item tmpMusic = item.GetComponent<Music_Item>();

            //최값보다 음악의 수가 적으면 파일 Load 안함
            if (i < numOfMusic) {
                //5개의 파일들 이름 가져오기
                string[] files = Directory.GetFiles(mTitles[i].Replace("\\", "/"))
                                          .Where(file => !file.EndsWith(".meta")) //.meta파일 제외
                                          .ToArray();

                for (int j = 0; j < files.Length; j++) {
                    UnityEngine.Debug.Log($"파일 이름: {files[j]}");
                }
                BMSData tmpBMS = null;
                /*
                //1. BMS 파일 열기
                if ((tmpBMS = LoadBMS(files[0]))!= null) {
                    tmpMusic.Title.text = tmpBMS.header.title;
                    tmpMusic.Artist.text = tmpBMS.header.artist;
                }
                //2. 음원 파일 열기
                if (LoadAudioClip(files[1])!= null) { 
                    tmpMusic.Audio = LoadAudioClip(files[1]);
                }
                //3. 엘범 사진 열기
                if (LoadAlbumArt(files[2]) != null)
                {
                    tmpMusic.Album.sprite = LoadAlbumArt(files[2]);
                }
                //4. 동영상 파일 열기
                if (LoadMusicVideo(files[3]) != null)
                {
                    tmpMusic.MuVi = LoadMusicVideo(files[3]);
                }
                //5. 기록 파일 열기
                if (false) {
                    //files[4];
                }*/

                UnityEngine.Debug.Log($"{i + 1}번째 폴더 확인 완료");
            }
            mList.Add(tmpMusic);
        }
        return mList;
    }

    #region FileLoader
    //BMS 파일 Load
    public BMSData LoadBMS(string path) {
        return BMSManager.Instance.ParseBMS(path);
    }

    //음원 파일 Load
    public AudioClip LoadAudioClip(string path)
    {
        return Load<AudioClip>(path);
    }

    //앨범 사진 Load
    public Sprite LoadAlbumArt(string path) {
        return Load<Sprite>(path);
    }

    //뮤비 Load
    public VideoClip LoadMusicVideo(string path) {
        return Load<VideoClip>(path);
    }
    #endregion
}
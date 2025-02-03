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

    MusicLog LoadLog(string path)
    {
        //Json파일 읽기
        TextAsset jsonText = Load<TextAsset>(path);
        //내용을 객체로 만들어서 전달
        return JsonUtility.FromJson<MusicLog>(jsonText.text);
    }

    public List<MusicData> GetMusicList() {
        musicDataRoot = new GameObject("MusicDataRoot");

        List<MusicData> mList = new List<MusicData>();
        //경로 설정
        string fullPath = appPath + mDataPath.Replace("Assets", "");
        //음악 개수
        int min = 17; //최소값
        int numOfMusic = 0;  //디렉토리 개수 == 음악 폴더 개수
        int result;
        //수정 필요
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
            GameObject item = Instantiate("MusicDataBox", musicDataRoot.transform);
            MusicData tmpMusic = item.GetComponent<MusicData>();

            /*//BMS내용 임시
            {
                tmpMusic.Title = $"{i + 1} 제목";
                tmpMusic.Artist = $"{i + 1} 작곡가";
                tmpMusic.Rank = "-";
            }*/


            //최값보다 음악의 수가 적으면 파일 Load 안함
            if (i < numOfMusic) {
                //경로 가져오기
                tmpMusic.DirPath = mTitles[i].Replace("\\","/");
                //파일들 이름 가져오기
                string[] files = Directory.GetFiles(mTitles[i])
                                          //LINQ문 (Language-Integrated Query)
                                          .Select(file => file.Replace("\\", "/"))
                                          .Where(file => !file.EndsWith(".meta")) //.meta파일 제외
                                          .ToArray();
                Dictionary<string, string> fileMap = FileMapping(files);

                /*for (int j = 0; j < files.Length; j++) {
                    UnityEngine.Debug.Log($"{j}번째 파일 이름: {files[j]}");
                }*/

                //1. BMS 파일 열기 (필수)
                if (fileMap.ContainsKey("bms")) {
                    tmpMusic.BMS = GameManager_PSH.BMS.ParseBMS(fileMap["bms"]);
                    UnityEngine.Debug.Log(tmpMusic.Title);
                    //UnityEngine.Debug.Log("BMS 파일 찾음");
                    tmpMusic.HasBMS = true;
                }
                //else
                //{
                //    tmpMusic.BMS = new BMSData();
                //    tmpMusic.BMS.header.title = $"{i + 1}번째 음악";
                //    UnityEngine.Debug.Log(tmpMusic.BMS.header.title);
                //    tmpMusic.BMS.header.artist = $"{i + 1}번째 작곡가";
                //}

                //2. 음원 파일 열기 (필수)
                if (fileMap.ContainsKey("audio")) { 
                    tmpMusic.Audio = Load<AudioClip>(fileMap["audio"]);
                    float audioLength = tmpMusic.Audio.length;
                    int minutes = Mathf.FloorToInt(audioLength / 60);
                    int seconds = Mathf.FloorToInt(audioLength % 60);
                    tmpMusic.Length = string.Format("{0:00}:{1:00}", minutes, seconds);
                }

                //3. 엘범 사진 열기 (선택)
                if (fileMap.ContainsKey("album"))
                    tmpMusic.Album = Load<Sprite>(fileMap["album"]);
                else //사진이 없으면 기본 사진 사용
                    tmpMusic.Album = Load<Sprite>("Default_IMG");


                //4. 기록 파일 열기 (선택)
                if (fileMap.ContainsKey("log")) {
                    //TODO
                    MusicLog tmpLog = LoadLog(fileMap["log"]);
                    tmpMusic.Score = tmpLog.Score;
                    tmpMusic.Accuracy = tmpLog.Accuracy;
                    tmpMusic.Rank = tmpMusic.Rank;
                }
                
                //5. 뮤비 파일 열기 (선택)
                if (fileMap.ContainsKey("video"))
                    tmpMusic.MuVi = Load<VideoClip>(fileMap["video"]);

                UnityEngine.Debug.Log($"{i + 1}번째 폴더 확인 완료\n");
            }
            mList.Add(tmpMusic);
        }
        return mList;
    }
    //파일경로와 확장자 분류
    Dictionary<string, string> FileMapping(string[] fileList) { 
        Dictionary<string, string> fileMap = new Dictionary<string, string>();

        foreach (string fullFilePath in fileList)
        {
            //확장자만 가져오기
            string fileExt = Path.GetExtension(fullFilePath);
            //"Music/노래이름/파일이름"까지만 경로 남기기
            string filePath = fullFilePath.Replace(appPath + "/Resources/" , "");
            //확장자 제거
            filePath = filePath.Replace(fileExt , "");
            //UnityEngine.Debug.Log(filePath);

            //BMS
            if (fileExt == ".txt") 
                fileMap["bms"] = filePath;
            //음원 파일
            else if (fileExt == ".wav" || fileExt == ".mp3" || fileExt == ".ogg")
                fileMap["audio"] = filePath;
            //엘범 사진
            else if (fileExt == ".png" || fileExt == ".jpg" || fullFilePath == ".jpeg")
                fileMap["album"] = filePath;
            //기록 파일
            else if (fileExt == ".json")
                fileMap["log"] = filePath;
            //뮤비 파일
            else if (fileExt == ".mp4" || fileExt == ".avi" || fileExt == ".mov")
                fileMap["video"] = filePath;
        }
        return fileMap;
    }
}
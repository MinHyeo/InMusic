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
    /// musicData 1차 가공: 파일들 읽어서 musicData List 생성
    /// </summary>
    public void CheckMusic()
    {
        Debug.Log("음악 동기화 및 MusicData  생성");
        //경로 설정
        string fullPath = appPath + mDataPath.Replace("Assets", "");
        //음악 개수
        int min = 17; //최소값
        int numOfMusic = 0;  //디렉토리 개수 == 음악 폴더 개수
        int result;
        //수정 필요
        string[] musicFolders = new string[17]; //제목(디렉토리 이름)

        if (Directory.Exists(fullPath))
        {
            //파일 갯수가 아닌 디렉토리 갯수를 구함
            musicFolders = Directory.GetDirectories(fullPath);
            numOfMusic = musicFolders.Length;
        }
        else
        {
            UnityEngine.Debug.Log("음악 디렉토리 없음");
            return;
        }

        result = min > numOfMusic ? min : numOfMusic;


        for (int i = 0; i < result; i++)
        {
            //객체(MusicDataBox GameObject) 생성
            GameObject dataobject = Instantiate("MusicDataBox", GameManager_PSH.Instance.DataRoot.transform);
             MusicData tmpMusic = dataobject.GetComponent<MusicData>();

            //최값보다 음악의 수가 적으면 파일 Load 안함
            if (i < numOfMusic)
            {
                //경로 가져오기
                tmpMusic.DirPath = musicFolders[i].Replace("\\", "/");

                //폴더 내 파일들 가져오기
                string[] files = Directory.GetFiles(musicFolders[i])
                                      .Select(file => file.Replace("\\", "/"))
                                      .Where(file => !file.EndsWith(".meta")) // .meta 파일 제외
                                      .ToArray();

                Dictionary<string, string> fileMap = FileMapping(files);

                //1. BMS 파일 읽기 (필수)
                if (fileMap.ContainsKey("bms"))
                {
                    tmpMusic.BMS = GameManager_PSH.BMS.ParseBMS(fileMap["bms"]);
                    tmpMusic.HasBMS = true;
                }

                //musicID (제목_아티스트) 설정
                tmpMusic.MusicID = $"{tmpMusic.BMS.header.title}_{tmpMusic.BMS.header.artist}";
                tmpMusic.Title = tmpMusic.BMS.header.title;
                tmpMusic.Artist = tmpMusic.BMS.header.artist;

                //2. 음원 파일 열기 (필수)
                if (fileMap.ContainsKey("audio"))
                {
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


                //4. 뮤비 파일 열기 (선택)
                if (fileMap.ContainsKey("video"))
                {
                    tmpMusic.MuVi = Load<VideoClip>(fileMap["video"]);
                    tmpMusic.HasMV = true;
                }
            }
            //디렉토리가 없는 더미 Item이 읽을 MusicData
            else
            {
                tmpMusic.Album = Load<Sprite>("Default_IMG");
            }

            //5. musicDataList에 추가
            musicDataList.Add(tmpMusic);

        }

        //6. 음악 리소스 목록 동기화
        GameManager_PSH.Web.CheckMusic(musicDataList);
    }

    /// <summary>
    /// musicData 2차 가공: musicData List의 log 부분 가공 |
    /// 기존의 로그 파일 읽는 방식에서 서버 데이터 읽는 방식으로 수정
    /// </summary>
    /// <returns></returns>
    public List<MusicData> GetMusicList() {
        List<MusicLog> logs = GameManager_PSH.Data.getLogDataList();

        for (int i = 0; i < musicDataList.Count - 1; i++)
        {
            //4-1. 기록 파일 열기 (선택)
            /*if (fileMap.ContainsKey("log")) {
                 MusicLog tmpLog = LoadLog(fileMap["log"]);
                 tmpMusic.Score = tmpLog.Score;
                 tmpMusic.Accuracy = tmpLog.Accuracy;
                 tmpMusic.Combo = tmpLog.Combo;
                 tmpMusic.Rank = tmpLog.Rank;
             }*/
             //4-2. 기록 가져오기 (로그랑 음악 맞춰주기)
             foreach (MusicLog log in logs)
             {
                if (log.MusicID == musicDataList[i].MusicID)
                {
                    musicDataList[i].LogID = log.LogID;
                    musicDataList[i].MusicID = log.MusicID;
                    //게임 진행엔 없어도 됨
                    musicDataList[i].Score = log.Score;
                    musicDataList[i].Accuracy = log.Accuracy;
                    musicDataList[i].Combo = log.Combo;
                    musicDataList[i].Rank = log.Rank;
                }
             }
        }

        return musicDataList;
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
            if (fileExt == ".bms")
                fileMap["bms"] = filePath;
            //음원 파일
            else if (fileExt == ".wav" || fileExt == ".mp3" || fileExt == ".ogg")
                fileMap["audio"] = filePath;
            //엘범 사진
            else if (fileExt == ".png" || fileExt == ".jpg" || fullFilePath == ".jpeg")
                fileMap["album"] = filePath;
            ////기록 파일
            //else if (fileExt == ".json")
            //    fileMap["log"] = filePath + ".json";
            //뮤비 파일
            else if (fileExt == ".mp4" || fileExt == ".avi" || fileExt == ".mov")
                fileMap["video"] = filePath;
        }
        return fileMap;
    }

    //기존의 로그 파일(json)읽어서 객체 생성
    MusicLog LoadLog(string path)
    {
        Debug.Log(path);
        string jsonText = File.ReadAllText(Path.Combine(appPath + "\\Resources\\", path));
        //내용을 객체로 만들어서 전달
        return JsonUtility.FromJson<MusicLog>(jsonText);
    }
}
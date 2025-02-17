using UnityEngine;
using Play;
using System;
using System.IO;
using System.Collections.Generic;

/// <summary>
/// 곡 플레이 정보 저장: 곡이름 - 스코어 쌍
/// </summary>
[Serializable]
public class SongScorePair
{
    public string songName;
    public ScoreData scoreData;
}

/// <summary>
/// 여러 곡의 ScoreData를 한 번에 담는 래퍼(Wrapper) 클래스
/// </summary>
[Serializable]
public class ScoreDataCollection
{
    public List<SongScorePair> allSongScores = new List<SongScorePair>();
}

public class SavePlayData : MonoBehaviour
{
    private const string SCORE_FILE_NAME = "scores.json";
    private ScoreDataCollection allScores = null;

    private void Awake()
    {
        LoadScores();
    }

    /// <summary>
    /// 실제 파일 경로를 반환
    /// </summary>
    private string GetSaveFilePath()
    {
        return Path.Combine(Application.persistentDataPath, SCORE_FILE_NAME);
    }

    /// <summary>
    /// scores.json 파일에서 데이터를 읽어와 allScores에 반영
    /// </summary>
    public void LoadScores()
    {
        if (allScores != null)
            return; // 이미 로딩됨

        string path = GetSaveFilePath();
        if (!File.Exists(path))
        {
            // 파일이 없으면 빈 리스트 생성
            allScores = new ScoreDataCollection();
            return;
        }

        try
        {
            string json = File.ReadAllText(path);
            allScores = JsonUtility.FromJson<ScoreDataCollection>(json);

            if (allScores == null)
                allScores = new ScoreDataCollection();
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[SavePlayData] LoadScores Error: {e.Message}");
            allScores = new ScoreDataCollection();
        }
    }

    /// <summary>
    /// allScores를 JSON 문자열로 변환하여 scores.json 파일로 저장
    /// </summary>
    public void SaveScores()
    {
        string path = GetSaveFilePath();
        try
        {
            string json = JsonUtility.ToJson(allScores, prettyPrint: true);
            File.WriteAllText(path, json);
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[SavePlayData] SaveScores Error: {e.Message}");
        }
    }

    /// <summary>
    /// 특정 곡(scoreData.songName)의 기록을 업데이트하고 저장
    /// </summary>
    public void SaveSongScore(ScoreData newScore)
    {
        if (allScores == null)
            LoadScores();

        // 곡 이름 기준으로 검색
        SongScorePair existing = allScores.allSongScores
            .Find(pair => pair.songName == newScore.songName);

        if (existing != null)
        {
            if (newScore.score > existing.scoreData.score)
            {
                existing.scoreData = newScore;
            }
        }
        else
        {
            // 해당 곡이 최초 저장이므로 바로 추가
            allScores.allSongScores.Add(
                new SongScorePair
                {
                    songName = newScore.songName,
                    scoreData = newScore
                }
            );
        }

        // 전체를 다시 파일로 저장(= 기존 곡들도 유지)
        SaveScores();
    }

    /// <summary>
    /// 특정 곡 이름으로 저장된 ScoreData 가져오기
    /// </summary>
    public ScoreData GetSongScore(string songName)
    {
        if (allScores == null)
            LoadScores();

        SongScorePair pair = allScores.allSongScores
            .Find(p => p.songName == songName);

        return pair != null ? pair.scoreData : null;
    }

    /// <summary>
    /// 모든 곡의 스코어 목록을 반환 (곡 리스트 화면 등에 사용)
    /// </summary>
    public List<SongScorePair> GetAllScores()
    {
        if (allScores == null)
            LoadScores();
        return allScores.allSongScores;
    }
    // 필요한 정보: 랭크, 정확도, 점수
}
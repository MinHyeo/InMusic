using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SongTitleList", menuName = "Game/Song Title List")]
public class SongTitleList : ScriptableObject
{
    [Header("Available Song Titles")]
    [SerializeField] private List<string> songTitles = new List<string>();
    
    /// <summary>
    /// 모든 곡 제목 목록을 반환
    /// </summary>
    public List<string> GetAllSongTitles()
    {
        return new List<string>(songTitles); // 복사본 반환으로 안전성 확보
    }
    
    /// <summary>
    /// 곡 개수 반환
    /// </summary>
    public int GetSongCount()
    {
        return songTitles.Count;
    }
    
    /// <summary>
    /// 특정 인덱스의 곡 제목 반환
    /// </summary>
    public string GetSongTitle(int index)
    {
        if (index >= 0 && index < songTitles.Count)
            return songTitles[index];
        return null;
    }
    
    /// <summary>
    /// 곡 제목이 목록에 있는지 확인
    /// </summary>
    public bool ContainsSong(string songTitle)
    {
        return songTitles.Contains(songTitle);
    }
}

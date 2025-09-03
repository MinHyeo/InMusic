using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// DB의 music 테이블과 통신용 음악 정보 객체
/// </summary>
[Serializable]
public class MusicDB
{
    [Header("DB의 music 테이블 구조")]
    [SerializeField] private string musicID;
    [SerializeField] private string musicName;
    [SerializeField] private string musicArtist;

    public MusicDB(string mID, string mName, string mArtist) {
        musicID = mID;
        musicName = mName;
        musicArtist = mArtist;
    }
}

[Serializable]
public class MusicDBList
{
    public List<MusicDB> musics;
}
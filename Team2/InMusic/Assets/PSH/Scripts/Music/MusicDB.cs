using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// DB�� music ���̺�� ��ſ� ���� ���� ��ü
/// </summary>
[Serializable]
public class MusicDB
{
    [Header("DB�� music ���̺� ����")]
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
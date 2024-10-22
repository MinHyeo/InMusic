using Microsoft.Unity.VisualStudio.Editor;
using System.IO;
using UnityEngine;
using UnityEngine.Video;

public enum Song 
{
    ���¾��°ǵ������̴ϱ� = 0,
    ù��������ȹ��ε����ʾ� = 1,
    Ŭ���� = 2,
    Heya = 3,
    Armageddon = 4,
    BubbleGum = 5,
    HotSweet = 6,
    Magentic = 7,
    Sticky = 8,
    Supernova = 9,
}

public class SongInfo : MonoBehaviour
{
    public string part;
    public string Title;
    public string Artist;
    public string Genre;
    public float BPM;
    public float PlayLevel;
    public float Rank;
    public string NoteCount;
}

public class SongInfoLoading : MonoBehaviour
{
    public string[] songList;       // �� �̸� ����Ʈ
    public int index;               // ���õ� �� �ε���

    private FileInfo fileName = null;
    private StreamReader reader = null;
    private string path;            // �� ���� path
    private string StrText;         // ���� �� �پ� �о��� �� ����� ����
    private string songName;        // �� ����

    private char[] seps;            // ������ ������ �迭
    private string[] tempSplit;     // �����ڷ� ���� ���ڿ��� ������ �ӽ� ���ڿ� �迭
    private string tempStr;         // �����ڷ� ���� ���ڿ��� ������ ����

    private void Start()
    {
        SelectSong(Song.Heya);
    }

    public void SelectSong(Song song)
    {
        this.index = (int)song;  //�ӽ� ��
        tempSplit = null;
        tempStr = "";
        StrText = "";
        songName = "";
        path = "Assets/Resources/Song/";
        seps = new char[] { ' ', ':' };

        songName = songList[index];
        path += songName + "/";
        fileName = new FileInfo(path + songList[index] + ".bms");

        if (fileName != null)
        {
            reader = fileName.OpenText();
        }
        else
        {
            Debug.Log("������ �����ϴ�.");
        }

        ParseBMS();
    }

    public SongInfo ParseBMS()
    {
        SongInfo bmsData = new SongInfo();

        while ((tempStr = reader.ReadLine()) != null)
        {
            string trimmedLine = tempStr.Trim();

            // BMS ������ ��� ó��
            if (trimmedLine.StartsWith("#TITLE"))
            {
                bmsData.Title = trimmedLine.Split(' ')[1];
            }
            else if (trimmedLine.StartsWith("#GENRE"))
            {
                bmsData.Genre = trimmedLine.Split(' ')[1];
            }
            else if (trimmedLine.StartsWith("#ARTIST"))
            {
                bmsData.Artist = trimmedLine.Split(' ')[1];
            }
            else if (trimmedLine.StartsWith("#BPM"))
            {
                bmsData.BPM = float.Parse(trimmedLine.Split(' ')[1]);
            }
            else if (trimmedLine.StartsWith("#PLAYLEVEL"))
            {
                bmsData.PlayLevel = float.Parse(trimmedLine.Split(' ')[1]);
            }
            else if (trimmedLine.StartsWith("#RANK"))
            {
                bmsData.Rank = float.Parse(trimmedLine.Split(' ')[1]);
            }
        }

        return bmsData;
    }
}

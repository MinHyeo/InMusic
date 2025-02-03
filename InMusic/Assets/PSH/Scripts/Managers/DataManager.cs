using System.IO;

public class DataManager
{
    MusicData mData;
    
    public void WriteData(MusicLog newLog)
    {
        //파일 이름 가져오기
        string fileName = Path.GetFileNameWithoutExtension(mData.DirPath) + "Log";
        //저장하기
    }

    public void SendData(Music_Item item) {
        mData.DirPath = item.DirPath;
        mData.BMS = item.Data.BMS;
        mData.Album = item.Album.sprite;
        mData.Audio = item.Audio;
        mData.MuVi = item.MuVi;
    }
}

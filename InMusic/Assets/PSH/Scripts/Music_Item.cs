using UnityEngine;
using UnityEngine.UI;

public class Music_Item : MonoBehaviour
{
    //움악 정보
    [SerializeField] private Text title;
    [SerializeField] private Text artist;
    [SerializeField] private string length;
    [SerializeField] private Image albumArt;
    [SerializeField] private Image back;
    //기록 정보
    [SerializeField] private Text rank;
    [SerializeField] private string score;
    [SerializeField] private string accuracy;
    #region Getter
    public Text Title { get { return title; } set { title = value; } }
    public Text Artist { get { return artist; } set { artist = value; } }
    public string Length { get { return length; } set { length = value; } }
    public Image Album { get { return albumArt; } set { albumArt = value; } }
    public Text Rank { get { return rank; } set { rank = value; } }
    public string Score { get { return score; } set { score = value; } }
    public string Accuracy { get { return accuracy; } set { accuracy = value; } }
    
       
    #endregion

    void Start()
    {
        title.text = "Title";
        artist.text = "Artist";
        length= "00:00";
        rank.text = "-";
    }

    public void ItemSelect()
    {
        /*Color Temp = Title.color;
        Temp.a = 0.5f; 
        Title.color = Temp;
        Temp = Artist.color;

        Artist.color.a = 255.0f;
        Rank.color.a = 255.0f;
        AlbumArt.color.a = 255.0f;
        Back.color.a = 255.0f;*/
    }

    public void ItemUnselect()
    {

    }

    public void SetData(string newtitle = "Title", string newartist = "Artist", string newrank = "-") {
        title.text = newtitle;
        artist.text = newartist;
        rank.text = newrank;
    }
}

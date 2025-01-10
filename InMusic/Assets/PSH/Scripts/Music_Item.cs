using UnityEngine;
using UnityEngine.UI;

public class Music_Item : MonoBehaviour
{
    public Text title;
    public Text artist;
    public Text length;
    public Text rank;
    public Image albumArt;
    public Image back;

    #region Getter
    public Text Title { get { return title; } set { title = value; } }
    public Text Artist { get { return artist; } set { artist = value; } }
    public Text Length { get { return length; } set { length = value; } }
    public Image Album { get { return albumArt; } set { albumArt = value; } }
    public Text Rank { get { return rank; } set { rank = value; } }     
       
    #endregion

    void Start()
    {
        title.text = "Title";
        artist.text = "Artist";
        length = new GameObject("Length").AddComponent<Text>();
        length.text = "00:00";
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

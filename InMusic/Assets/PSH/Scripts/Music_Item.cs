using UnityEngine;
using UnityEngine.UI;

public class Music_Item : MonoBehaviour
{
    public Text Title;
    public Text Artist;
    public Text Rank;
    public Image AlbumArt;
    public Image Back;
    void Start()
    {
        Title.text = "Title";
        Artist.text = "Artist";
        Rank.text = "-";
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

    public void SetData(string title = "Title", string artist = "Artist", string rank = "-") {
        Title.text = title;
        Artist.text = artist;
        Rank.text = rank;
    }
}

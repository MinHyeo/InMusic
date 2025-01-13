using UnityEngine;
using UnityEngine.UI;

public class Music_Item : MonoBehaviour
{
    //배경(?)
    [SerializeField] private Image background;
    [Header("음악 관련 정보")]
    [SerializeField] private Text title;
    [SerializeField] private Text artist;
    [SerializeField] private string length;
    [SerializeField] private Image albumArt;
    [SerializeField] private Image back;
    [SerializeField] private Color wMint = new Color(104.0f, 240.0f, 235.0f, 0.08f);
    [SerializeField] private Color sMint = new Color(57, 255, 255, 1.0f);
    [SerializeField] private Color purple = new Color(155.0f, 48.0f, 255.0f, 1.0f);
    [Header("기록 관련 정보")]
    [SerializeField] private string score;
    [SerializeField] private string accuracy;
    [SerializeField] private string combo;
    [SerializeField] private Text rank;
    
    
    #region Get/Set
    public Text Title { get { return title; } set { title = value; } }
    public Text Artist { get { return artist; } set { artist = value; } }
    public string Length { get { return length; } set { length = value; } }
    public Image Album { get { return albumArt; } set { albumArt = value; } }
    public string Score { get { return score; } set { score = value; } }
    public string Accuracy { get { return accuracy; } set { accuracy = value; } }
    public string Combo { get { return combo; } set { combo = value; } }
    public Text Rank { get { return rank; } set { rank = value; } }
    #endregion

    void Start()
    {
        title.text = "Title";
        artist.text = "Artist";
        rank.text = "-";
    }

    public void SetData(string newtitle = "Title", string newartist = "Artist", string newrank = "-", Image newAlbum = null, string newscore = "0", string newaccuracy = "0", string newcombo = "0")
    {
        title.text = newtitle;
        artist.text = newartist;
        rank.text = newrank;
        //albumArt = newAlbum;
        score = newscore;
        accuracy = newaccuracy;
        combo = newcombo;
    }


    public void ItemSelect()
    {
        
        SetColor(background, purple);
        SetColor(back, purple);
        
        SetAlpha(title,1.0f);
        SetAlpha(artist,1.0f);
        SetAlpha(rank,1.0f);
        
        SetAlpha(albumArt, 1.0f);
    }

    public void ItemUnselect()
    {
        SetColor(background, wMint);
        SetColor(back, wMint);
        
        SetAlpha(title, 0.5f);
        SetAlpha(artist, 0.5f);
        SetAlpha(rank, 0.5f);
        
        SetAlpha(albumArt, 0.5f);
    }

    //투명도만 바꾸기
    void SetAlpha(Graphic target, float alpha) {
        Color Temp = target.color;
        Temp.a = alpha;
        target.color = Temp;
    }

    //색상 바꾸기
    void SetColor(Graphic target, Color color) {
        target.color = color;
    }

}

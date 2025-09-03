using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LodingUI : MonoBehaviour
{
    public TextMeshProUGUI artistText;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI loadingRate;
    public Image musicImage;
    public Slider loadingBar;
    public RectTransform loadingTextRect; // 퍼센트 텍스트 위치 조절용 RectTransform

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateTextPosition(float progress)
    {
        float newX = Mathf.Lerp(-600f, 650f, progress);
        loadingTextRect.anchoredPosition = new Vector2(newX, loadingTextRect.anchoredPosition.y);
    }
}

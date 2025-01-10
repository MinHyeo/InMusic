using TMPro;
using UnityEngine;

public class SinglePlayResultUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI score;
    [SerializeField] private TextMeshProUGUI great;
    [SerializeField] private TextMeshProUGUI good;
    [SerializeField] private TextMeshProUGUI bad;
    [SerializeField] private TextMeshProUGUI miss;
    [SerializeField] private TextMeshProUGUI accuracy;
    [SerializeField] private TextMeshProUGUI combo;
    [SerializeField] private TextMeshProUGUI rate;




    void Start()
    {
        
    }

    
    void Update()
    {
        
    }

    public void UpdateResult()
    {
        score.text = GameManager.Instance.totalScore.ToString();
        great.text = GameManager.Instance.greatCount.ToString();
        good.text = GameManager.Instance.goodCount.ToString();
        bad.text = GameManager.Instance.badCount.ToString();
        miss.text = GameManager.Instance.missCount.ToString();
        accuracy.text = GameManager.Instance.accuracy.ToString();
        combo.text = GameManager.Instance.combo.ToString();
        
    }
}

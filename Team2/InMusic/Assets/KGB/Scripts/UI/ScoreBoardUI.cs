using TMPro;
using UnityEngine;

public class ScoreBoardUI : MonoBehaviour
{
    public GameObject scoreGapGaugeMarker;

    public TextMeshProUGUI scoreGapText;
    public TextMeshProUGUI p1_rateText;
    public TextMeshProUGUI p1_missText;
    public TextMeshProUGUI p2_rateText;
    public TextMeshProUGUI p2_missText;

    private float scoreGap;
    private float p1_rate;
    private float p2_rate;
    private float p1_miss;
    private float p2_miss;

    private float p1_score;
    private float p2_score;

    void Start()
    {
        scoreGap = 0;
        p1_miss = 0;
        p1_rate = 0;
        p2_miss = 0;
        p2_rate = 0;
        p1_score = 0;
        p2_score = 0;
    }

    // Update is called once per frame

    public void UpdateScoreBoard()
    {
        
        p1_rateText.text = $"{p1_rate:F2}%";
        p1_missText.text = $"{p1_miss}";
        p2_rateText.text = $"{p2_rate:F2}%";
        p2_missText.text = $"{p2_miss}";

        UpdateScoreGapGauge();
    }


    public void UpdateScoreBoard_p1()
    {
        p1_rate = KGB_GameManager_Multi.Instance.accuracy;
        p1_miss = KGB_GameManager_Multi.Instance.missCount;
        p1_score = KGB_GameManager_Multi.Instance.totalScore;
        UpdateScoreBoard();
    }

    public void UpdateScoreBoard_p2(ScoreData data)
    {
        p2_rate = data.accuracy;
        p2_miss = data.missCount;
        p2_score = data.totalScore;
        UpdateScoreBoard();
    }


    private void UpdateScoreGapGauge()
    {
        float total = p1_score + p2_score;

        // ������ �� �� 0�̸� �������� �߾�(0)���� �д�
        if (total <= 0)
        {
            scoreGapGaugeMarker.transform.localPosition = new Vector3(0,
                scoreGapGaugeMarker.transform.localPosition.y,
                scoreGapGaugeMarker.transform.localPosition.z);
            return;
        }

        // (p2 ���� - p1 ����) = [-1, 1] ����
        float ratio = (p2_score - p1_score) / total;

        // ratio�� -160 ~ 160 ������ ����
        float xPos = ratio * 160f;

        // ����
        scoreGapGaugeMarker.transform.localPosition = new Vector3(
            xPos,
            scoreGapGaugeMarker.transform.localPosition.y,
            scoreGapGaugeMarker.transform.localPosition.z
        );

        float gap = p1_score - p2_score;

        if (gap > 0)
        {
            // p1�� �� ���� ��� �� �����, +����
            scoreGapText.text = $"+{gap}";
            scoreGapText.color = new Color(155f / 255f, 48f / 255f, 255f / 255f);  // �����
        }
        else if (gap < 0)
        {
            // p2�� �� ���� ��� �� �ϴû�, -����
            scoreGapText.text = $"{gap}"; // gap�� �����ϱ� -60 ���� �״�� ǥ��
            scoreGapText.color = new Color(60f / 255f, 253f / 255f, 255f / 255f); // �ϴû�
        }
        else
        {
            // ���� �� �� ���, 0
            scoreGapText.text = "0";
            scoreGapText.color = Color.black;
        }
    }
}

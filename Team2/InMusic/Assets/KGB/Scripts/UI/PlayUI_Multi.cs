using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayUI_Multi : MonoBehaviour
{
    [SerializeField] Slider hpBar; //ü�� ǥ�� �����̵��
    [SerializeField] TextMeshProUGUI comboText; // �޺� ǥ�� �ؽ�Ʈ
    public TextMeshProUGUI scoreText; // ���� ǥ�� �ؽ�Ʈ
    public TextMeshProUGUI accuracyText; // ��Ȯ�� ǥ�� �ؽ�Ʈ
    [SerializeField] GameObject[] judgeText;
    public TextMeshProUGUI countText;
    void Start()
    {
        //hpBar.value = GameManagerProvider.Instance.MaxHP;
        //comboText.text = GameManagerProvider.Instance.Combo.ToString();
        countText.text = "";
    }

    public void UpdatePlayUI_Multi(ScoreData data)
    {
        //comboText.text = GameManagerProvider.Instance.Combo.ToString();
        //hpBar.value = GameManagerProvider.Instance.CurHP / 100f;

        //ü��, ����, ��Ȯ��, �޺�
        hpBar.value = data.curHp;
        scoreText.text = $"{data.totalScore:F0}";
        accuracyText.text = $"{data.accuracy:F2}%";
        comboText.text = $"{data.combo}";
        JudgeTextUpdate(data.judgement);
    }

    public void JudgeTextUpdate(string judgement)
    {
        foreach (GameObject text in judgeText)
        {
            text.SetActive(false); // ������ Ȱ��ȭ�� �ؽ�Ʈ�� ������ ��� ��Ȱ��ȭ
        }
        switch (judgement)
        {
            case "Great":
                judgeText[0].SetActive(true); // Great Ȱ��ȭ
                break;
            case "Good":
                judgeText[1].SetActive(true); // Good Ȱ��ȭ
                break;
            case "Bad":
                judgeText[2].SetActive(true); // Bad Ȱ��ȭ
                break;
            case "Miss":
                judgeText[3].SetActive(true); // Miss Ȱ��ȭ
                break;
        }
    }
}

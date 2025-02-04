using TMPro;
using Unity.Burst.Intrinsics;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class PlayUI : MonoBehaviour
{
    [SerializeField] Slider hpBar;
    [SerializeField] TextMeshProUGUI comboText; // �޺� ǥ�� �ؽ�Ʈ
    [SerializeField] GameObject[] judgeText;
    public Image BackImage;
    public TextMeshProUGUI countText;
    

    void Start()
    {
        hpBar.value = GameManager.Instance.maxHP;
        comboText.text = GameManager.Instance.combo.ToString();
        countText.text = "";
    }

    void Update()
    {
        
    }

    public void UpdatePlayUI()
    {
        comboText.text = GameManager.Instance.combo.ToString();
        hpBar.value = GameManager.Instance.curHP/100f;
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

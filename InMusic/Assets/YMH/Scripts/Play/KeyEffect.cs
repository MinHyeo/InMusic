using UnityEngine;

public class KeyEffect : MonoBehaviour
{
    private Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void Start()
    {
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        anim.SetTrigger("IsHit");
    }

    public void SetActiveFalse()
    {
        gameObject.SetActive(false);
    }
}

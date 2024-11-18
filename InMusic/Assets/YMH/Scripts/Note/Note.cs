using System.Collections;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Note : MonoBehaviour
{
    private float speed;
    private float targetY;

    public void Initialize(float noteSpeed, float judgementLineY)
    {
        speed = noteSpeed;
        targetY = judgementLineY;
    }

    void Update()
    {
        // 노트가 내려오는 로직
        transform.Translate(Vector3.down * speed * Time.deltaTime);

        // 판정선에 도달하면 삭제
        if (transform.position.y <= targetY)
        {
            Destroy(gameObject);
        }
    }
}
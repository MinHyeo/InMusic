using UnityEngine;
using Fusion;

public class PlayerController : NetworkBehaviour
{
    public float moveSpeed = 5f;

    public override void FixedUpdateNetwork()
    {
        if (!HasInputAuthority)
            return;

        if (GetInput(out NetworkInputData input))
        {
            Vector2 moveDir = input.moveDirection.normalized;
            transform.position += (Vector3)moveDir * moveSpeed * Runner.DeltaTime;

        }
    }
}

using UnityEngine;

public class PlayerAnimationEvents : MonoBehaviour
{
    [SerializeField] Collider attackCollider;
    [SerializeField] Collider chargeCollider;

    public void EnableAttackCollider()
    {
        attackCollider.enabled = true;
    }

    public void EnableChargeCollider()
    {
        chargeCollider.enabled = true;
    }

    public void DisableColliders()
    {
        attackCollider.enabled = false;
        chargeCollider.enabled = false;
    }

}

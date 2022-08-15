using UnityEngine;

public class PlayerHitCollider : MonoBehaviour
{
    [SerializeField] Player player;
    [SerializeField][Range(0.5f, 3f)] float damageMultiplier = 1f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("Atingiu um Inimigo");
        }
    }


}

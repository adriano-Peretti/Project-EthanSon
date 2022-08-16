using UnityEngine;

public class PlayerProjectile : MonoBehaviour
{
    [Header("Projectile")]
    [SerializeField] float speed = 20f;
    [SerializeField] float lifeTime = 5f;

    [Header("Damage")]
    public float playerAttack;
    [SerializeField][Range(0.5f, 3f)] float damageMultiplier = 1f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("Atingiu um Inimigo");
        }

        Destroy(this.gameObject, .1f);
    }


    private void Start()
    {
        Destroy(this.gameObject, lifeTime);
    }


    private void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }
}

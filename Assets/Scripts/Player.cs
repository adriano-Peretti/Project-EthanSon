using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Status")]
    public int health = 100;
    public int currentHealth = 0;

    public int attack = 10;

    public int rangedCharges = 3;
    public int currentRangedCharges = 0;
    public float chargeRefreshTime = 5f;

    [Header("Damage")]
    [SerializeField] float hitCooldown = .2f;
    float hitReady = 0f;



    private void Awake()
    {
        currentHealth = health;

        currentRangedCharges = rangedCharges;
    }
}

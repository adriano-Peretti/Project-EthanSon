using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Player player;
    [SerializeField] Animator animator;
    [SerializeField] Transform visual;
    Rigidbody rb;

    [Header("Movement")]
    [SerializeField] float speed = 10f;
    [SerializeField] float turnSpeed = 720f;
    Vector3 movement = Vector3.zero;
    bool isMoving = false;

    [Header("Dash")]
    [SerializeField] float dashForce = 5f;
    [SerializeField] float dashCooldown;
    float dashReady = 0f;

    [Header("Attack")]
    //Normal Attack
    [SerializeField] float attackCooldown = 0.3f;
    float attackReady = 0f;
    bool isAttacking = false;
    Coroutine waitAttackCoroutine = null;

    //Combo Attack
    [SerializeField] float startComboTime = 0.15f;
    [SerializeField] float endComboTime = 0.4f;
    bool canCombo = false;
    Coroutine canComboCoroutine = null;

    //Charge Attack
    [SerializeField] float chargeHoldTime = 0.9f;
    float chargeReady = 0f;
    bool isHolding = false;
    Coroutine chargingCoroutine = null;

    [Header("Ranged Attack")]
    [SerializeField] Transform firePoint;
    [SerializeField] GameObject firePrefab;
    [SerializeField] LayerMask mouseLayer;
    [SerializeField] float rangedAttackCooldown = 0.5f;


    //Animations
    readonly int anim_isWalking = Animator.StringToHash("isWalking");
    readonly int anim_isCharging = Animator.StringToHash("isCharging");
    readonly int anim_attack = Animator.StringToHash("attack");
    readonly int anim_comboAttack = Animator.StringToHash("comboAttack");
    readonly int anim_chargeAttack = Animator.StringToHash("chargeAttack");
    readonly int anim_rangedAttack = Animator.StringToHash("rangedAttack");

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        movement = Vector3.right * context.ReadValue<Vector2>().x + Vector3.forward * context.ReadValue<Vector2>().y;
        isMoving = movement.magnitude > 0f;
        animator.SetBool(anim_isWalking, isMoving);
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (Time.time > dashReady)
        {
            dashReady = Time.time + dashCooldown;

            if (isMoving)
            {
                rb.AddForce(movement * dashForce * 1000f, ForceMode.Impulse);

                Quaternion rotateDirection = Quaternion.LookRotation(movement);
                visual.rotation = rotateDirection;
            }
            else
            {
                rb.AddForce(visual.forward * dashForce * 1000f, ForceMode.Impulse);
            }

        }
    }

    public void OnRangedAttack(InputAction.CallbackContext context)
    {
        if (Time.time > attackReady && player.currentRangedCharges > 0)
        {
            player.currentRangedCharges--;

            attackReady = Time.time + rangedAttackCooldown;

            if (waitAttackCoroutine != null)
            {
                StopCoroutine(waitAttackCoroutine);
            }

            waitAttackCoroutine = StartCoroutine(EWaitAttackTime(0.2f));

            //Logica do Projectil
            Ray mouseRay = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(mouseRay, out RaycastHit hit, 500f, mouseLayer))
            {
                Vector3 mouseDirection = hit.point - visual.position;
                mouseDirection.y = 0f;

                visual.rotation = Quaternion.LookRotation(mouseDirection, Vector3.up);
                animator.SetTrigger(anim_rangedAttack);

                PlayerProjectile projectile = Instantiate(firePrefab, firePoint.position, firePoint.rotation).GetComponent<PlayerProjectile>();
                projectile.playerAttack = player.attack;
            }

        }
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            chargeReady = Time.time + chargeHoldTime;
            isHolding = true;

            if (chargingCoroutine != null)
            {
                StopCoroutine(chargingCoroutine);
            }

            chargingCoroutine = StartCoroutine(EChargingAnimation());
        }

        if (context.canceled)
        {
            isHolding = false;

            if (animator.GetBool(anim_isCharging))
            {
                animator.SetBool(anim_isCharging, false);
            }

            if (Time.time > chargeReady)
            {
                attackReady = Time.time + (attackCooldown * 2f);
                animator.SetTrigger(anim_chargeAttack);

                if (waitAttackCoroutine != null)
                {
                    StopCoroutine(waitAttackCoroutine);
                }

                waitAttackCoroutine = StartCoroutine(EWaitAttackTime(0.2f));
            }
            else
            {
                if (canCombo)
                {
                    if (canComboCoroutine != null)
                    {
                        StopCoroutine(canComboCoroutine);
                    }

                    canCombo = false;

                    attackReady = Time.time + attackCooldown;
                    animator.SetTrigger(anim_comboAttack);

                    if (waitAttackCoroutine != null)
                    {
                        StopCoroutine(waitAttackCoroutine);
                    }

                    waitAttackCoroutine = StartCoroutine(EWaitAttackTime(0.2f));
                }
                else
                {
                    if (Time.time > attackReady)
                    {
                        attackReady = Time.time + attackCooldown;
                        animator.SetTrigger(anim_attack);

                        if (waitAttackCoroutine != null)
                        {
                            StopCoroutine(waitAttackCoroutine);
                        }

                        waitAttackCoroutine = StartCoroutine(EWaitAttackTime(0.2f));

                        if (canComboCoroutine != null)
                        {
                            StopCoroutine(canComboCoroutine);
                        }

                        canComboCoroutine = StartCoroutine(ECanComboWindow());
                    }
                }
            }
        }
    }

    IEnumerator EWaitAttackTime(float waitTime)
    {
        isAttacking = true;
        yield return new WaitForSeconds(waitTime);
        isAttacking = false;
    }

    IEnumerator ECanComboWindow()
    {
        yield return new WaitForSeconds(startComboTime);
        canCombo = true;
        yield return new WaitForSeconds(endComboTime - startComboTime);
        canCombo = false;
    }

    IEnumerator EChargingAnimation()
    {
        yield return new WaitForSeconds(.15f);

        if (isHolding)
        {
            animator.SetBool(anim_isCharging, true);
        }
    }

    private void FixedUpdate()
    {
        rb.velocity = Vector3.zero;

        if (isAttacking)
        {
            rb.velocity = new Vector3(0f, rb.velocity.y, 0f);
            return;
        }


        if (!isAttacking)
        {
            rb.velocity = movement * speed;

            if (isMoving)
            {
                Quaternion desiredRotation = Quaternion.LookRotation(movement);
                visual.rotation = Quaternion.RotateTowards(visual.rotation, desiredRotation, turnSpeed * Time.deltaTime);
            }
        }
        if (isMoving)
        {
            Quaternion desiredRotation = Quaternion.LookRotation(movement);
            visual.rotation = Quaternion.RotateTowards(visual.rotation, desiredRotation, turnSpeed * Time.deltaTime);
        }
    }
}

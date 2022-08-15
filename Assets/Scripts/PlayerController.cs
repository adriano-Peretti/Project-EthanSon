using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Animator animator;
    [SerializeField] Transform visual;
    Rigidbody rb;

    [Header("Movement")]
    [SerializeField] float speed = 10f;
    [SerializeField] float turnSpeed = 720f;
    Vector2 movement = Vector2.zero;
    bool isMoving = false;

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

    //Animations
    readonly int anim_isWalking = Animator.StringToHash("isWalking");
    readonly int anim_isCharging = Animator.StringToHash("isCharging");
    readonly int anim_attack = Animator.StringToHash("attack");
    readonly int anim_comboAttack = Animator.StringToHash("comboAttack");
    readonly int anim_chargeAttack = Animator.StringToHash("chargeAttack");

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        movement = context.ReadValue<Vector2>();
        isMoving = movement.magnitude > 0f;
        animator.SetBool(anim_isWalking, isMoving);
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
        if (isAttacking)
        {
            rb.velocity = new Vector3(0f, rb.velocity.y, 0f);
            return;
        }

        rb.velocity = new Vector3(movement.x * speed, rb.velocity.y, movement.y * speed);

        if (isMoving)
        {
            Quaternion desiredRotation = Quaternion.LookRotation(new Vector3(movement.x, 0f, movement.y));
            visual.rotation = Quaternion.RotateTowards(visual.rotation, desiredRotation, turnSpeed * Time.deltaTime);
        }
    }
}

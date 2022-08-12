using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Transform visual;
    Rigidbody rb;

    [Header("Moviment")]
    [SerializeField] float speed = 10f;
    [SerializeField] float turnSpeed = 720f;
    Vector2 movement = Vector2.zero;
    bool isMoving = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        movement = context.ReadValue<Vector2>();
        isMoving = movement.magnitude > 0f;
    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector3(movement.x * speed, rb.velocity.y, movement.y * speed);

        if (isMoving)
        {
            Quaternion desiredRotation = Quaternion.LookRotation(new Vector3(movement.x, 0f, movement.y));
            visual.rotation = Quaternion.RotateTowards(visual.rotation, desiredRotation, turnSpeed * Time.deltaTime);
        }
    }



}
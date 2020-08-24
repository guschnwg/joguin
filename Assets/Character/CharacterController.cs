using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    public Camera camera;
    public Animator animator;
    public Rigidbody rigidbody;
    public GameState gameState;

    public float walkForce= 6f;
    public float runForce = 18f;
    public float jumpForce = 5f;
    public float drag = 0.85f;
    public float turnSmoothTime = 0.1f;
    public float animatorVelocitySmoothTime = 0.1f;

    // Do not mess
    [Header("DEBUG")]

    public float turnSmoothVelocity;
    public float animatorSmoothVelocity;
    public float animatorVelocity;

    public float inputHorizontal;
    public float inputVertical;
    public bool inputJump;
    public float targetAngle;
    public float smoothTargetAngle;
    public float currentAngle;

    public Vector3 moveDirection;
    public Vector3 velocity;

    public bool running;
    public bool grounded;

    // Start is called before the first frame update
    void Start()
    {
        //Cursor.lockState = CursorLockMode.Locked;
    }

    void FixedUpdate()
    {
        Move();
        Rotate();
        Jump();

        Vector3 currentVelocity = rigidbody.velocity;
        currentVelocity.z *= drag;
        currentVelocity.x *= drag;
        rigidbody.velocity = currentVelocity;
        velocity = currentVelocity;
    }

    // Update is called once per frame
    void Update()
    {
        running = Input.GetKey(KeyCode.LeftShift);
        inputHorizontal = Input.GetAxis("Horizontal");
        inputVertical = Input.GetAxis("Vertical");
        inputJump = Input.GetKeyDown(KeyCode.Space);
        currentAngle = transform.eulerAngles.y;
    }

    void Rotate()
    {

    }

    void Move()
    {
        Vector3 direction = new Vector3(inputHorizontal, 0f, inputVertical).normalized;

        float forceToApply = direction.magnitude == 0 ? 0 : running ? runForce : walkForce;

        if (direction.magnitude > 0)
        {
            targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + camera.transform.eulerAngles.y;
            smoothTargetAngle = Mathf.SmoothDampAngle(currentAngle, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, smoothTargetAngle, 0f);

            moveDirection = (Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward).normalized;
            rigidbody.AddForce(moveDirection * forceToApply, ForceMode.Acceleration);
        }

        animatorVelocity = Mathf.SmoothDamp(animator.GetFloat("Velocity"), Mathf.InverseLerp(0, runForce, forceToApply), ref animatorSmoothVelocity, animatorVelocitySmoothTime);
        animator.SetFloat("Velocity", animatorVelocity);
    }

    void Jump ()
    {
        if (grounded && inputJump)
        {
            animator.SetBool("Jumping", true);
            rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 8)
        {
            grounded = true;
            animator.SetBool("Jumping", false);
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.layer == 8)
        {
            grounded = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.layer == 8)
        {
            grounded = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Food"))
        {
            Destroy(other.gameObject, 0);
            gameState.score++;
        }
    }
}

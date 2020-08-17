using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public float walkAcceleration = 10f;
    public float runAcceleration = 30f;
    public float jumpForce = 4f;
    public float drag = 0.85f;

    Transform playerBody;
    Animator animator;
    new Rigidbody rigidbody;

    bool isGrounded = false;
    bool isAttacking = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody>();
        playerBody = GetComponent<Transform>();

        //Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        Translation();
        Jump();
        Attack();
    }

    private void FixedUpdate()
    {
        if (isGrounded || true) // Why not
        {
            Vector3 currentVelocity = rigidbody.velocity;
            currentVelocity.x *= drag;
            currentVelocity.z *= drag;
            rigidbody.velocity = currentVelocity;
        }

        Vector3 localVelocity = Quaternion.Inverse(transform.rotation) * rigidbody.velocity / 8; // Arbitrary 😬
        Debug.Log(localVelocity);
        animator.SetFloat("VelocityX", localVelocity.x);
        animator.SetFloat("VelocityZ", localVelocity.z);
    }

    void Translation()
    {
        if (isGrounded || true) // Why not
        {
            bool runModifier = Input.GetKey(KeyCode.LeftShift);
            float acceleration = runModifier ? runAcceleration : walkAcceleration;

            float vertical = Input.GetAxis("Vertical");
            float horizontal = Input.GetAxis("Horizontal");

            Vector3 moveVertical = playerBody.forward * vertical * acceleration;
            Vector3 moveHorizontal = playerBody.right * horizontal * acceleration;

            rigidbody.AddForce(moveVertical + moveHorizontal, ForceMode.Acceleration);

            animator.SetBool("isForward", vertical > 0);
            animator.SetBool("isBackward", vertical < 0);
            animator.SetBool("isRight", horizontal > 0);
            animator.SetBool("isLeft", horizontal < 0);

            animator.SetBool("isRunning", runModifier && vertical != 0);
        }
    }
    void Jump()
    {
        if (isGrounded)
        {
            bool jump = Input.GetKeyDown(KeyCode.Space);

            if (jump)
            {
                animator.SetBool("IsJumping", true);

                Vector3 move = Vector3.up * jumpForce;

                //rigidbody.AddForce(move, ForceMode.Impulse);

                // Don't want this, but the animation has a minor delay
                IEnumerator JumpDelayed()
                {
                    yield return new WaitForSeconds(0.5f);
                    rigidbody.AddForce(move, ForceMode.Impulse);
                }
                StartCoroutine(JumpDelayed());
            }

        }
    }

    void Attack()
    {
        bool attack = Input.GetKeyDown(KeyCode.LeftCommand);

        if (isAttacking &&
            animator.GetCurrentAnimatorStateInfo(1).IsName("Punch") &&
            animator.GetCurrentAnimatorStateInfo(1).normalizedTime >= 1.0f)
        {
            animator.SetBool("IsPunching", false);
            isAttacking = false;
        }

        if (attack && !isAttacking)
        {
            animator.SetBool("IsPunching", true);
            isAttacking = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Floor")
        {
            isGrounded = true;
            animator.SetBool("IsJumping", false);
            animator.SetBool("isGrounded", true);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Floor")
        {
            isGrounded = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Floor")
        {
            isGrounded = false;
            animator.SetBool("isGrounded", false);
        }
    }
}

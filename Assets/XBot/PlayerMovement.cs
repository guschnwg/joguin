using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    public float walkSpeed;
    public float runSpeed;
    public float speedMagnitudeMax;
    public float drag;
    public float mouseRotationSpeed;
    public float movementRotationSpeed;

    Animator animator;
    new Camera camera;
    new Rigidbody rigidbody;

    public bool isGrounded = false;


    [Header("Read only")]
    public float inputHorizontal;
    public float inputVertical;
    public bool inputRun;
    public bool inputJump;
    public Vector3 move;
    public float speedMagnitude;
    public float angleBetweenPlayerAndCamera;
    public float angleWanted;
    public bool angleAccepted;
    public Vector3 cameraForward;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        camera = Camera.main;
        rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        inputHorizontal = Input.GetAxis("Horizontal");
        inputVertical = Input.GetAxis("Vertical");
        inputRun = Input.GetKey(KeyCode.LeftShift);
        inputJump = Input.GetKeyDown(KeyCode.Space);

        cameraForward = camera.transform.forward;
        cameraForward.y = 0;

        Jump();

        Rotation();
        Movement();
    }

    void FixedUpdate()
    {
        Vector3 currentVelocity = rigidbody.velocity;
        currentVelocity.z *= drag;
        currentVelocity.x *= drag;
        rigidbody.velocity = currentVelocity;


        // Later we do it better
        speedMagnitude = rigidbody.velocity.magnitude;
        animator.SetFloat("Speed", Mathf.InverseLerp(0, speedMagnitudeMax, speedMagnitude) * (inputVertical < 0 ? -1 : 1));
    }


    void Rotation()
    {
        float desiredRotationAngle = Vector3.SignedAngle(transform.forward, cameraForward, Vector3.up);

        if (move.magnitude > 0 && inputHorizontal == 0 && Mathf.Abs(desiredRotationAngle) > 10)
        {
            transform.Rotate(Vector3.up * desiredRotationAngle * mouseRotationSpeed);
        }
    }

    void Movement()
    {
        angleBetweenPlayerAndCamera = Vector3.SignedAngle(transform.forward, cameraForward, Vector3.up);
        angleWanted = (90 - 45 * inputVertical) * inputHorizontal * -1;
        angleAccepted = Mathf.Abs(angleBetweenPlayerAndCamera - angleWanted) < 10;

        float speed = inputRun ? runSpeed : walkSpeed;

        if (!angleAccepted)
        {
            transform.Rotate(Vector3.up * (angleBetweenPlayerAndCamera - angleWanted) * movementRotationSpeed);
        }

        move = transform.forward * Mathf.Clamp(Mathf.Abs(inputHorizontal) + Mathf.Abs(inputVertical), 0, 1) * speed;
        rigidbody.AddForce(move, ForceMode.Acceleration);

    }

    void Jump()
    {
        if (inputJump && isGrounded)
        {
            animator.SetBool("IsJumping", true);

            // Don't want this, but the animation has a minor delay
            IEnumerator JumpDelayed(Vector3 moveForce)
            {
                yield return new WaitForSeconds(0.5f);
                rigidbody.AddForce(moveForce, ForceMode.Impulse);
            }

            Vector3 move = Vector3.up * 4f;
            StartCoroutine(JumpDelayed(move));
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Floor"))
        {
            isGrounded = true;
            animator.SetBool("IsJumping", false);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Floor"))
        {
            isGrounded = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Floor"))
        {
            isGrounded = false;
        }
    }
}

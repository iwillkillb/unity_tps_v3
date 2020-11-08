using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerInput), typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    // Components
    PlayerInput playerInput;
    CharacterController characterController;

    [Header("Move")]
    public float movementSpeed = 7f;
    public float moveAxisSlerp = 5f;
    public float slopeForce = 5f;
    float verticalVelocity = 0f;
    float currentSlopeForce = 0f;
    float axisHor, axisVer;

    [Header("Rotation")]
    public float rotationSpeed = 5f;
    float rotationAngleDifference = 0f;

    [Header("Jump")]
    public float gravity = 9.81f;
    public float jumpForce = 10f;

    [Header("Dash")]
    public float dashSpeed = 20f;
    public float dashTime = 0.5f;
    float currentDashTime = 0f;

    [Header("Animator")]
    public Animator animator;
    public string apMove = "move";
    public string apDirection = "direction";
    public string apIsGrounded = "isGrounded";
    public string apVVelocity = "vVelocity";

    void Awake()
    {
        // Components connecting
        playerInput = GetComponent<PlayerInput>();
        characterController = GetComponent<CharacterController>();
    }


    void Update()
    {
        // input axis lerp
        axisHor = Mathf.Lerp(axisHor, playerInput.axisHor, Time.deltaTime * moveAxisSlerp);
        axisVer = Mathf.Lerp(axisVer, playerInput.axisVer, Time.deltaTime * moveAxisSlerp);

        SetVerticalVelocity(playerInput.axisJump);
        Movement(axisHor, axisVer);
        Rotation(axisHor, axisVer, playerInput.axisAttack);
        Dash();

        SetAnimationParameter(axisHor, axisVer, playerInput.axisAttack);
    }

    void SetVerticalVelocity(bool axisJump)
    {
        /*
        // Slope check
        Vector3 groundNormal = GetGroundNormal();
        float currentSlopeForce = slopeForce;
        //float currentSlopeForce = Mathf.Lerp(0f, slopeForce, Vector3.Angle(Vector3.up, groundNormal) / characterController.slopeLimit);

        if (groundNormal != Vector3.up)
            verticalVelocity -= characterController.height * currentSlopeForce;
        */

        // Ground check and gravity
        if (characterController.isGrounded)
        {
            /*
            // Slow falling
            if (verticalVelocity > -gravity)
                verticalVelocity = -gravity * Time.deltaTime;
            */
            if (axisJump)
            {
                // Jump
                verticalVelocity = jumpForce;
            }
            else
            {
                // Slope check
                currentSlopeForce = (GetGroundNormal() != Vector3.up) ? slopeForce : 0f;

                if (verticalVelocity > -gravity)
                    verticalVelocity -= currentSlopeForce;
            }
        }
        else
        {
            // Falling
            if (verticalVelocity > -gravity)
                verticalVelocity -= gravity * Time.deltaTime;

            // Ceiling check
            if (characterController.collisionFlags == CollisionFlags.Above)
            {
                verticalVelocity = (verticalVelocity > 0f) ? 0f : verticalVelocity;
            }
        }
    }

    Vector3 GetGroundNormal()
    {
        // Slope Check
        RaycastHit slopeHit;
        Vector3 groundNormal = Vector3.up;
        if (Physics.Raycast(transform.position, -transform.up, out slopeHit, 0.1f))
        {
            groundNormal = slopeHit.normal;
        }

        return groundNormal;
    }

    void Movement(float axisHor, float axisVer)
    {
        // Use Input data
        Vector3 moveAxis = Vector3.right * axisHor + Vector3.forward * axisVer;

        // Get Camera's y rotation.
        Quaternion moveDir = Quaternion.Euler(Vector3.up * Camera.main.transform.eulerAngles.y);

        // Quaternion groundQuaternion = Quaternion.FromToRotation(transform.up, GetGroundNormal());

        // Setting velocity
        Vector3 resultVelocity = Vector3.up * verticalVelocity + (moveDir * moveAxis) * movementSpeed;

        // Actual Moving
        characterController.Move(resultVelocity * Time.deltaTime);
    }

    void Rotation(float axisHor, float axisVer, bool isStaringFront)
    {
        // Character Rotation : Character and camera move in the apposite direction (in Y axis).
        // Example : 
        // 1. Input Right key -> inputAngle is 90
        // 2. Character moves Right -> Character rotates (inputAngle + Camera's current angle)

        // inputAngle : Front 0, Back 180, Left -90, Right 90
        float inputAngle = Mathf.Atan2(axisHor, axisVer) * Mathf.Rad2Deg;

        // Rotation backup
        Quaternion newRot = transform.rotation;  // Set new direction rotation value.

        // Stare
        // True  : Character stares Camera's direction.
        // False : The character looks in the direction in which it moves.
        if (isStaringFront)
        {
            newRot = Quaternion.Euler(Vector3.up * Camera.main.transform.eulerAngles.y);
        }
        // No stare -> Rotate only moving
        else if (axisHor != 0f || axisVer != 0f)
        {
            newRot = Quaternion.Euler(Vector3.up * (inputAngle + Camera.main.transform.eulerAngles.y));

            // Get angle's difference between Current euler angle and Goal euler angle. (-180 ~ 180)
            rotationAngleDifference = inputAngle + Camera.main.transform.eulerAngles.y - transform.eulerAngles.y;
            if (rotationAngleDifference > 180f)
                rotationAngleDifference -= 360f;
            else if (rotationAngleDifference < -180f)
                rotationAngleDifference += 360f;
        }
        else
        {
            // Initialization in stoppint time
            if (rotationAngleDifference != 0f)
                rotationAngleDifference = 0f;
        }

        // Actual Rotation
        transform.rotation = Quaternion.Slerp(transform.rotation, newRot, rotationSpeed * Time.deltaTime);
    }

    void Dash()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            currentDashTime = dashTime;
        }

        if (currentDashTime > 0f)
        {
            currentDashTime -= Time.deltaTime;

            Rotation(0f, 1f, true);

            // Dash to camera's forward
            characterController.Move(Quaternion.Euler(Vector3.up * Camera.main.transform.eulerAngles.y) * Vector3.forward * (dashSpeed * currentDashTime / dashTime) * Time.deltaTime);
        }
    }

    void SetAnimationParameter(float axisHor, float axisVer, bool isStaringFront)
    {
        if (animator == null)
        {
            return;
        }

        if (isStaringFront)
        {
            animator.SetFloat(apMove, axisVer);
        }
        else
        {
            //animator.SetFloat("move", Mathf.Max(Mathf.Abs(inputAxisHor), Mathf.Abs(inputAxisVer)));
            animator.SetFloat(apMove, Mathf.Max(Mathf.Abs(axisHor), Mathf.Abs(axisVer)), 0.1f, Time.deltaTime);
        }

        animator.SetFloat(apDirection, rotationAngleDifference / 180f);
        animator.SetFloat(apVVelocity, verticalVelocity);
        animator.SetBool(apIsGrounded, characterController.isGrounded);
    }



    /*
    void LookAtPoint(Vector3 point)
    {
        Vector3 direction = (point - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0f, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }*/
}

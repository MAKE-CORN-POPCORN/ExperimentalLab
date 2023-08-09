using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animationMixingState : MonoBehaviour
{
    
    [Header("Debug Zone")]
    public bool doDebug;

    Animator animator;
    int isWalkingHash;
    int isSprintingHash;
    int cancelRunningHash;
    float Velocity = 0.0f;
    float Accleration = 0.4f;
    float Deccleration = 0.8f;
    int velocityHash;
    // Start is called before the first frame update
    void Start()
    {
        // can check for the difference between "gameObject.GetComponent" and "GetComponent"
        animator = gameObject.GetComponent<Animator>();

        isWalkingHash = Animator.StringToHash("isWalking");
        isSprintingHash = Animator.StringToHash("isSprinting");
        cancelRunningHash = Animator.StringToHash("cancelRunning");
        velocityHash = Animator.StringToHash("Velocity");
    }

    // model state
    bool isWalking = false;
    bool isRunning = false;
    bool isSprinting = false;
    bool leftControlLeave = false;
    // this is the speed set in the blending part (BlendTree)
    float sprintSpeed = 4.0f;
    float runSpeed = 3.0f;
    float walkSpeed = 1.0f;
    float idleSpeed = 0.0f;
    // timer
    float sprintTime = 2.5f;
    float timer = 0.0f;
    // Update is called once per frame
    void FixedUpdate()
    {
        if (isSprinting)
        {
            timer += Time.fixedDeltaTime;
            if (timer > sprintTime)
            {
                timer = 0;
                animator.SetBool(isSprintingHash, false);
                isSprinting = false;
            }
        }

        bool forwardPressing = Input.GetKey(KeyCode.W);
        bool leftControlPressing = Input.GetKey(KeyCode.LeftControl);
        if (forwardPressing && !isWalking)
        {
            animator.SetBool(isWalkingHash, true);
            isWalking = true; Velocity = walkSpeed;
            animator.SetBool(cancelRunningHash, false);
        }
        else if (!forwardPressing && isWalking)
        {
            animator.SetBool(isWalkingHash, false);
            isWalking = false; Velocity = idleSpeed;
        }

        if (isWalking && leftControlPressing && !isRunning)
        {
            isRunning = true;
            leftControlLeave = false;
        }
        else if (!forwardPressing && isRunning)
        {
            isRunning = false; 
            animator.SetBool(cancelRunningHash, true);
        }

        if (!leftControlPressing && isRunning)
        {
            leftControlLeave = true;
        }
        
        if (leftControlLeave && leftControlPressing && isRunning && !isSprinting)
        {
            animator.SetBool(isSprintingHash, true);
            isSprinting = true; Velocity = sprintSpeed;
        }
        else if (isSprinting && !forwardPressing)
        {
            animator.SetBool(isSprintingHash, false);
            animator.SetBool(cancelRunningHash, true);
            isSprinting = false; Velocity = runSpeed;
        }


        if (isSprinting) // means sprinting
        {
            Velocity = sprintSpeed;
        }
        else if (isRunning) // means not sprinting running
        {
            Velocity += Accleration * Time.deltaTime;
            if (doDebug)
            {
                Debug.Log('H' + Velocity.ToString());
            }
            if (Velocity > runSpeed)
            {
                if (doDebug)
                {
                    Debug.Log('I');
                }
                Velocity = runSpeed;
            }
        }
        else if (isWalking) // means not running walking
        {
            Velocity = walkSpeed;
        }
        else if (!isRunning && !isWalking && !isSprinting) // means leave the "W" key
        {
            Velocity -= Deccleration * Time.deltaTime;
            if (Velocity < 0.0f)
            {
                Velocity = 0.0f;
            }
        }
        
        animator.SetFloat(velocityHash, Velocity);

    }
}

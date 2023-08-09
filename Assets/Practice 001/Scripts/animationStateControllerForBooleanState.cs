using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animationStateControllerForBooleanState : MonoBehaviour
{
    
    [Header("Debug Zone")]
    public bool doDebug;

    Animator animator;
    int isWalkingHash;
    int isRunningHash;
    int isSprintingHash;
    // Start is called before the first frame update
    void Start()
    {
        // can check for the difference between "gameObject.GetComponent" and "GetComponent"
        animator = gameObject.GetComponent<Animator>();

        isWalkingHash = Animator.StringToHash("isWalking");
        isRunningHash = Animator.StringToHash("isRunning");
        isSprintingHash = Animator.StringToHash("isSprinting");
    }

    bool isWalking = false;
    bool isRunning = false;
    bool isSprinting = false;
    bool leftControlLeave = false;

    float waitTime = 5.0f;
    float timer = 0.0f;
    // Update is called once per frame
    void FixedUpdate()
    {
        if (isSprinting) // so i want to make the sprint can go for 5 seconds
        {
            timer += Time.fixedDeltaTime;
            if (timer > waitTime)
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
            isWalking = true;
        }
        else if (!forwardPressing && isWalking)
        {
            animator.SetBool(isWalkingHash, false);
            isWalking = false;
        }

        if (isWalking && leftControlPressing && !isRunning)
        {
            animator.SetBool(isRunningHash, true);
            isRunning = true;
            leftControlLeave = false;
        }
        else if (!forwardPressing && isRunning) // so that when boosted, we can release left-Control
        {
            animator.SetBool(isRunningHash, false);
            isRunning = false;
        }
        /* SUCCESS : but i am not going to use it
            else if (leftControlLeave && leftControlPressing && isRunning) // so that when is running, we can leave the lrft-Control; but when left-Control is double clicked after boostes, end fast-run
            {
                animator.SetBool(isRunningHash, false);
                isRunning = false;
            }
        */

        if (!leftControlPressing && isRunning)
        {
            leftControlLeave = true;
        }
        
        if (leftControlLeave && leftControlPressing && isRunning && !isSprinting) // the same staring mechanic with the above to start the sprint-run
        {
            animator.SetBool(isSprintingHash, true);
            isSprinting = true;
        }
        else if (isSprinting && !forwardPressing)
        {
            animator.SetBool(isSprintingHash, false);
            isSprinting = false;
        }

        if (doDebug)
        {
            Debug.Log(leftControlPressing);
        }
        
    }
}

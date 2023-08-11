using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animationStateControllerForDoubleFloatState : MonoBehaviour
{

    [Header("Debug Zone")]
    public bool doDebug;

    Animator animator;
    float VelocityX = 0.0f;
    float VelocityZ = 0.0f;
    float Accleration = 0.5f;
    float Decceleration = -0.8f;
    int velocityXHash;
    int velocityZHash;
    void Start()
    {
        animator = gameObject.GetComponent<Animator>();
        velocityXHash = Animator.StringToHash("Velocity X");
        velocityZHash = Animator.StringToHash("Velocity Z");
    }

    // this is the speed set in the blending part (BlendTree)
    float sprintSpeed = 4.0f;
    float runSpeed = 2.0f;
    float walkSpeed = 1.0f;
    float idleSpeed = 0.0f;
    // state management
    bool isWalking = false;
    bool isRunning = false;
    void FixedUpdate()
    {

        bool forwardPressing = Input.GetKey(KeyCode.W);
        bool leftSidePressing = Input.GetKey(KeyCode.A);
        bool backwardPressing = Input.GetKey(KeyCode.S);
        bool rightSidePressing = Input.GetKey(KeyCode.D);
        bool leftShiftPressing = Input.GetKey(KeyCode.LeftShift); // to prevent the ctrl+s, switch ti shift

        //oldCode(forwardPressing, leftSidePressing, rightSidePressing, leftShiftPressing);
        /*
         * if (forwardPressing || leftSidePressing || rightSidePressing) // only for forward moving and backward moving do not exists
         */
        if (forwardPressing || leftSidePressing || rightSidePressing || backwardPressing)
        {
            if ((isWalking && !isRunning && leftShiftPressing) || isRunning)
            {
                isRunning = true;
            }

            if (leftShiftPressing)
            {
                //                  the following 3 lines are meant to make sure the running is in the 45 degree if two direction keys are pressed                  //
                /*
                 * // only for forward moving and backward moving do not exists
                 * float max_speed = Mathf.Abs(VelocityX) != VelocityZ ? Mathf.Max(Mathf.Abs(VelocityX), VelocityZ) : -707;
                 * VelocityZ = max_speed == -707 ? VelocityZ : max_speed;
                 * VelocityX = max_speed == -707 ? VelocityX : VelocityX > 0 ? max_speed : -max_speed;
                 */
                float max_speed = Mathf.Abs(VelocityX) != Mathf.Abs(VelocityZ) ? Mathf.Max(Mathf.Abs(VelocityX), Mathf.Abs(VelocityZ)) : -707;
                VelocityZ = max_speed == -707 ? VelocityZ : VelocityZ > 0 ? max_speed : -max_speed;
                VelocityX = max_speed == -707 ? VelocityX : VelocityX > 0 ? max_speed : -max_speed;

                //                  gradually add speed to VelocityZ                  //
                /*
                 * VelocityZ = forwardPressing ? VelocityZ + Accleration * Time.deltaTime : 0.0f; // only for forward moving and backward moving do not exists
                 */
                VelocityZ = Mathf.Abs(VelocityZ) < runSpeed ?
                    (forwardPressing && !backwardPressing ? Mathf.Abs(VelocityZ) + Accleration * Time.deltaTime : (!forwardPressing && backwardPressing ? -Mathf.Abs(VelocityZ) - Accleration * Time.deltaTime : (forwardPressing && backwardPressing ? VelocityZ : 0.0f))) :
                    (forwardPressing && !backwardPressing ? runSpeed : (!forwardPressing && backwardPressing ? -runSpeed : (forwardPressing && backwardPressing ? VelocityZ : 0.0f)));

                //                  gradually add speed to VelocityX                  //
                VelocityX = Mathf.Abs(VelocityX) < runSpeed ?
                    (leftSidePressing && !rightSidePressing ? Mathf.Abs(VelocityX) + Accleration * Time.deltaTime : (!leftSidePressing && rightSidePressing ? -Mathf.Abs(VelocityX) - Accleration * Time.deltaTime : (leftSidePressing && rightSidePressing ? VelocityX : 0.0f))) :
                    (leftSidePressing && !rightSidePressing ? runSpeed : (!leftSidePressing && rightSidePressing ? -runSpeed : (leftSidePressing && rightSidePressing ? VelocityX : 0.0f)));

                //                  make sure the speed is under the limit                 //
                /*
                 * VelocityZ = VelocityZ > runSpeed ? runSpeed : VelocityZ; // only for forward moving and backward moving do not exists
                 */
                VelocityZ = Mathf.Abs(VelocityZ) > runSpeed ? VelocityZ > 0 ? runSpeed : -runSpeed : VelocityZ;
                VelocityX = Mathf.Abs(VelocityX) > runSpeed ? VelocityX > 0 ? runSpeed : -runSpeed : VelocityX;

                //                  vector normalization (may be needed depend on project)                 //
                //...
            }
            else if (isRunning && !leftShiftPressing)
            {
                //                  do the deccelerationEvent                 //
                /*
                 * VelocityZ += VelocityZ > walkSpeed ? Decceleration * Time.deltaTime : 0.0f; // only for forward moving and backward moving do not exists
                 */
                VelocityZ += Mathf.Abs(VelocityZ) > walkSpeed ?
                    (forwardPressing && !backwardPressing ? Decceleration * Time.deltaTime : !forwardPressing && backwardPressing ? -Decceleration * Time.deltaTime : 0.0f) :
                    0.0f;
                VelocityX += Mathf.Abs(VelocityX) > walkSpeed ?
                    (leftSidePressing && !rightSidePressing ? Decceleration * Time.deltaTime : !leftSidePressing && rightSidePressing ? -Decceleration * Time.deltaTime : 0.0f) :
                    0.0f;

                //                  make sure the speed is under the limit                 //
                /*
                 * VelocityZ = VelocityZ!=0.0f ? VelocityZ < walkSpeed ? walkSpeed : VelocityZ : 0.0f; // only for forward moving and backward moving do not exists
                 */
                VelocityZ = VelocityZ != 0.0f ? (Mathf.Abs(VelocityZ) < walkSpeed ? VelocityZ > 0 ? walkSpeed : -walkSpeed : VelocityZ) : 0.0f;
                VelocityX = VelocityX != 0.0f ? (Mathf.Abs(VelocityX) < walkSpeed ? VelocityX > 0 ? walkSpeed : -walkSpeed : VelocityX) : 0.0f;

                //                  vector normalization (may be needed depend on project)                 //
                //...

                //                              update the running flag                             //
                /*
                 * isRunning = (VelocityZ > walkSpeed || Mathf.Abs(VelocityX) > walkSpeed); // only for forward moving and backward moving do not exists
                 */
                isRunning = (Mathf.Abs(VelocityZ) > walkSpeed || Mathf.Abs(VelocityX) > walkSpeed);
            }
            else if (!isRunning)
            {
                //                  the basic input to determine the moving axis                 //
                /*
                 * VelocityZ = forwardPressing ? 1.0f : 0.0f; // only for forward moving and backward moving do not exists
                 */
                VelocityZ = forwardPressing ? (backwardPressing ? 0.0f : 1.0f) : (backwardPressing ? -1.0f : 0.0f);
                VelocityX = leftSidePressing ? (rightSidePressing ? 0.0f : 1.0f) : (rightSidePressing ? -1.0f : 0.0f);
                //                  vector normalization (may be needed depend on project)                 //
                //...

                //                  update the flag                 //
                isWalking = true;
            }
        }
        else
        {
            //                  nothing under pressed                 //
            VelocityZ = 0.0f;
            VelocityX = 0.0f;
            isWalking = false;
        }

        animator.SetFloat(velocityXHash, VelocityX);
        animator.SetFloat(velocityZHash, VelocityZ);

    }


    // battered code -- failed : almost done... --->   can delete
    void oldCode(bool forwardPressing, bool leftSidePressing, bool rightSidePressing, bool leftConrtrolPressing)
    {
        // combined run forward movement
        if (isWalking)
        {
            if (forwardPressing || leftSidePressing || rightSidePressing)
            {
                if (leftConrtrolPressing)
                {
                    isRunning = true;
                    // the following 3 lines are meant to make sure the running is in the 45 degree if two direction keys are pressed
                    float max_speed = Mathf.Abs(VelocityX) != VelocityZ ? Mathf.Max(Mathf.Abs(VelocityX), VelocityZ) : -707;
                    VelocityZ = max_speed == -707 ? VelocityZ : max_speed;
                    VelocityX = max_speed == -707 ? VelocityX : VelocityX > 0 ? max_speed : -max_speed;
                    // gradually add speed to VelocityZ
                    VelocityZ = forwardPressing ? VelocityZ + Accleration * Time.deltaTime : 0.0f;
                    // gradually add speed to VelocityX
                    VelocityX = Mathf.Abs(VelocityX) < runSpeed ? (leftSidePressing && !rightSidePressing ? VelocityX + Accleration * Time.deltaTime : (!leftSidePressing && rightSidePressing ? VelocityX - Accleration * Time.deltaTime : 0.0f)) : 0.0f;
                }
                else if (!leftConrtrolPressing)
                {
                    isRunning = false;
                    VelocityZ -= VelocityZ > walkSpeed ? (!forwardPressing ? Decceleration * Time.deltaTime : 0.0f) : 0.0f;
                    VelocityX -= Mathf.Abs(VelocityX) > walkSpeed ? (!leftSidePressing ? (!rightSidePressing ? 0.0f : -Decceleration * Time.deltaTime) : (!rightSidePressing ? Decceleration * Time.deltaTime : 0.0f)) : 0.0f;
                }
                VelocityZ = VelocityZ > runSpeed ? runSpeed : VelocityZ;
                VelocityX = Mathf.Abs(VelocityX) > runSpeed ? VelocityX > 0 ? runSpeed : -runSpeed : VelocityX;
                //isRunning = (Mathf.Abs(VelocityX) >= walkSpeed || VelocityZ >= walkSpeed);\
            }
            else
            {
                VelocityZ = !forwardPressing ? 0.0f : VelocityZ;
                VelocityX = !leftSidePressing ? (!rightSidePressing ? 0.0f : VelocityX) : (!rightSidePressing ? VelocityX : 0.0f);
            }
        }

        // combined walk forward movement
        if ((forwardPressing || leftSidePressing || rightSidePressing) && !isWalking)
        {
            VelocityZ = forwardPressing ? 1.0f : 0.0f;
            VelocityX = leftSidePressing ? (rightSidePressing ? 0.0f : 1.0f) : (rightSidePressing ? -1.0f : 0.0f);
            isWalking = true;
        }
        else if ((!forwardPressing || !leftSidePressing || !rightSidePressing) && isWalking && !leftConrtrolPressing)
        {
            VelocityZ = !forwardPressing ? 0.0f : VelocityZ;
            VelocityX = !leftSidePressing ? (!rightSidePressing ? 0.0f : VelocityX) : (!rightSidePressing ? VelocityX : 0.0f);
            // detect if velocity is 0
            isWalking = (VelocityX != 0.0f || VelocityZ != 0.0f);
        }
        isWalking = (forwardPressing || leftSidePressing || rightSidePressing);
    }

}

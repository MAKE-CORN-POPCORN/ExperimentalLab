using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animationStateControllerForFloatState : MonoBehaviour
{
    
    [Header("Debug Zone")]
    public bool doDebug;

    Animator animator;
    float Velocity = 0.0f;
    float Accleration = 0.1f;
    float Deccleration = 0.5f;
    int velocityHash;
    void Start()
    {
        animator = gameObject.GetComponent<Animator>();
        velocityHash = Animator.StringToHash("Velocity");
    }

    // this is the speed set in the blending part (BlendTree)
    float sprintSpeed = 4.0f;
    float runSpeed = 2.0f;
    float walkSpeed = 1.0f;
    float idleSpeed = 0.0f;
    void FixedUpdate()
    {

        bool forwardPressing = Input.GetKey(KeyCode.W);
        if (forwardPressing && Velocity < sprintSpeed)
        {
            // the running speed will gradually increase while holding "w"
            Velocity += Accleration * Time.deltaTime;
        }
        else if (!forwardPressing && Velocity > idleSpeed)
        {
            // and gradually decrease after "w" is released
            Velocity -= Deccleration * Time.deltaTime;
        }

        if (Velocity < idleSpeed)
        {
            Velocity = idleSpeed;
        }else if (Velocity > sprintSpeed)
        {
            Velocity = sprintSpeed;
        }

        animator.SetFloat(velocityHash, Velocity);

    }


}

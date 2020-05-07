using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script moves the character controller forward
// and sideways based on the arrow keys.
// It also jumps when pressing space.
// Make sure to attach a character controller to the same game object.
// It is recommended that you make only one call to Move or SimpleMove per frame.

public class move : MonoBehaviour
{
    CharacterController characterController;

    public float speed = 6.0f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;
    public Transform Camera;

    private Vector3 moveDirection = Vector3.zero;
    private Collider collideR;
    private float distToGround;
    private Quaternion targetRotation;
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        collideR = GetComponent<Collider>();
        distToGround = collideR.bounds.extents.y;
    }

    void Update()
    {
        float input_H = Input.GetAxis("Horizontal");
        float input_V = Input.GetAxis("Vertical");


        //角色在落地時啟動
        if (characterController.isGrounded)
        {
            //方向鍵有按著的時候才會啟動
            if (input_H != 0 || input_V != 0)
            {
                //以camera forward更改角色forward方向
                Vector3 camFor = Camera.transform.forward;
                camFor.y = 0.0f;
                //Debug.Log("camFor : "+ camFor);
                targetRotation = Quaternion.LookRotation(camFor, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
            }
            // We are grounded, so recalculate
            // move direction directly from axes       
            //前進方向local coord.轉world coord.
            moveDirection = transform.TransformDirection(new Vector3(input_H, 0, input_V)/*.normalized*/);

            moveDirection *= speed;

            //按空白鍵時啟動
            if (Input.GetButton("Jump"))
            {
                moveDirection.y = jumpSpeed;
            }
        }

        // Apply gravity. Gravity is multiplied by deltaTime twice (once here, and once below
        // when the moveDirection is multiplied by deltaTime). This is because gravity should be applied
        // as an acceleration (ms^-2)
        //給予重力
        moveDirection.y -= gravity * Time.deltaTime;
        characterController.Move(moveDirection * Time.deltaTime);
    }
    //bool IsGrounded()
    //{
    //    return Physics.Raycast(transform.position, -Vector3.up, distToGround + 0.1f);
    //}
}

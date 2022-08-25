using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallClimb : MonoBehaviour
{
    [Header("References")]
    public Transform orientation;
    public CharacterController controller;
    public LayerMask wall;

    public ThirdPersonControl tpc;


    [Header("WallClimbing")]
    public float wallClimbSpeed;
    public float maxWallClimbTime;
    private float wallClimbTimer;
    public float wallDirection;
    public float wallCounter;
    public float maxWallCounter;


    private bool isWallClimbing;

    [Header("Detection")]
    public float detectionLength;
    public float sphereCastRadius;
    public float maxWallLookAngle;
    private float wallLookAngle;

    private RaycastHit frontWallHit;
    private bool wallFront;





    // Update is called once per frame
    void Update()
    {
        WallCheck();
        StateMachine();

        if (isWallClimbing) WallClimbMovement();
    }

    private void StateMachine()
    {
        //State 1 - Enter WallClimb
        if (wallFront && Input.GetButton("Jump") && wallLookAngle < maxWallLookAngle)
        {
            if (!isWallClimbing && wallClimbTimer > 0 && wallCounter > 0) StartWallClimb();
            Debug.Log("IsWallClimbing");

           
            //timer
            if (wallClimbTimer > 0) wallClimbTimer -= Time.deltaTime;
            if (wallClimbTimer < 0) StopWallClimb();

            //counter
            wallCounter--;
            
        }

        //State 3 - Ending WallClimb
        else
        {
            if (isWallClimbing) StopWallClimb();
            Debug.Log("WallClimbing Ended");
        }
    }

    private void WallCheck()
    {
        wallFront = Physics.SphereCast(transform.position, sphereCastRadius, orientation.forward, out frontWallHit, detectionLength, wall);
        wallLookAngle = Vector3.Angle(orientation.forward, -frontWallHit.normal);

        Vector3 forward = transform.TransformDirection(Vector3.forward) * detectionLength;
        Debug.DrawRay(orientation.position, forward, Color.red);

        if (tpc.isGrounded)
        {
            wallClimbTimer = maxWallClimbTime;
            wallCounter = maxWallCounter;
        }

    }

    private void StartWallClimb()
    {
            isWallClimbing = true;
      

    }

    private void WallClimbMovement()
    {

        //Need Controls to follow Player Velocity

        //This doesn't work as intended
        Vector3 move = new Vector3(0f , Input.GetAxisRaw("Vertical"), 0f).normalized;
        controller.Move(move * Time.deltaTime * wallClimbSpeed);
        
    }

    private void StopWallClimb()
    {
        isWallClimbing = false;
    }
}

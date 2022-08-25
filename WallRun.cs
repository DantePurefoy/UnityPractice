using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRun : MonoBehaviour
{
    [Header("Wallrunning")]
   
    public LayerMask Wall;
    public LayerMask Ground;
    public float wallRunSpeed;
    public float maxWallRunTime;
    public float wallRunTimer;

    [Header("Input")]
    private float horizontalInput;
    private float verticalInput;

    [Header("Detection")]
    public float wallCheckDistance;
    public float minJumpHeight;
    private RaycastHit leftWallHit;
    private RaycastHit rightWallHit;
    private bool wallLeft;
    private bool wallRight;
    

    public float wallStickForce = 100f;

    [Header("Refernces")]
    public Transform orientation;
    private ThirdPersonControl tpc;
    private CharacterController controller;

    public WallClimb wc;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        tpc = GetComponent<ThirdPersonControl>();
        wc = GetComponent<WallClimb>();
    }

    private void Update()
    {
        CheckForWall();
        WallRunStateMachine();
    }

    private void FixedUpdate()
    {
        if (tpc.isWallRunning)
            WallRunMovement();
    }

    private void CheckForWall()
    {

        
        wallLeft = Physics.Raycast(transform.position, -orientation.right, out leftWallHit, wallCheckDistance, Wall);

        Vector3 left = transform.TransformDirection(Vector3.left) * wallCheckDistance;

        wallRight = Physics.Raycast(transform.position, orientation.right, out rightWallHit, wallCheckDistance, Wall);

        Vector3 right = transform.TransformDirection(Vector3.right) * wallCheckDistance;
        
        Debug.DrawRay(orientation.position, right, Color.green);
        Debug.DrawRay(orientation.position, left, Color.cyan);
      
    }

    private bool AboveGround()
    {
        return !Physics.Raycast(transform.position, Vector3.down, minJumpHeight, Ground);
    }

    private void WallRunStateMachine()
    {
        // Getting Inputs
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        //State 1 - Enter WallRun
        if ((wallLeft || wallRight) && verticalInput > 0 && wc.wallCounter > 0 && tpc.isGrounded /*AboveGround()*/)
        {
            if (!tpc.isWallRunning)
                StartWallRun();

            


            Debug.Log("IsWallRunning");
        }

        //State 3 - Ending WallRun
        else
        {
            if (tpc.isWallRunning)
                StopWallRun();
            Debug.Log("WallRunning Ended");
        }
    }

    private void StartWallRun()
    {
        
        tpc.isWallRunning = true;
        //counter
        wc.wallCounter--;
    }

    private void WallRunMovement()
    {
        tpc.gravityOn = false;

        Vector3 wallNormal = wallRight? rightWallHit.normal : leftWallHit.normal;
        

        Vector3 wallForward = Vector3.Cross(wallNormal, transform.up) .normalized;

        ////allows wallrunning in either direction
        if ((orientation.forward - wallForward).magnitude > (orientation.forward - -wallForward).magnitude)
        {
            wallRunSpeed = -Mathf.Abs(wallRunSpeed);
            Debug.Log("Detection Works!");
        }

        //actual wallrun movement
        Vector3 move = new Vector3(Input.GetAxisRaw("Vertical")/* & Input.GetButton("Jump")*/, 0f, 0f).normalized;
        controller.Move(move * Time.deltaTime * wallRunSpeed);

        Vector3 move2 = new Vector3(Input.GetAxisRaw("Horizantal"), 0f, 0f).normalized;
        controller.Move(move2 * Time.deltaTime * wallRunSpeed);



        //stick to wall (need to figure out how to force the character agaist the wall durring wallrun)
        //Vector3 wallStick = new Vector3(0f, 0f, wallNormal);

        //if (!(wallLeft && horizontalInput > 0) && !(wallRight && horizontalInput < 0))
        //    controller.Move(wallStick * wallStickForce * Time.deltaTime);

    }

    private void StopWallRun()
    {
        tpc.isWallRunning = false;
        tpc.gravityOn = true;
        wallRunSpeed = Mathf.Abs(wallRunSpeed);
        
    } 

    
}

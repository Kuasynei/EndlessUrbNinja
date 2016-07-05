using UnityEngine;
using System.Collections;

public class DashMovement : MonoBehaviour {

    //Public Variables
    [SerializeField] float dragDashTimeLimit = 1;
    [SerializeField] int dashSpeed = 100;

    //Private Variables
    private Camera gCam;
    private Vector3 dashTarget;
    private Vector3 dragDashDirection;
    private Rigidbody rb;
    private bool canDragDash = false;
    private bool isDashing = false;


    //Dash timer variables @note: This is to properly reset the gravity
    [SerializeField] float dashTime;
    private float dashStartTime;

    //Private Variables for the Held Button Dash
    private float timeButtonHeldDown;
    private float startTime;
    private bool buttonHeldDown = false;


	// Use this for initialization
	void Start () {

        //Store the main camera so a ray can be sent from the mouses position relative to it
        gCam = Camera.main;
       
        //Store the Rigidbody on the player for faster access
        rb = GetComponent<Rigidbody>();

	}
	
	// Update is called once per frame
	void Update () {

        //This is a fast way of saying Mouse 1, touch screen or CTRL
        if (Input.GetButtonDown("Fire1"))
        {

            buttonHeldDown = true;
            startTime = Time.time;
            //Keeps Update clean by having all the mouse/touch behaviour in another function
            mouseHandler();
            
        }
        //Added for the drag dash fire. It runs the dragDashHandler function as well as tells the game that
        //The player can no longer dragdash and to stop the held button count down.
        if(Input.GetButtonUp("Fire1"))
        {
            if (canDragDash && timeButtonHeldDown > 0.3f)
            {
                Debug.Log("Running");
                dragDashHandler();
            }
            canDragDash = false;
            buttonHeldDown = false;
        }

        //Checks if the players finger is pressed on the screen
        if (buttonHeldDown == true)
        {
            //If the timer still needs to count up then do so
            if(timeButtonHeldDown < dragDashTimeLimit)
            {
                timeButtonHeldDown = CalculateTimePassed(startTime);
            }
        }
        //If the timer runs out make it so that the dash does not run.
        if(timeButtonHeldDown > dragDashTimeLimit)
        {
            canDragDash = false;
        }

        //Creates a timer for dash that turns gravity back on.
        if(CalculateTimePassed(dashStartTime) > 0.5f)
        {
            rb.useGravity = true;
        }
    }

    void mouseHandler()
    {
        //reset the time held down
        timeButtonHeldDown = 0;

        //Resets the time scale so the dash is still fast
        if (Time.timeScale != 1)
        {
            Time.timeScale = 1;
        }

        //Create a Hit object to store the following raycast in
        RaycastHit hit;
        //Send a ray from the mouses current position relative to the camera that is 100 units long 
        //(@note: Not sure if this value matters, need more testing), and stores it in the Raycast Hit object
        if (Physics.Raycast(gCam.ScreenPointToRay(Input.mousePosition), out hit, 100))
        {
            if (hit.collider.CompareTag("Enemy"))
            {

                //Sets the dash target to the object that was hit as long as it hits an enemy.
                dashTarget = hit.collider.gameObject.transform.position;

                Debug.Log(hit.collider.gameObject.transform.position);
                Debug.DrawLine(transform.position, hit.collider.gameObject.transform.position, Color.red, 3);

                //Set no gravity so the player doesnt immediately start flying down, reset the player motion so the dash works properly.
                rb.useGravity = false;
                rb.velocity = Vector3.zero;

                //Add force at the point between the altered dashTarget and the players current position multiplied by the movement force.
                rb.AddForce((dashTarget - transform.position) * dashSpeed);

                dashStartTime = Time.time;

                //Allow the system to start the drag dash
                canDragDash = true;
                

            }
        }
    }

    void dragDashHandler()
    {
        //Start by getting the mouses raw position
        dragDashDirection = Input.mousePosition;

        //Turn off gravity and stop all movement like before.
        rb.useGravity = false;
        rb.velocity = Vector3.zero;

        //You have to set a Z-Depth for the Screen to world point to work. At the moment it is set to the cameras Z position away from the player.
        dragDashDirection.z = 9.8f;

        //Add force at the point between the altered dragDashDirection and the players current position multiplied by the movement force.
        rb.AddForce((gCam.ScreenToWorldPoint(dragDashDirection) - transform.position) * dashSpeed);

        dashStartTime = Time.time;

    }

    float CalculateTimePassed(float startTime)
    {
        return Time.time - startTime;
    }
       
}

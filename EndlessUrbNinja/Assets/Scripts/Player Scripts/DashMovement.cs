using UnityEngine;
using System.Collections;

public class DashMovement : MonoBehaviour {

    //Private Variables
    private Camera gCam;
    private Vector3 dashTarget;
    private Vector3 dragDashDirection;
    private Rigidbody rb;
    private bool canDragDash = false;

    //Private Variables for the Held Button Dash
    private float timeButtonHeldDown;
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
            //Keeps Update clean by having all the mouse/touch behaviour in another function
            mouseHandler();
            
        }
        //Added for the drag dash fire. It runs the dragDashHandler function as well as tells the game that
        //The player can no longer dragdash and to stop the held button count down.
        //@note: Not working yet.
        if(Input.GetButtonUp("Fire1"))
        {
            if (canDragDash)
            {
                Debug.Log("Running");
                //dragDashHandler();
            }
            canDragDash = false;
            buttonHeldDown = false;
        }

        //Checks the distance between the player and the dash target. Also slows down time so the player has
        //More time to react to the next point. Needs to be made smoother and needs tweaking
        //@note: Can be changed and made dynamic so it can check not only
        //The dash target but also the drag dash target.
        if(Vector3.Distance(transform.position, dashTarget) < 0.5f)
        {
            rb.useGravity = true;
            Time.timeScale = 0.4f;
        }


        //Starting code for the drag dash
        if(buttonHeldDown == true)
        {
            timeButtonHeldDown++;
        }
        if(timeButtonHeldDown > 10)
        {
            canDragDash = false;
        }

    }

    void mouseHandler()
    {
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
                Debug.Log("Target Set");

                //Sets the dash target to the object that was hit as long as it hits an enemy.
                dashTarget = hit.collider.gameObject.transform.position;

                Debug.Log(hit.collider.gameObject.transform.position);
                Debug.DrawLine(transform.position, hit.collider.gameObject.transform.position, Color.red, 3);

                //Set no gravity so the player doesnt immediately start flying down, reset the player motion so the dash works properly.
                rb.useGravity = false;
                rb.velocity = Vector3.zero;

                //Get the position between the dashTarget and the players current position and multiply it by the 100 scalar to maek it fast
                rb.AddForce((dashTarget - transform.position) * 100);

                //Allow the system to start the drag dash
                canDragDash = true;
                

            }
            else
            //Just a debug that shows if the player clicked something that wasnt the enemy. Useful for debugging.
            Debug.Log(hit.collider.name);
        }
    }

    //void dragDashHandler()
    //{
    //    dragDashDirection = gCam.(Input.mousePosition);

    //    Debug.DrawLine(transform.position, dragDashDirection, Color.green, 3);

    //    rb.useGravity = false;
    //    rb.velocity = Vector3.zero;
    //    rb.AddForce((dragDashDirection - transform.position) * 100);
    //}
}

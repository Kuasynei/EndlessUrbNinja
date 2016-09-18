using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	[Header("Auto Run")]
	[SerializeField] float accelerationSpeed;
	[SerializeField] float maxRunSpeed;
	[SerializeField] float catchupMaxSpeed;

	[Header("Dashing")]
    [SerializeField] int dashSpeed = 50; // The speed the player will dash at during first, and second dashes.
	[SerializeField] float dragDashMinimumThreshold = 3; //If the player's drag dash is not longer than this distance, the player 'pops-up' with a default dash.
	[SerializeField] float dragDashMaximumDistance = 10; //The player cannot drag dash longer than this distance. 
	[SerializeField] float afterDragDashCatchBuffer = 1; //The size of the "Net" used to catch the player after a drag dash to activate the dampener.
	[SerializeField] float afterDragDashDampener = 1; //How much to reduce the player's velocity after a drag dash so they don't go into space.

    private Camera gCam; //Game Camera
	private Rigidbody rb; //Getting rigidbody component

	private bool isTouchingGround = true; //Used to determine autorun behaviours.

	private Vector3 nullPosition = new Vector3(-100,-100,-100); //A physically unreachable position.
	private Vector3 dashTarget; //When an enemy is tapped on, the dash target becomes the coordinates of the enemy.
	private Vector3 dragDashTarget; //Determines the location the player will dash to after the destruction of an enemy.
    private bool canDragDash = false; 
    private bool isDashing = false;
    private GameObject currentlyTargetedEnemy; //The gameobject belonging to the coordinates of the dashTarget.


    private bool buttonHeldDown = false; //True if the player is holding their finger down on the screen.

	void Awake(){
		//Store the main camera so a ray can be sent from the mouses position relative to it
		gCam = Camera.main;

		//Store the Rigidbody on the player for faster access
		rb = GetComponent<Rigidbody>();

		dashTarget = nullPosition;
		dragDashTarget = nullPosition;
	}

	void OnDrawGizmos()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere (dragDashTarget, afterDragDashCatchBuffer);
	}

	// Update is called once per frame
	void Update () {
        //This is a fast way of saying Mouse 1, touch screen or CTRL
        if (Input.GetButtonDown("Fire1"))
        {

            buttonHeldDown = true;
            //Keeps Update clean by having all the mouse/touch behaviour in another function
            mouseButtonDownHandler();
            
        }
        //Added for the drag dash fire. It runs the dragDashHandler function as well as tells the game that
        //The player can no longer dragdash and to stop the held button count down.
        if(Input.GetButtonUp("Fire1"))
        {
            buttonHeldDown = false;

			//Visuals for the drag-dash on mouse-release.
			if (isDashing)
			{
				if (Vector3.Distance (dashTarget, dragDashTarget) < dragDashMaximumDistance)
				{
					Debug.DrawLine (dashTarget, dragDashTarget, Color.yellow, 2f);
				}
				else
				{
					Debug.DrawRay (dashTarget, (dragDashTarget - dashTarget).normalized * dragDashMaximumDistance, Color.yellow, 2f);
				}
			}
        }

		AutoRunHandler (); //#MakeUpdateCleanAgain2016

        //Checks if the players finger is pressed on the screen
        if (buttonHeldDown == true)
        {
			//Some debug visuals to see when drag dashing is possible.
			if (canDragDash)
			{
				dragDashHandler (); //Keeping Update() Clean!

				//Visuals for the drag-dash while mouse is held down.
				if (isDashing)
				{
					if (Vector3.Distance (dashTarget, dragDashTarget) < dragDashMaximumDistance)
					{
						Debug.DrawLine (dashTarget, dragDashTarget, Color.yellow);
					}
					else
					{
						Debug.DrawRay (dashTarget, (dragDashTarget - dashTarget).normalized * dragDashMaximumDistance, Color.yellow);
					}
				}

			}
        }

		//DRAG DASH LIMITER (This little block makes sure that once you reach your drag dash destination, you don't explode.)
		if (Vector3.Distance (transform.position, dragDashTarget) < afterDragDashCatchBuffer && canDragDash == false)
		{
			rb.drag = afterDragDashDampener;

			dragDashTarget = nullPosition;
			rb.useGravity = true;
			isDashing = false;
		}

		if (rb.drag > 1)
		{
			rb.drag -= (rb.drag * 0.8f) * (Time.deltaTime * 10);
		}
		else
		{
			rb.drag = 0;
		}
    }

	void AutoRunHandler ()
	{
		if (!isDashing) //Don't apply auto-run force while in the player is dashing.
		{
			//If the player is behind the ideal X position, we speed them up so that they can recover from falling off the left side of the screen.
			if (rb.velocity.magnitude <= catchupMaxSpeed && isTouchingGround && transform.position.x < GlobalReferences.idealXPosition)
			{
				//Accelerate by the difference in position so that the player will speed up the closer they are to death, 
				//and slow down when safe (but not slower than the default max run speed.)
				rb.AddForce (Vector3.right * (Mathf.Max (GlobalReferences.idealXPosition - transform.position.x, maxRunSpeed)) * (Time.deltaTime * 100));
			} 
			//Check if the object is going too fast as well if it is on the ground.
			//If the player is not on on the ground then dont push it forward at all
			else if (rb.velocity.magnitude <= maxRunSpeed)
			{
				rb.AddForce (Vector3.right * accelerationSpeed * (Time.deltaTime * 100));
			}
		}
	}

	public float GetMaxRunSpeed()
	{
		return maxRunSpeed;
	}

    void mouseButtonDownHandler()
    {
        //Resets the time scale so the dash is still fast
        if (Time.timeScale != 1)
        {
            Time.timeScale = 1;
        }

        //Create a Hit object to store the following raycast in
        RaycastHit hit;

        //Using Bitshifting select the layer that we want to raycast on only, then use the ~
        //to make it do the exact opposite.
        int layerMask = 1 << 8;

        //Send a ray from the mouses current position relative to the camera that is 100 units long 
        //(@note: Not sure if this value matters, need more testing), and stores it in the Raycast Hit object
        if (Physics.Raycast(gCam.ScreenPointToRay(Input.mousePosition), out hit, 100, ~layerMask))
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                //Sets the dash target to the object that was hit as long as it hits an enemy.
                dashTarget = hit.collider.transform.position;

                //Debug.Log(hit.collider.gameObject.transform.position);
				Debug.DrawLine (transform.position, dashTarget, Color.red, 3);

                //Set no gravity so the player doesnt immediately start flying down, reset the player motion so the dash works properly.
                rb.useGravity = false;
                rb.velocity = Vector3.zero;
				rb.drag = 0;

                //Add force at the point between the altered dashTarget and the players current position multiplied by the movement force.
				rb.velocity = (dashTarget - transform.position).normalized * dashSpeed;

                //Allow the system to start the drag dash
                canDragDash = true;
                isDashing = true;


            }
        }
    }

	//This function handles the drag dash's functions up until launch.
	void dragDashHandler()
	{
		//Start by getting the mouses raw position.
		dragDashTarget = Input.mousePosition;

		//You have to set a Z-Depth for the Screen to world point to work. At the moment it is set to the cameras Z position away from the player.
		dragDashTarget.z = Mathf.Abs(GlobalReferences.cameraHandler.transform.position.z);

		//Converting the mouse location to a world position.
		dragDashTarget = (gCam.ScreenToWorldPoint (dragDashTarget));

		//If the dragDashTarget puts the player on a collision path with terrain, stop the drag dash there so they don't charge into terrain.
		RaycastHit[] hits = Physics.SphereCastAll(transform.position, 1, dragDashTarget - transform.position, (dragDashTarget - transform.position).magnitude);
		if (hits.Length > 0)
		{
			float closestTerrain = Vector3.Distance(transform.position, dragDashTarget);
			for (int i = 0; i < hits.Length; i++)
			{
				//If you've hit terrain, Mark its distance, and place the dragDashTarget at the closest terrain-collision position.
				if (hits [i].collider.CompareTag ("Terrain") && Vector3.Distance (transform.position, hits [i].point) < closestTerrain)
				{
					closestTerrain = Vector3.Distance (transform.position, hits [i].point);
				}
			}

			//If the path between the player and the dragDashTarget is not free of terrain, then pull the target back so the collision won't happen.
			if (closestTerrain < Vector3.Distance(transform.position, dragDashTarget))
			{
				dragDashTarget = (dragDashTarget - transform.position).normalized * closestTerrain + transform.position;
			}
		}

		//If the dragDashTarget is close enough to an enemy, a default "pop-up" dash is performed. A bit of velocity is retained for a more natural feeling.
		if (Vector3.Distance (dashTarget, dragDashTarget) < dragDashMinimumThreshold)
		{
			dragDashTarget = dashTarget + Vector3.up * dragDashMinimumThreshold + rb.velocity / 100;
		}
		//If the dragDashTarget goes beyond the maximum allowed distance, pull it back to the nearest possible position.
		else if (Vector3.Distance (dashTarget, dragDashTarget) > dragDashMaximumDistance)
		{
			dragDashTarget = dashTarget + (dragDashTarget - dashTarget).normalized * dragDashMaximumDistance;
		}
	}

	//This function handles the drag dash's actual launch.
    void dragDashLaunch()
    {
        //Turn off gravity and stop all movement like before.
        rb.useGravity = false;
        rb.velocity = Vector3.zero;

        //Add force at the point between the altered dragDashDirection and the players current position multiplied by the movement force.
		rb.velocity = (dragDashTarget - transform.position).normalized * dashSpeed;
		Debug.DrawRay (transform.position, (dragDashTarget - transform.position) * (dashSpeed/100), Color.green, 2f);

		canDragDash = false; //Disable the player's ability to drag dash after launch.
    }


    //Simple time calculator that tells how much time has passed.
    float CalculateTimePassed(float startTime)
    {
        return Time.time - startTime;
    }

    void OnTriggerEnter(Collider colInfo)
    {
		//An enemy was hit.
        if(colInfo.gameObject.tag == "Enemy") 
        {
            //Check if the player is currently dashing
            if(isDashing)
            {
				//If the player is in a dash and contacts an enemy, kill them.
				colInfo.gameObject.SendMessage ("Die");

				//Determine the appropriate course of action after killing an enemy.
				if (canDragDash && dragDashTarget != nullPosition)
				{
					dragDashLaunch ();
				}
            }
            else
            {
				Die();
            }
        }

		if (colInfo.CompareTag ("DeathBoundary"))
		{
			Die ();
		}
    }
       
	public void Die()
	{
		GlobalReferences.gameController.GameOver ();

		dragDashTarget = nullPosition;
		rb.useGravity = true;
		isDashing = false;
		rb.velocity = Vector3.zero;

		gameObject.SetActive (false);
	}
}

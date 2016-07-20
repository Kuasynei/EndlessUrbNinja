using UnityEngine;
using System.Collections;

public class AutoRunMovement : MonoBehaviour {

    //private variables
    private Rigidbody rb;
    private bool isTouchingGround = true;

    //publicly set variables
    [SerializeField] float accelerationSpeed;
    [SerializeField] float maxRunSpeed;
	[SerializeField] float catchupMaxSpeed;

    // Use this for initialization
    void Start () {

        rb = GetComponent<Rigidbody>();
	
	}
	
	// Update is called once per frame
	void Update () {

		//If the player is behind the ideal X position, we speed them up so that they can recover from falling off the left side of the screen.
		if (rb.velocity.magnitude <= catchupMaxSpeed && isTouchingGround && transform.position.x < GlobalReferences.idealXPosition)
		{
			//Accelerate by the difference in position so that the player will speed up the closer they are to death, 
			//and slow down when safe (but not slower than the default max run speed.)
			rb.AddForce(Vector3.right * (Mathf.Max(GlobalReferences.idealXPosition - transform.position.x, maxRunSpeed)) * (Time.deltaTime * 100));
		} 
		//Check if the object is going too fast as well if it is on the ground.
		//If the player is not on on the ground then dont push it forward at all
		else if(rb.velocity.magnitude <= maxRunSpeed && isTouchingGround)
        {
			rb.AddForce(Vector3.right * accelerationSpeed * (Time.deltaTime * 100));
        }
    }

    //Collision Check to see if the player is standing on the ground.
    //@note: Started with Stay instead of Enter, found there was too much delay with stay.
    void OnCollisionEnter(Collision colInfo)
    {
        if(colInfo.gameObject.CompareTag("Ground"))
        {
            isTouchingGround = true;
        }
    }

    //Similar to the function above just changes the bool if the player leaves the ground.
    void OnCollisionExit(Collision colInfo)
    {
        if (colInfo.gameObject.CompareTag("Ground"))
        {
            isTouchingGround = false;
        }
    }

	public float GetMaxRunSpeed()
	{
		return maxRunSpeed;
	}
}

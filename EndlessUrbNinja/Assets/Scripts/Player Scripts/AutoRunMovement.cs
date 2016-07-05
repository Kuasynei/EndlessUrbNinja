using UnityEngine;
using System.Collections;

public class AutoRunMovement : MonoBehaviour {

    //private variables
    private Rigidbody rb;
    private bool isTouchingGround = true;

    //publicly set variables
    [SerializeField] float movementSpeed;
    [SerializeField] float maxRunSpeed;


    // Use this for initialization
    void Start () {

        rb = GetComponent<Rigidbody>();
	
	}
	
	// Update is called once per frame
	void Update () {

        //Check if the object is going to fast as well if it is on the ground.
        //If the player is not on on the ground then dont push it forward at all
        if(rb.velocity.magnitude <= maxRunSpeed && isTouchingGround)
        {
            rb.AddForce(Vector3.right * movementSpeed);
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
}

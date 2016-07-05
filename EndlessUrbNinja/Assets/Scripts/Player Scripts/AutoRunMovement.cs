using UnityEngine;
using System.Collections;

public class AutoRunMovement : MonoBehaviour {

    //private variables
    private Rigidbody rb;

    //publicly set variables
    [SerializeField] float movementSpeed;
    [SerializeField] float maxRunSpeed;


    // Use this for initialization
    void Start () {

        rb = GetComponent<Rigidbody>();
	
	}
	
	// Update is called once per frame
	void Update () {

        if(rb.velocity.magnitude <= maxRunSpeed)
        {
            rb.AddForce(Vector3.right * movementSpeed);
        }

        Debug.Log(rb.velocity.magnitude);
    }
}

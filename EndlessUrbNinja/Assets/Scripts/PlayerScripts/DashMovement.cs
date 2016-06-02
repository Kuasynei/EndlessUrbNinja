using UnityEngine;
using System.Collections;

public class DashMovement : MonoBehaviour {

    //Private Variables
    private Camera gCam;

	// Use this for initialization
	void Start () {

        //Store the main camera so a ray can be sent from the mouses position relative to it
        gCam = Camera.main;

	}
	
	// Update is called once per frame
	void Update () {

        //This is a fast way of saying Mouse 1, touch screen or CTRL
        if (Input.GetButtonDown("Fire1"))
        {
            //Keeps Update clean by having all the mouse/touch behaviour in another function
            mouseHandler();
        }

    }

    void mouseHandler()
    {
        //Create a Hit object to store the following raycast in
        RaycastHit hit;
        //Send a ray from the mouses current position relative to the camera that is 100 units long 
        //(@note: Not sure if this value matters, need more testing), and stores it in the Raycast Hit object
        if (Physics.Raycast(gCam.ScreenPointToRay(Input.mousePosition), out hit, 100))
        {
            //Quick test to see if it is hitting only one object specifically.
            Debug.Log(hit.collider.name);
        }
    }
}

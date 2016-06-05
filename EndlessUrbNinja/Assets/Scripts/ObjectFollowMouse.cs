using UnityEngine;
using System.Collections;

public class ObjectFollowMouse : MonoBehaviour {

    private Vector3 mousePos;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        mousePos = Input.mousePosition;

        mousePos.z = 9.8f;

        Debug.Log(mousePos);

        transform.position = Camera.main.ScreenToWorldPoint(mousePos);
	}
}

using UnityEngine;
using System.Collections;

public class UTIL_Move : MonoBehaviour {

	[SerializeField] Vector3 moveVector;
	
	// Update is called once per frame
	void Update () {
		transform.position += moveVector * Time.deltaTime;
	}
}

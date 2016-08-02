using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class PlayerStatusChanges : MonoBehaviour {

	Rigidbody RB;

	void Awake()
	{
		RB = GetComponent<Rigidbody> ();
	}

	void OnTriggerEnter(Collider coll)
	{
		//If you touch a death boundary, you ded. 
		//Dying to enemies is called by the dash script because that script enables the player's invunlerability as well.
		//Although we may want to move it elsewhere.
		if (coll.CompareTag ("DeathBoundary"))
		{
			GameOver ();
		}
	}

	public void GameOver()
	{
		GlobalReferences.gameController.GameOver ();
		RB.velocity = Vector3.zero;
		gameObject.SetActive (false);
	}
}

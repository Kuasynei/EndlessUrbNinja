using UnityEngine;
using System.Collections;

public class PlayerGameover : MonoBehaviour {

	void OnTriggerEnter(Collision collInfo)
	{
		if (collInfo.collider.CompareTag ("DeathBoundary"))
		{
			GlobalReferences.gameController.GameOver ();
		}
	}
}

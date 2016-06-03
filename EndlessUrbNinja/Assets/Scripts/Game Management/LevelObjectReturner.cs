using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class LevelObjectReturner : MonoBehaviour {

	GameController GC;

	void Start()
	{
		GC = GlobalReferences.gameController;
	}

	void OnTriggerEnter(Collider otherColl)
	{
		GC.DespawnLevel ();
	}
}

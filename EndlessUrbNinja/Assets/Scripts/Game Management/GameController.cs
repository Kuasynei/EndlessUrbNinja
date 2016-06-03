using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof(LevelObjectPool))]
public class GameController : MonoBehaviour {

	[SerializeField] bool debugMode = false;

	Vector3 levelSpawnOffset = new Vector3(20, 0, 0); //Where levels will spawn by default.

	LevelObjectPool lvlPool; //Getting a reference to the level pool attached to this gameobject.

	GameObject activeLevelsRoot; //A gameobject to hold all levels that are currently ACTIVE.
	List<LevelPackage> activeLevels = new List<LevelPackage>();

	bool levelSpawnAllowed = true; //If there is room for the level to spawn, try spawning it (among other conditions).
	int metersProgressed = 0;

	void Awake()
	{
		GlobalReferences.gameController = this;

		lvlPool = GetComponent<LevelObjectPool>(); 
		lvlPool.PrepareNewLevels (0); //Adds levels of the specified difficulty to the level pool.

		activeLevelsRoot = new GameObject ("Active Levels");
	}

	void Update()
	{
		metersProgressed = Mathf.RoundToInt(transform.position.x);
	}

	void FixedUpdate()
	{
		if (debugMode)
		{
			Debug.Log ("levelSpawnAllowed = " + levelSpawnAllowed);
		}

		////SPAWN LEVEL CODE
		if (levelSpawnAllowed)
		{
			Vector3 levelSpawnPosition = transform.position + levelSpawnOffset;

			LevelPackage newActiveLevel = lvlPool.PopOffPool ();
			activeLevels.Add (newActiveLevel);
	
			newActiveLevel.level.transform.SetParent (activeLevelsRoot.transform);
			newActiveLevel.level.transform.position = levelSpawnPosition;
		}
	}

	void OnTriggerEnter(Collider otherColl)
	{
		levelSpawnAllowed = false;
	}

	void OnTriggerExit(Collider otherColl)
	{
		levelSpawnAllowed = true;
	}

	public void DespawnLevel()
	{
		lvlPool.AddToPool (activeLevels[0]);
		activeLevels.RemoveAt (0);
	}
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

[RequireComponent (typeof(LevelObjectPool))]
public class GameController : MonoBehaviour {
	
	[SerializeField] bool debugMode = false;
	[SerializeField] bool randomize = true;
	[SerializeField] float startDelay = 3; //Only start spawning after this much time has passed (so we don't spawn within the starting block).
	[SerializeField] CameraHandler cameraHandler;
	[SerializeField] Text[] distanceText;
	[SerializeField] GameObject playerCharacter;

	LevelObjectPool lvlPool; //Getting a reference to the level pool attached to this gameobject.
	GameObject activeLevelsRoot; //A gameobject to hold all levels that are currently ACTIVE.
	List<LevelPackage> activeLevels = new List<LevelPackage>();

	Vector3 levelSpawnOffset; //Where levels will spawn by default.
	bool levelSpawnAllowed = true; //If there is room for the level to spawn, try spawning it (among other conditions).
	int levelHeightVariance = 2;
	float idealXPositionOffset = -4;

	Vector3 playerSpawnOffset = new Vector3(-23, 0, 0);

	void Awake()
	{
		GlobalReferences.gameController = this;

		lvlPool = GetComponent<LevelObjectPool>(); 
		lvlPool.PrepareNewLevels (1); //Adds levels of the specified difficulty to the level pool.
		activeLevelsRoot = new GameObject ("Active Levels");

		levelSpawnOffset = GetComponent<BoxCollider> ().center;

		StartGame (); //MOVE THIS TO A BUTTON
	}

	void Update()
	{
		foreach (Text uiText in distanceText)
		{
			uiText.text = Mathf.RoundToInt (transform.position.x).ToString();
		}
	}

	void FixedUpdate()
	{
		GlobalReferences.distanceRun = Mathf.RoundToInt(transform.position.x);
		GlobalReferences.idealXPosition = transform.position.x + idealXPositionOffset;

		if (debugMode)
		{
			Debug.Log ("levelSpawnAllowed = " + levelSpawnAllowed);
		}

		if (startDelay > 0)
		{
			startDelay -= Time.deltaTime;
		}

		////SPAWN LEVEL CODE
		if (levelSpawnAllowed && startDelay <= 0)
		{
			Vector3 levelSpawnPosition = transform.position + levelSpawnOffset + (Vector3.up * Random.Range(-levelHeightVariance, levelHeightVariance));

			if (randomize)
			{
				LevelPackage newActiveLevel = lvlPool.PopOffPoolRand ();
				activeLevels.Add (newActiveLevel);

				newActiveLevel.level.transform.SetParent (activeLevelsRoot.transform);
				newActiveLevel.level.transform.position = levelSpawnPosition;
			}
			else
			{
				LevelPackage newActiveLevel = lvlPool.PopOffPool ();
				activeLevels.Add (newActiveLevel);

				newActiveLevel.level.transform.SetParent (activeLevelsRoot.transform);
				newActiveLevel.level.transform.position = levelSpawnPosition;
			}
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

	//Despawn level removes the oldest level to have been created. Since we're constantly moving to the right, this will always be the oldest level, the value at 0.
	public void DespawnLevel()
	{
		lvlPool.AddToPool (activeLevels[0]);
		activeLevels.RemoveAt (0);
	}

	public void StartGame()
	{
		cameraHandler.enabled = true;
		cameraHandler.SetTargetPlayer (playerCharacter);
	}

	//Player ded.
	public void GameOver()
	{
		
	}
}

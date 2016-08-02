using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

[RequireComponent (typeof(LevelObjectPool))]
public class GameController : MonoBehaviour {
	
	[SerializeField] bool randomize = true;
	[SerializeField] CameraHandler cameraHandler;
	[SerializeField] GameObject playerCharacter;
	[SerializeField] Text[] distanceText;
	[SerializeField] GameObject retryButton;

	LevelObjectPool lvlPool; //Getting a reference to the level pool attached to this gameobject.
	GameObject activeLevelsRoot; //A gameobject to hold all levels that are currently ACTIVE.
	List<LevelPackage> activeLevels = new List<LevelPackage>();

	Vector3 levelSpawnOffset; //Where levels will spawn by default.
	bool levelSpawnAllowed = false; //If there is room for the level to spawn, try spawning it (among other conditions).
	int levelHeightVariance = 2;
	float idealXPositionOffset = -4;

	Vector3 playerSpawnOffset = new Vector3(-23, 0, 0);
	Vector3 defaultStartingCameraPosition = new Vector3(0, 0, -25);

	bool gameRunning = false;

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
		if (gameRunning)
		{
			foreach (Text uiText in distanceText)
			{
				uiText.text = Mathf.RoundToInt (transform.position.x).ToString ();
			}
		}
	}

	void FixedUpdate()
	{
		GlobalReferences.distanceRun = Mathf.RoundToInt(transform.position.x);
		GlobalReferences.idealXPosition = transform.position.x + idealXPositionOffset;

		////SPAWN LEVEL CODE
		if (levelSpawnAllowed)
		{
			//Levels are spawned if they aren't going to overlap another level, 
			//at a position relative to the camera, with some variation in height.
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

	//Despawn level removes the oldest level to have been created. 
	//Since we're constantly moving to the right, this will always be the oldest level, the value at 0.
	public void DespawnLevel()
	{
		if (activeLevels.Count > 0)
		{
			lvlPool.AddToPool (activeLevels [0]);
			activeLevels.RemoveAt (0);
		}
		else
		{
			Debug.Log ("A script tried to despawn a level that didn't exist. - GameController");
		}
	}

	public void StartGame()
	{
		gameRunning = true;

		//Despawn all levels (manually) that are currently active.
		for (int i = 0; i < activeLevels.Count; i++)
		{
			lvlPool.AddToPool (activeLevels[0]);
			activeLevels.RemoveAt (0);
		}

		if (activeLevels.Count > 0)
		{
			for (int i = 0; i < activeLevels.Count; i++)
			{
				lvlPool.AddToPool (activeLevels [0]);
				activeLevels.RemoveAt (0);
			}
		}
		//Return to the original camera position.
		cameraHandler.transform.position = defaultStartingCameraPosition;

		//Respawn the player character.
		playerCharacter.transform.position = playerSpawnOffset;
		playerCharacter.SetActive (true);

		cameraHandler.SetMovementMode(CameraMode.running);

		retryButton.SetActive (false);
	}

	//Player ded.
	public void GameOver()
	{
		gameRunning = false;
		cameraHandler.SetMovementMode(CameraMode.slowStop);

		retryButton.SetActive (true);
	}
}

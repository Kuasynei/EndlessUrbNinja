using UnityEngine;
using System.Collections;

[System.Serializable]
public class LevelPackage
{
	public GameObject level;
	public float difficulty;

	public void WakeFromPooling() //This function is run whenever this level is being pulled out of a pool.
	{
		RefreshEnemyList ();
	}

	private void RefreshEnemyList() //This will reactive any destroyed enemies in a level.
	{
		Enemy[] enemyList = level.GetComponentsInChildren<Enemy> (true);

		foreach (Enemy enemy in enemyList)
		{
			enemy.gameObject.SetActive (true);
			enemy.Respawn ();
		}
	}
}

public class LevelObjectPool : UTIL_DynamicObjectPool<LevelPackage>
{
	[Header ("Main")]
	[SerializeField] LevelPackage[] levelList;

	GameObject poolRootObject;

	void Awake()
	{
		poolRootObject = new GameObject ("Level Pool");
	}

	//Will add all levels of a certain difficulty to the pool.
	public void PrepareNewLevels(int difficultySetting)
	{
		for (int i = 0; i < levelList.Length; i++)
		{
			if (levelList [i].difficulty == difficultySetting)
			{
				//Take the prefab from the levelList, instantiate it, then put it in the scene and the pool.
				LevelPackage newLevel = levelList [i];
				newLevel.level = Instantiate (newLevel.level); 
				AddToPool (newLevel);
			}
		}
	}

	//Will remove all levels of a certain difficulty to the pool.
	public void RemoveLevels(int difficultySetting)
	{
		for (int i = 0; i < levelList.Length; i++)
		{
			if (levelList[i].difficulty == difficultySetting)
			{
				GameObject removedLevel = RemoveIndexFromPool (i).level;
				Destroy (removedLevel);
			}
		}
	}

	public override void AddToPool (LevelPackage someLevel)
	{
		someLevel.level.transform.SetParent (poolRootObject.transform);
		someLevel.level.SetActive (false);
		base.AddToPool (someLevel);
	}

	public override LevelPackage PopOffPool ()
	{
		LevelPackage someLevel = GetNext ();

		someLevel.level.transform.SetParent (null);
		someLevel.level.SetActive (true);
		someLevel.WakeFromPooling ();

		return base.PopOffPool ();
	}

	public LevelPackage PopOffPoolRand ()
	{
		if (Count () > 0)
		{
			int randomLevelIndex = Random.Range (0, Count () - 1);

			LevelPackage someLevel = GetIndex (randomLevelIndex);

			someLevel.level.transform.SetParent (null);
			someLevel.level.SetActive (true);
			someLevel.WakeFromPooling ();

			return base.RemoveIndexFromPool (randomLevelIndex);
		}
		else
		{
			Debug.LogWarning ("DynamicObjectPool is empty! Cannot PullFromPool(), make sure the pool has enough objects.");
			return null;
		}
	}
}

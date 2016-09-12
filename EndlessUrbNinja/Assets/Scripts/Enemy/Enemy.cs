using UnityEngine;
using System.Collections;

enum EnemyState{Alive, Dead}

public class Enemy : MonoBehaviour {
	
    GameObject player;
	EnemyState myState = EnemyState.Alive;

	// Update is called once per frame
	void Update () {

		//Debug input to kill this enemy.
		if (Input.GetKey(KeyCode.K))
		{
			myState = EnemyState.Dead;
			gameObject.SetActive (false);
		}
	}

	void Die()
	{
		myState = EnemyState.Dead;
		gameObject.SetActive (false);
	}

	public bool isDead()
    {
		if (myState == EnemyState.Dead)
		{
			return true;
		}
		else
		{
			return false;
		}

    }

	public void Respawn()
	{
		myState = EnemyState.Alive;

		//gameObject.SetActive (true); 
		//If this game object was not active, this code wouldn't even run.
		//So it's been commented out, and is being externally SetActive.
	}
}

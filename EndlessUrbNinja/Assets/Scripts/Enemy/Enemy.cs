using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {

    [SerializeField] int distanceBuffer;

    private GameObject player;

    //True = Alive, False = Dead
    bool currentState = true;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        if(player)
        {
            //Since the hitbox on the enemies is large to account for the player misclicking due to clumbsy hands, the enemy will not die until the 
            //player is within the distance buffer.
            if (Vector3.Distance(gameObject.transform.position, player.transform.position) <= distanceBuffer && currentState == true)
            {
                //Debug.Log("Player killed enemy");

                currentState = false;

                gameObject.SetActive(false);
                
            }

        }	

		if (Input.GetKey(KeyCode.K))
		{
			currentState = false;
			gameObject.SetActive (false);
		}
	}

    void OnTriggerEnter(Collider colInfo)
    {
        if (colInfo.gameObject.CompareTag("Player"))
        {
            //Debug.Log("Player Within Enemy Radius");

            player = colInfo.gameObject;
        }
    }

    public bool isDead()
    {
        return currentState;
    }

	public void Respawn()
	{
		currentState = true;

		//gameObject.SetActive (true); 
		//If this game object was not active, this code wouldn't even run.
		//This game object needs to be reactivated externally.
	}
}

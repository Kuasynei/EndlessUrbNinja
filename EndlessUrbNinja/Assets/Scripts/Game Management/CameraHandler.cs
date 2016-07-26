using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class CameraHandler : MonoBehaviour {

	[SerializeField]AutoRunMovement playerAutoRun;
	[SerializeField]float cameraYOffset = 0;

	float targetScrollSpeed;

	void Start()
	{
		targetScrollSpeed = playerAutoRun.GetMaxRunSpeed ();
	}

	void Update()
	{
		Vector3 playerPosition = playerAutoRun.transform.position;

		transform.position = new Vector3 (transform.position.x, playerPosition.y/4 + cameraYOffset, transform.position.z);
		transform.position += new Vector3 (targetScrollSpeed * Time.deltaTime, 0, 0);

		Debug.Log (transform.position.x + "   |||   " + playerPosition.x);
		if (transform.position.x < playerPosition.x)
		{
			transform.position = Vector3.Lerp (new Vector3(transform.position.x, transform.position.y, transform.position.z), new Vector3(playerPosition.x, transform.position.y, transform.position.z), 0.1f);
		}
	}

	public void SetTargetPlayer(GameObject targetPlayer)
	{
		playerAutoRun = targetPlayer.GetComponent<AutoRunMovement>();
	}

}

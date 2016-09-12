using UnityEngine;
using System.Collections;

public enum CameraMode{running, slowStop, stopped}

[RequireComponent(typeof(Rigidbody))]
public class CameraHandler : MonoBehaviour {

	[SerializeField]PlayerController playerController;
	[SerializeField]float cameraYOffset = 0;
	[SerializeField]float cameraXOffset = 0;

	CameraMode myCamMode = CameraMode.stopped;

	float targetScrollSpeed;

	void Start()
	{
		targetScrollSpeed = playerController.GetMaxRunSpeed ();
		GlobalReferences.cameraHandler = this;
	}

	void Update()
	{
		Vector3 playerPosition = playerController.transform.position + new Vector3 (cameraXOffset, 0, 0);

		switch (myCamMode)
		{
		//Run normally. Slightly move up or down depending on the player's height, and if the player is ahead then lerp to them.
		case CameraMode.running:
			
			transform.position = new Vector3 (transform.position.x, playerPosition.y / 4 + cameraYOffset, transform.position.z);
			transform.position += new Vector3 (targetScrollSpeed * Time.deltaTime, 0, 0);
			if (transform.position.x < playerPosition.x)
			{
				transform.position = Vector3.Lerp (new Vector3 (transform.position.x, transform.position.y, transform.position.z), new Vector3 (playerPosition.x, transform.position.y, transform.position.z), 0.1f);
			}

			break;
		//Slow to a stop. Slightly move up or down depending on the player's height. The scrolling speed will slowly decrease to zero,
		//and the camera will not lerp to the player if they are ahead and died (or something).
		case CameraMode.slowStop:
			
			transform.position = new Vector3 (transform.position.x, playerPosition.y / 4 + cameraYOffset, transform.position.z);
			transform.position += new Vector3 (targetScrollSpeed * Time.deltaTime, 0, 0);

			break;
		//Camera is stopped, and not moving.
		case CameraMode.stopped:
			break;
		}
	}

	public void SetTargetPlayer(GameObject targetPlayer)
	{
		playerController = targetPlayer.GetComponent<PlayerController>();
	}

	public void SetMovementMode(CameraMode camMode)
	{
		myCamMode = camMode;
	}
}

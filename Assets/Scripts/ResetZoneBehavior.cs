using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetZoneBehavior : MonoBehaviour
{

	public Vector3 teleportPos;
	public bool resetVelocity;
	public GameObject teleportReceiver;

	// Start is called before the first frame update
	void Start()
	{
		
	}

	// Update is called once per frame
	void Update()
	{
		
	}

	void OnTriggerEnter(Collider collider)
	{
		if (collider.gameObject.GetComponent<PlayerBehavior>() != null)
		{
			References.theLevelLogic.RespawnPlayer();
		}

		/*GameObject otherObject = collider.gameObject;

		//if this is the player, then teleport them
		if (otherObject.GetComponent<PlayerBehavior>() != null)
		{
			//avoid funny trail rendering bug
			otherObject.GetComponent<PlayerBehavior>().speedTrailRenderer.emitting = false;

			if (teleportReceiver != null)
				otherObject.transform.position = teleportReceiver.transform.position;
			else
				otherObject.transform.position = teleportPos;


			if (resetVelocity)
				otherObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
		}*/
	}

}
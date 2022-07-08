using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterBehavior : MonoBehaviour
{

	readonly float minPlayerSpeed = 32;
	bool playerInContact = false;
	PlayerBehavior thePlayer;
	public Collider myCollider;

	// Start is called before the first frame update
	void Start()
	{
		thePlayer = References.thePlayer.GetComponent<PlayerBehavior>();
	}
	/*
	// Update is called once per frame
	void Update()
	{
		if (myCollider.enabled == true && thePlayer.GetVector3("MyLateralVelocity()").magnitude < minPlayerSpeed)
			myCollider.enabled = false;
		else if (myCollider.enabled == false && 
				 thePlayer.GetVector3("MyLateralVelocity()").magnitude >= minPlayerSpeed && 
				 !playerInContact)
			myCollider.enabled = true;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.GetComponent<PlayerBehavior>() != null)
			playerInContact = true;
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.GetComponent<PlayerBehavior>() != null)
			playerInContact = false;
	}*/

}

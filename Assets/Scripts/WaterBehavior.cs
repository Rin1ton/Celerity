/*using System.Collections;
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
		if (References.thePlayer != null)
			thePlayer = References.thePlayer.GetComponent<PlayerBehavior>();
	}

	private void Update()
	{
		if (References.thePlayer != null && thePlayer == null)
			thePlayer = References.thePlayer.GetComponent<PlayerBehavior>();
	}

}*/
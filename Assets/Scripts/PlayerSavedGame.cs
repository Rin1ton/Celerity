using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerSavedGame
{
	public Vector3 playerPosition;
	public Quaternion playerRotation;

	public PlayerSavedGame ()
	{
		playerPosition = References.thePlayer.transform.position;
		playerRotation = References.thePlayer.transform.rotation;
	}
}

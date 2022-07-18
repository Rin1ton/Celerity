using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerSavedGame
{
	public float playerPositionX;
	public float playerPositionY;
	public float playerPositionZ;
	public float playerRotationW;
	public float playerRotationX;
	public float playerRotationY;
	public float playerRotationZ;
	public string[] NRGCollected;

	public PlayerSavedGame()
	{
		//player coordinates
		playerPositionX = References.thePlayer.transform.position.x;
		playerPositionY = References.thePlayer.transform.position.y;
		playerPositionZ = References.thePlayer.transform.position.z;
		playerRotationW = References.thePlayer.transform.rotation.w;
		playerRotationX = References.thePlayer.transform.rotation.x;
		playerRotationY = References.thePlayer.transform.rotation.y;
		playerRotationZ = References.thePlayer.transform.rotation.z;
		NRGCollected = new string[References.theLevelLogic.NRGCollectedThisSession.Count];

		//NRG directly collected
		for(int NRG = 0; NRG < References.theLevelLogic.NRGCollectedThisSession.Count; NRG++)
			NRGCollected[NRG] = References.theLevelLogic.NRGCollectedThisSession[NRG];

		//NRG collected from time trials

		//NRG collected from speed traps
	}
}

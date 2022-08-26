using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerSavedGame
{
	public float playerPosX;
	public float playerPosY;
	public float playerPosZ;
	public float playerRotW;
	public float playerRotX;
	public float playerRotY;
	public float playerRotZ;
	public float playerVelX;
	public float playerVelY;
	public float playerVelZ;
	public bool playerDiveReady;
	public string[] NRGCollected;
	public string[] TimeTrialsCompleted;
	public string[] SpeedTrapsCompleted;

	public PlayerSavedGame()
	{
		//player coordinates
		playerPosX = References.thePlayer.transform.position.x;
		playerPosY = References.thePlayer.transform.position.y;
		playerPosZ = References.thePlayer.transform.position.z;
		playerRotW = References.thePlayer.transform.rotation.w;
		playerRotX = References.thePlayer.transform.rotation.x;
		playerRotY = References.thePlayer.transform.rotation.y;
		playerRotZ = References.thePlayer.transform.rotation.z;
		playerVelX = References.thePlayer.velocity.x;
		playerVelY = References.thePlayer.velocity.y;
		playerVelZ = References.thePlayer.velocity.z;
		playerDiveReady = References.thePlayer.diveReady;
		NRGCollected = new string[References.theLevelLogic.NRGCollectedThisSession.Count];
		TimeTrialsCompleted = new string[References.theLevelLogic.timeTrialsCompletedThisSession.Count];
		SpeedTrapsCompleted = new string[References.theLevelLogic.speedTrapsCompletedThisSession.Count];

		//NRG directly collected
		for(int NRG = 0; NRG < References.theLevelLogic.NRGCollectedThisSession.Count; NRG++)
			NRGCollected[NRG] = References.theLevelLogic.NRGCollectedThisSession[NRG];

		//NRG collected from time trials
		for (int timeTrial = 0; timeTrial < References.theLevelLogic.timeTrialsCompletedThisSession.Count; timeTrial++)
			TimeTrialsCompleted[timeTrial] = References.theLevelLogic.timeTrialsCompletedThisSession[timeTrial];

		//NRG collected from speed traps
		for (int speedTrap = 0; speedTrap < References.theLevelLogic.speedTrapsCompletedThisSession.Count; speedTrap++)
			SpeedTrapsCompleted[speedTrap] = References.theLevelLogic.speedTrapsCompletedThisSession[speedTrap];
	}

	public PlayerSavedGame(Vector3 playerPosition, Quaternion playerRotation, Vector3 playerVelocity)
	{
		playerPosX = playerPosition.x;
		playerPosY = playerPosition.y;
		playerPosZ = playerPosition.z;
		playerRotW = playerRotation.w;
		playerRotX = playerRotation.x;
		playerRotY = playerRotation.y;
		playerRotZ = playerRotation.z;
		playerVelX = playerVelocity.x;
		playerVelY = playerVelocity.y;
		playerVelZ = playerVelocity.z;
		playerDiveReady = false;
		NRGCollected = new string[0];
		TimeTrialsCompleted = new string[0];
		SpeedTrapsCompleted = new string[0];
	}
}


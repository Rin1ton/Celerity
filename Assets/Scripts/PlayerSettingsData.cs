using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerSettingsData
{
	public bool invertMouseY;
	public float horizontalSens;
	public float verticalSens;

	public string jumpButton;
	public string yeetButton;
	public string skateButton;
	public string forwardButton;
	public string backwardButton;
	public string leftButton;
	public string rightButton;
	public string grabButton;

	public PlayerSettingsData (PlayerBehavior myPlayer)
	{
		horizontalSens = myPlayer.xSens;
		verticalSens = myPlayer.ySens;
		invertMouseY = myPlayer.invertY;

		jumpButton = myPlayer.jumpButton.ToString();
		yeetButton = myPlayer.yeetButton.ToString();
		skateButton = myPlayer.skateButton.ToString();
		forwardButton = myPlayer.forwardButton.ToString();
		backwardButton = myPlayer.backwardButton.ToString();
		leftButton = myPlayer.leftButton.ToString();
		rightButton = myPlayer.rightButton.ToString();
		grabButton = myPlayer.grabButton.ToString();
	}

}

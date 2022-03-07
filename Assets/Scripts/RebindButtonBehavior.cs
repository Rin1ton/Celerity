using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class RebindButtonBehavior : MonoBehaviour
{
	[System.NonSerialized] public TextMeshProUGUI myTextObject;
	public string myActionName;
	PlayerBehavior myPlayer;

	private void Awake()
	{
		myTextObject = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
		//myPlayer = References.thePlayer.GetComponent<PlayerBehavior>();
	}

	private void OnEnable()
	{
		//set our buttons' text in the options menu based on the player's settings when it is opened
		if (References.thePlayer != null)
		switch (myActionName)
		{
			case References.yeet:
				myTextObject.text = References.thePlayer.GetComponent<PlayerBehavior>().yeetButton.ToString();
				break;
			case References.jump:
				myTextObject.text = References.thePlayer.GetComponent<PlayerBehavior>().jumpButton.ToString();
				break;
			case References.skate:
				myTextObject.text = References.thePlayer.GetComponent<PlayerBehavior>().skateButton.ToString();
				break;
			case References.forward:
				myTextObject.text = References.thePlayer.GetComponent<PlayerBehavior>().forwardButton.ToString();
				break;
			case References.backward:
				myTextObject.text = References.thePlayer.GetComponent<PlayerBehavior>().backwardButton.ToString();
				break;
			case References.left:
				myTextObject.text = References.thePlayer.GetComponent<PlayerBehavior>().leftButton.ToString();
				break;
			case References.right:
				myTextObject.text = References.thePlayer.GetComponent<PlayerBehavior>().rightButton.ToString();
				break;
			case References.grab:
				myTextObject.text = References.thePlayer.GetComponent<PlayerBehavior>().grabButton.ToString();
				break;
			default:
				Debug.Log("No case for loading an action called \"" + myActionName + "\" in ");
				break;
		}
	}

	void OnDisable()
	{

		switch (myActionName)
		{
			case References.yeet:
				References.thePauseMenu.yeetRebindButton = myTextObject;
				break;
			case References.jump:
				References.thePauseMenu.jumpRebindButton = myTextObject;
				break;
			case References.skate:
				References.thePauseMenu.skateRebindButton = myTextObject;
				break;
			case References.forward:
				References.thePauseMenu.forwardRebindButton = myTextObject;
				break;
			case References.backward:
				References.thePauseMenu.backwardRebindButton = myTextObject;
				break;
			case References.left:
				References.thePauseMenu.leftRebindButton = myTextObject;
				break;
			case References.right:
				References.thePauseMenu.rightRebindButton = myTextObject;
				break;
			case References.grab:
				References.thePauseMenu.grabRebindButton = myTextObject;
				break;
			default:
				Debug.Log("No case for loading an action called \"" + myActionName + "\" in ");
				break;
		}

	}

	public void RebindMe()
	{
			SavedSettings.ActionCastToRebind(this);
	}

}

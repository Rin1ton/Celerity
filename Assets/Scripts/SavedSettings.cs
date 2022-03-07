using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;


// this behavior acts as the buffer between the settings menu, and the saved configs of the playerBehavior script.
// it also acts as the buffer between the settings menu and our saved settings on file.
// the purpose of this script is to allow the rebinding of and setting of player controlled settings by passing the buttons and keys pressed to the 
// player script and handle them ourselves to write them to the settings file.
// this script will also be able to detect whether or not we're at the main menu, so no other script has to.
public class SavedSettings
{
	//options
	public static readonly float sliderMin = 0.001f;
	public static readonly float sliderMax = 9.999f;
	public static PlayerBehavior ourPlayerBehavior;
	public static KeyCode lastKeyPressed = KeyCode.None;
	public static RebindButtonBehavior currentRebindButton = null;
	public static string settingsPath;
	public static bool isRebindingFunction = false;

	//references given by themselves
	//public static PauseMenuBehavior myPauseMenu;
	public static TMP_InputField horizontalSensInputField;
	public static TMP_InputField verticalSensInputField;
	public static Slider horizontalSensSlider;
	public static Slider verticalSensSlider;
	static bool settingsLoaded = false;

	public static void Awake()
	{
		//get my settings path
		settingsPath = Application.persistentDataPath + "/PlayerSettings.kek";
	}

	// Start is called before the first frame update
	public static void Start()
    {
		//give the settings script the player script
		if (References.thePlayer != null)
			ourPlayerBehavior = References.thePlayer.GetComponent<PlayerBehavior>();

		//save our settings if the settings don't already exist
		if (!File.Exists(settingsPath))
			SavePlayerSettings();

		//load our settings
		ApplyPlayerSettingsFromFile();
	}

    // Update is called once per frame
    public static void Update()
    {
		//check that we haven't already updated our options menu
		if (!settingsLoaded)
		{
			//apply our settings to the options menu
			References.thePauseMenu.ApplySettingsToOptionsMenu(LoadPlayerSettings());
			settingsLoaded = true;
		}
		TryToRebindAction();
		lastKeyPressed = SearchForButtonPush();
    }

	public static void ApplyPlayerSettingsFromFile()
	{
		//check if we have any data on the player before we read from it
		if (LoadPlayerSettings() == null)
			return;

		//load the settings
		PlayerSettingsData dataOnFile = LoadPlayerSettings();

		//*try* to load our settings, but if our settings file is missing a value, it must be an old file, in which case, save a new file and load that.
		try
		{
			//set mouse options
			ourPlayerBehavior.xSens =			dataOnFile.horizontalSens;
			ourPlayerBehavior.ySens =			dataOnFile.verticalSens;
			ourPlayerBehavior.invertY =			dataOnFile.invertMouseY;

			//set all the binds
			ourPlayerBehavior.jumpButton =		(KeyCode)Enum.Parse(typeof(KeyCode), dataOnFile.jumpButton);
			ourPlayerBehavior.yeetButton =		(KeyCode)Enum.Parse(typeof(KeyCode), dataOnFile.yeetButton);
			ourPlayerBehavior.skateButton =		(KeyCode)Enum.Parse(typeof(KeyCode), dataOnFile.skateButton);
			ourPlayerBehavior.forwardButton =	(KeyCode)Enum.Parse(typeof(KeyCode), dataOnFile.forwardButton);
			ourPlayerBehavior.backwardButton =	(KeyCode)Enum.Parse(typeof(KeyCode), dataOnFile.backwardButton);
			ourPlayerBehavior.leftButton =		(KeyCode)Enum.Parse(typeof(KeyCode), dataOnFile.leftButton);
			ourPlayerBehavior.rightButton =		(KeyCode)Enum.Parse(typeof(KeyCode), dataOnFile.rightButton);

			//NEW SETTINGS. IF YOU NEED TO ADD A NEW SETTING, PUT IT AT THE END OF THESE TO PRESERVE PRECEEDING SETTINGS ON FILE.
			ourPlayerBehavior.grabButton =		(KeyCode)Enum.Parse(typeof(KeyCode), dataOnFile.grabButton);

		} catch (ArgumentNullException e)
		{
			//the way we do this happens to keep player settings
			Debug.Log("OLD SETTINGS FILE DETECTED!!! Saving new file. Exception: \"" + e + "\"");
			SavePlayerSettings();
			ApplyPlayerSettingsFromFile();
		}

	}

	public static void ActionCastToRebind(RebindButtonBehavior menuButton)
	{
		//start by saving the menu button that called for the rebind
		currentRebindButton = menuButton;

		//show the player they're rebinding a function, which function, and tell this script that as well
		References.thePauseMenu.ShowRebindWindow(menuButton.myActionName);
		isRebindingFunction = true;

		//zero out our last key so we don't take M0 as the button to rebind our function to
		lastKeyPressed = KeyCode.None;
	}

	//constantly check if we're trying to rebind any function every frame
	static void TryToRebindAction()
	{
		if (currentRebindButton != null &&              //we need a name of a function before we can rebind it
			lastKeyPressed != KeyCode.None)             //and we need a key to bind it to
		{

			//if we press escape, cancel the rebinding process
			if (lastKeyPressed == KeyCode.Escape)
			{
				References.thePauseMenu.HideRebindWindow();
				currentRebindButton = null;
				isRebindingFunction = false;
				return;
			}

			//save the binding in our save file
			RebindActionInSaveFile(currentRebindButton.myActionName, lastKeyPressed);

			//rebind the action in the current player behavior script, if the player exists exists
			if (References.thePlayer != null)
			{
				switch (currentRebindButton.myActionName)
				{
					case References.yeet:
						RebindActionInPlayerScript(ref ourPlayerBehavior.yeetButton, lastKeyPressed);
						break;
					case References.jump:
						RebindActionInPlayerScript(ref ourPlayerBehavior.jumpButton, lastKeyPressed);
						break;
					case References.skate:
						RebindActionInPlayerScript(ref ourPlayerBehavior.skateButton, lastKeyPressed);
						break;
					case References.forward:
						RebindActionInPlayerScript(ref ourPlayerBehavior.forwardButton, lastKeyPressed);
						break;
					case References.backward:
						RebindActionInPlayerScript(ref ourPlayerBehavior.backwardButton, lastKeyPressed);
						break;
					case References.left:
						RebindActionInPlayerScript(ref ourPlayerBehavior.leftButton, lastKeyPressed);
						break;
					case References.right:
						RebindActionInPlayerScript(ref ourPlayerBehavior.rightButton, lastKeyPressed);
						break;
					case References.grab:
						RebindActionInPlayerScript(ref ourPlayerBehavior.grabButton, lastKeyPressed);
						break;
					default:
						Debug.LogError("No case for rebinding \"" + currentRebindButton.myActionName + "\"");
						break;
				}
			}

			//exit the rebind window and set the new button text
			References.thePauseMenu.HideRebindWindow();
			currentRebindButton.myTextObject.text = lastKeyPressed.ToString();

		}
	}

	static void RebindActionInPlayerScript(ref KeyCode functionToRebind, KeyCode buttonToRebindItTo)
	{

		if (lastKeyPressed != KeyCode.None)
		{
			functionToRebind = buttonToRebindItTo;
		}
	}

	static KeyCode SearchForButtonPush()
	{
		if (isRebindingFunction)
		{
			foreach (KeyCode kCode in Enum.GetValues(typeof(KeyCode)))
			{
				if (Input.GetKeyDown(kCode))
				{
					isRebindingFunction = false;
					return kCode;
				}
			}
		}
		return KeyCode.None;
	}

	public static void UpdateSensBasedOnInputField()
	{
		//save our input fields' values
		float newHorizontalSens = float.Parse(horizontalSensInputField.text);
		float newVerticalSens = float.Parse(verticalSensInputField.text);

		//clamp the values
		newHorizontalSens = Mathf.Clamp(newHorizontalSens, sliderMin, sliderMax);
		newVerticalSens = Mathf.Clamp(newVerticalSens, sliderMin, sliderMax);

		//make the following zeroes
		horizontalSensInputField.text = References.thePauseMenu.SetInputFieldText(newHorizontalSens);
		verticalSensInputField.text = References.thePauseMenu.SetInputFieldText(newVerticalSens);

		//set the slider to our new value
		horizontalSensSlider.value = newHorizontalSens;
		verticalSensSlider.value = newVerticalSens;

		//update sens on file
		UpdateSensOnFile(newHorizontalSens, newVerticalSens);

		//set the sens for the players
		if (References.thePlayer != null)
		{
			ourPlayerBehavior.xSens = newHorizontalSens;
			ourPlayerBehavior.ySens = newVerticalSens;
		}
	}

	public static void UpdateSensBasedOnSlider()
	{
		//update the input field as the user moves the slider
		horizontalSensInputField.text = References.thePauseMenu.SetInputFieldText(horizontalSensSlider.value);
		verticalSensInputField.text = References.thePauseMenu.SetInputFieldText(verticalSensSlider.value);

		//update sens on file
		UpdateSensOnFile(horizontalSensSlider.value, verticalSensSlider.value);

		//update the sens for the player
		if (References.thePlayer != null)
		{
			ourPlayerBehavior.xSens = horizontalSensSlider.value;
			ourPlayerBehavior.ySens = verticalSensSlider.value;
		}
	}

	public static void ToggleMouseInvert(InvertMouseButtonBehavior myInvertButton)
	{
		if (!ourPlayerBehavior.invertY)
		{
			ourPlayerBehavior.invertY = true;
			myInvertButton.myTextObject.text = "Enabled";
		}
		else
		{
			ourPlayerBehavior.invertY = false;
			myInvertButton.myTextObject.text = "Disabled";
		}
		UpdateMouseInvertOnFile(ourPlayerBehavior.invertY);
	}

	#region Writing to Save File

	static void UpdateMouseInvertOnFile(bool invertMouse)
	{
		return;
	}

	static void UpdateSensOnFile(float newHorizontalSens, float newVerticalSens)
	{
		return;
	}

	static void RebindActionInSaveFile(string actionName, KeyCode buttonToRebindItTo)
	{
		return;
	}

	public static void SavePlayerSettings()
	{
		BinaryFormatter myFormatter = new BinaryFormatter();
		FileStream myStream = new FileStream(settingsPath, FileMode.Create);

		PlayerSettingsData myData = new PlayerSettingsData(ourPlayerBehavior);

		myFormatter.Serialize(myStream, myData);
		myStream.Close();
	}
	
	static PlayerSettingsData LoadPlayerSettings()
	{
		if (File.Exists(settingsPath))
		{
			BinaryFormatter myFormatter = new BinaryFormatter();
			FileStream myStream = new FileStream(settingsPath, FileMode.Open);

			PlayerSettingsData myData = myFormatter.Deserialize(myStream) as PlayerSettingsData;
			myStream.Close();

			return myData;
		}
		else
		{
			Debug.LogError("Trying to load settings file that does not exist in \"" + settingsPath + "\"!");
			return null;
		}
	}

	#endregion

}
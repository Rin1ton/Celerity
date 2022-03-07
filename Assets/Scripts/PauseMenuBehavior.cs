using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System;


//the only thing that this script does is decide what gets displayed on the pause menu based on input from the player and other scripts
//it takes commands from other scripts and formats what *they* want displayed in the menu
//this script controls formatting and takes arguments for what to format
//this script also acts as our main menu behavior
public class PauseMenuBehavior : MonoBehaviour
{

	readonly KeyCode pauseButton = KeyCode.Escape;
	readonly string inputFieldDecimalPlaces = "F3";
    public GameObject rootMenu;
	public GameObject optionsMenu;
    public GameObject controlsWindow;
	public GameObject rebindWindow;
	TextMeshProUGUI rebindWindowText;
	bool inOptions = false;
	bool isRebindingFunction = false;

	[NonSerialized] public TMP_InputField horizontalSensInputField;
	[NonSerialized] public TMP_InputField verticalSensInputField;
	[NonSerialized] public Slider horizontalSensSlider;
	[NonSerialized] public Slider verticalSensSlider;
	[NonSerialized] public InvertMouseButtonBehavior invertMouseYButton;

	[NonSerialized] public TextMeshProUGUI jumpRebindButton;
	[NonSerialized] public TextMeshProUGUI yeetRebindButton;
	[NonSerialized] public TextMeshProUGUI skateRebindButton;
	[NonSerialized] public TextMeshProUGUI forwardRebindButton;
	[NonSerialized] public TextMeshProUGUI backwardRebindButton;
	[NonSerialized] public TextMeshProUGUI leftRebindButton;
	[NonSerialized] public TextMeshProUGUI rightRebindButton;
	[NonSerialized] public TextMeshProUGUI grabRebindButton;

	/*	
		jumpButton = myPlayer.jumpButton.ToString();
		yeetButton = myPlayer.yeetButton.ToString();
		skateButton = myPlayer.skateButton.ToString();
		forwardButton = myPlayer.forwardButton.ToString();
		backwardButton = myPlayer.backwardButton.ToString();
		leftButton = myPlayer.leftButton.ToString();
		rightButton = myPlayer.rightButton.ToString();
	*/

	//all of the things we interact with that's attached to the player script
	/*
	[System.NonSerialized] public KeyCode yeetButton = KeyCode.Mouse0
	[System.NonSerialized] public KeyCode jumpButton = KeyCode.Space;
	[System.NonSerialized] public KeyCode skateButton = KeyCode.LeftShift;
	[System.NonSerialized] public KeyCode forwardButton = KeyCode.W;
	[System.NonSerialized] public KeyCode backwardButton = KeyCode.S;
	[System.NonSerialized] public KeyCode leftButton = KeyCode.A;
	[System.NonSerialized] public KeyCode rightButton = KeyCode.D;
	[System.NonSerialized] public bool rawMouseInput = false;
	[System.NonSerialized] public bool invertY = false;
	[System.NonSerialized] public float xSens = 1;
	[System.NonSerialized] public float ySens = 1;
	*/

	// Start is called before the first frame update

	public void Awake()
	{
		//tell references we are the pause menu
		References.thePauseMenu = this;

		//get the text component of our rebind action dialogue box
		rebindWindowText = rebindWindow.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();

		SavedSettings.Awake();
	}

	void Start()
    {
		rootMenu.SetActive(false);
		optionsMenu.SetActive(false);
		rebindWindow.SetActive(false);

		SavedSettings.Start();
	}

    // Update is called once per frame
    void Update()
    {
		if (References.thePlayer != null)
			GetPauseButton();
		else
			GetBackButton();

		SavedSettings.Update();
    }

	void GetPauseButton()
	{
		if (Input.GetKeyDown(pauseButton) && !References.isPaused)
			PauseGame();
		else if (Input.GetKeyDown(pauseButton) && inOptions && !isRebindingFunction)
			ExitOptions();
		else if (isRebindingFunction) 
			return;				//RebindAction() takes care of cancelling the rebind, so we don't do anything here with the pausebutton
		else if (Input.GetKeyDown(pauseButton) && References.isPaused)
			UnpauseGame();
	}

	void GetBackButton()
	{
		if (Input.GetKeyDown(pauseButton) && inOptions && !isRebindingFunction)
			ExitOptions();
	}

    public void PauseGame()
    { 
        References.isPaused = true;
		
        rootMenu.SetActive(true);
        controlsWindow.SetActive(false);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Time.timeScale = 0;
    }

    public void UnpauseGame()
    {
        References.isPaused = false;

        rootMenu.SetActive(false);
        controlsWindow.SetActive(true);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Time.timeScale = 1;
    }

	public void EnterOptions()
	{
		inOptions = true;

		optionsMenu.SetActive(true);
		rootMenu.SetActive(false);
	}

	public void ExitOptions()
	{
		inOptions = false;
		SavedSettings.SavePlayerSettings();

		optionsMenu.SetActive(false);
		rootMenu.SetActive(true);
	}

	public void ShowRebindWindow(string actionName)
	{
		rebindWindowText.text = "Press any button to rebind \"" + actionName + "\"\n(ESCAPE to cancel)";
		rebindWindow.SetActive(true);
		isRebindingFunction = true;
	}

	public void ApplySettingsToOptionsMenu(PlayerSettingsData dataOnFile)
	{
		//set mouse options
		horizontalSensInputField.text = dataOnFile.horizontalSens.ToString(inputFieldDecimalPlaces);
		horizontalSensSlider.value = dataOnFile.horizontalSens;
		verticalSensInputField.text = dataOnFile.verticalSens.ToString(inputFieldDecimalPlaces);
		verticalSensSlider.value = dataOnFile.verticalSens;
		invertMouseYButton.myTextObject.text = dataOnFile.invertMouseY ? "Enabled" : "Disabled";

		//set all the binds
		jumpRebindButton.text = dataOnFile.jumpButton;
		yeetRebindButton.text = dataOnFile.yeetButton;
		skateRebindButton.text = dataOnFile.skateButton;
		forwardRebindButton.text = dataOnFile.forwardButton;
		backwardRebindButton.text = dataOnFile.backwardButton;
		leftRebindButton.text = dataOnFile.leftButton;
		rightRebindButton.text = dataOnFile.rightButton;
		grabRebindButton.text = dataOnFile.grabButton;
	}

	public void ApplySettings()
	{
		SavedSettings.SavePlayerSettings();
	}

	public void HideRebindWindow()
	{
		rebindWindow.SetActive(false);
		isRebindingFunction = false;
	}

	public string SetInputFieldText(float value)
	{
		return value.ToString(inputFieldDecimalPlaces);
	}

	public void QuitToMainMenu()
	{
		SceneManager.LoadScene("Assets/Scenes/Main Menu.unity");
		UnpauseGame();
	}

	public void QuitGame()
    {
        Application.Quit();
    }

}

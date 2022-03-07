using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class SensInputFieldBehavior : MonoBehaviour
{

	public bool isHorizontal;

	void Awake()
	{
		if (isHorizontal)
			SavedSettings.horizontalSensInputField = gameObject.GetComponent<TMP_InputField>();
		else
			SavedSettings.verticalSensInputField = gameObject.GetComponent<TMP_InputField>();
	}

	private void OnEnable()
	{
		//set our buttons' text in the options menu based on the player's settings when it is opened
		if (References.thePlayer != null)
		{
			if (isHorizontal)
				gameObject.GetComponent<TMP_InputField>().text = References.thePauseMenu.SetInputFieldText(References.thePlayer.GetComponent<PlayerBehavior>().xSens);
			else
				gameObject.GetComponent<TMP_InputField>().text = References.thePauseMenu.SetInputFieldText(References.thePlayer.GetComponent<PlayerBehavior>().ySens);
		}
	}

	private void OnDisable()
	{
		if (isHorizontal)
			References.thePauseMenu.horizontalSensInputField = gameObject.GetComponent<TMP_InputField>();
		else
			References.thePauseMenu.verticalSensInputField = gameObject.GetComponent<TMP_InputField>();
	}

	public void InputFieldChanged()
	{
			SavedSettings.UpdateSensBasedOnInputField();
	}
}

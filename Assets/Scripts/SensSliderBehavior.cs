using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SensSliderBehavior : MonoBehaviour
{

	public bool isHorizontal;

	void Awake()
	{
		if (isHorizontal)
			SavedSettings.horizontalSensSlider = gameObject.GetComponent<Slider>();
		else
			SavedSettings.verticalSensSlider = gameObject.GetComponent<Slider>();
	}

	private void OnEnable()
	{
		//set our buttons' text in the options menu based on the player's settings when it is opened
		//Debug.LogError("this is overwriting the player's sensitivity setting. idk, figure it out\n" +
		//			   "after this, just the invert mouselook button is left.");
		if (References.thePlayer != null)
		{
			//Debug.Log("X Sens : " + References.thePlayer.GetComponent<PlayerBehavior>().xSens + " Y Sens : " + References.thePlayer.GetComponent<PlayerBehavior>().ySens);
			if (isHorizontal)
				gameObject.GetComponent<Slider>().value = References.thePlayer.GetComponent<PlayerBehavior>().xSens;
			else
				gameObject.GetComponent<Slider>().value = References.thePlayer.GetComponent<PlayerBehavior>().ySens;
		}
	}

	void OnDisable()
	{
		if (isHorizontal)
			References.thePauseMenu.horizontalSensSlider = gameObject.GetComponent<Slider>();
		else
			References.thePauseMenu.verticalSensSlider = gameObject.GetComponent<Slider>();
	}

	public void SliderMoved()
	{
		SavedSettings.UpdateSensBasedOnSlider();
	}

}

//																															<|*_*|>
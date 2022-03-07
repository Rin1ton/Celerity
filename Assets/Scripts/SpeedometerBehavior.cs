using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeedometerBehavior : MonoBehaviour
{
    string decimalPlacesToRound = "F1"; //"F1" means round to the first decimal place
	Text myText;

	void Awake()
	{
		myText = gameObject.GetComponent<Text>();
	}

	void Start()
	{
		
	}
    
    public void SetSpeedDisplay(float speed)
    {
		//change our displayed speed
		myText.text = speed.ToString(decimalPlacesToRound);
    }

    public void Disable()
    {
        gameObject.SetActive(false);
    }

    public void Enable()
    {
        gameObject.SetActive(true);
    }
}
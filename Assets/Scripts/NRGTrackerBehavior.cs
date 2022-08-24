﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NRGTrackerBehavior : MonoBehaviour
{

	TextMeshProUGUI myText;
	int currentCaps = 0;

	private void Awake()
	{
		References.playerNRGTracker = this;
	}

	// Start is called before the first frame update
	void Start()
    {
		myText = gameObject.GetComponent<TextMeshProUGUI>();
		myText.text = (References.startingEnergyCapsuleCount - References.currentEnergyCapsuleCount).ToString("D2") + "/" + References.startingEnergyCapsuleCount.ToString("D2");
		currentCaps = References.startingEnergyCapsuleCount;
	}

    // Update is called once per frame
    void Update()
    {
		if (currentCaps != References.currentEnergyCapsuleCount)
		{
			myText.text = (References.startingEnergyCapsuleCount - References.currentEnergyCapsuleCount).ToString("D2") + "/" + References.startingEnergyCapsuleCount.ToString("D2");
			currentCaps = References.currentEnergyCapsuleCount;
		}

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
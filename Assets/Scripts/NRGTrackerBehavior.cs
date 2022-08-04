using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NRGTrackerBehavior : MonoBehaviour
{

	TextMeshProUGUI myText;
	int startingCaps = 0;

	private void Awake()
	{
		References.playerNRGTracker = this;
	}

	// Start is called before the first frame update
	void Start()
    {
		myText = gameObject.GetComponent<TextMeshProUGUI>();
		myText.text = (References.startingEnergyCapsuleCount - References.currentEnergyCapsuleCount).ToString("D2") + "/" + References.startingEnergyCapsuleCount.ToString("D2");
	}

    // Update is called once per frame
    void Update()
    {
		if (startingCaps == 0)
		{
			myText.text = (References.startingEnergyCapsuleCount - References.currentEnergyCapsuleCount).ToString("D2") + "/" + References.startingEnergyCapsuleCount.ToString("D2");
			startingCaps = References.startingEnergyCapsuleCount;
		}
	}

	public void SetNRGTrackerDisplay(int numberLeft)
	{
		int numberCollected = References.startingEnergyCapsuleCount - numberLeft;
		myText.text = numberCollected.ToString("D2") + "/" + References.startingEnergyCapsuleCount.ToString("D2");
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
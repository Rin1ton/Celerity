using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InvertMouseButtonBehavior : MonoBehaviour
{

	[System.NonSerialized] public TextMeshProUGUI myTextObject;

	private void Awake()
	{
		myTextObject = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
	}

	private void OnDisable()
	{
		References.thePauseMenu.invertMouseYButton = this;
	}

	public void InvertMouse()
	{
		SavedSettings.ToggleMouseInvert(this);
	}
}
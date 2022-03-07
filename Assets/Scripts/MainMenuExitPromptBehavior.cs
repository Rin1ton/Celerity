using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MainMenuExitPromptBehavior : MonoBehaviour
{
	private void Awake()
	{
		References.mainMenuExitPrompt = gameObject.GetComponent<TextMeshProUGUI>();
	}
}

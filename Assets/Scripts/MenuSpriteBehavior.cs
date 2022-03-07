using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class MenuSpriteBehavior : MonoBehaviour
{
	Vector3 centerMenuPosition;
	public string myFunction;
	public GameObject RootMainMenuOptionsPrefab;
	public GameObject CancelExitPrefab;

	// Start is called before the first frame update
	void Start()
    {
		centerMenuPosition = new Vector3(0, 3.36f, 12);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public void MyFunction()
	{
		switch (myFunction)
		{
			case "Continue":
				Continue();
				break;
			case "Exit":
				Exit();
				break;
			case "Level Select":
				LevelSelect();
				break;
			case "Cancel Exit":
				CancelExit();
				break;
			default:
				Debug.LogError("No such menu selection exists idiot: " + myFunction);
				break;
		}

	}

	void Continue()
	{
		SceneManager.LoadScene("Assets/Scenes/Level One.unity");
		KillMe();
	}

	void Exit()
	{
		/*might be broken*/ Instantiate(CancelExitPrefab, centerMenuPosition, Quaternion.identity);
		References.mainMenuExitPrompt.text = "Hold \"" + References.thePlayer.GetComponent<PlayerBehavior>().backwardButton + "\" to Exit.";
		KillMe();
	}

	void LevelSelect()
	{
		Debug.Log("very epic");
		KillMe();
	}
	void CancelExit()
	{
		References.mainMenuExitPrompt.text = "";
		Instantiate(RootMainMenuOptionsPrefab);
		KillMe();
	}

	void KillMe()
	{
		Destroy(gameObject);
	}

}

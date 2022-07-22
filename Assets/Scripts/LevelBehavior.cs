using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;


public class LevelBehavior : MonoBehaviour
{

	public List<NRGCapsuleBehavior> thisLevelsNRG;
	public List<string> NRGCollectedThisSession;
	public List<string> timeTrialsCompletedThisSession;
	bool levelJustLoaded = true;

	//saving and loading
	public static string saveGamePath;
	readonly KeyCode deleteSavedGameKey1 = KeyCode.LeftShift;
	readonly KeyCode deleteSavedGameKey2 = KeyCode.Semicolon;
	static bool isTryingToDeleteGame;

	private void Awake()
    {
		//
		saveGamePath = Application.persistentDataPath + "/PlayerSaveGame.cringe";

        References.theLevelLogic = this;
		References.startingEnergyCapsuleCount = 0;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
		if (levelJustLoaded)
		{
			References.currentEnergyCapsuleCount = References.startingEnergyCapsuleCount;
			levelJustLoaded = false;
		}
		TryToDeleteGame();
	}

	public void NRGCollect(NRGCapsuleBehavior collectedNRG)
	{
		if (References.playerNRGTracker != null)
			References.playerNRGTracker.SetNRGTrackerDisplay(References.currentEnergyCapsuleCount);
		NRGCollectedThisSession.Add(collectedNRG.name);
	}

	public void NRGCollect()
	{
		if (References.playerNRGTracker != null)
			References.playerNRGTracker.SetNRGTrackerDisplay(References.currentEnergyCapsuleCount);
	}

	public void DestroyAllNRG()
	{
		for (int NRGs = thisLevelsNRG.Count - 1; NRGs >= 0; NRGs--)
		{
			Destroy(thisLevelsNRG[NRGs].gameObject);
		}
	}

	public void SaveGame()
	{
		BinaryFormatter myFormatter = new BinaryFormatter();
		FileStream myStream = new FileStream(saveGamePath, FileMode.Create);

		PlayerSavedGame myData = new PlayerSavedGame();

		myFormatter.Serialize(myStream, myData);
		myStream.Close();
	}

	public void CastSavedGameToBeDeleted()
	{
		References.thePauseMenu.ShowDialogueWindow("Press " + deleteSavedGameKey1 + " and " + deleteSavedGameKey2 + " to delete saved game and quit\n(ESCAPE to cancel)");
		isTryingToDeleteGame = true;
	}

	void TryToDeleteGame()
	{
		if (isTryingToDeleteGame)
		{

			//if we press escape, cancel the whole process
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				References.thePauseMenu.HideRebindWindow();
				isTryingToDeleteGame = false;
				return;
			}

			//if the appropriate keys are pressed, delete the file and quit
			if (Input.GetKey(deleteSavedGameKey1) &&
				Input.GetKey(deleteSavedGameKey2))
			{
				References.thePauseMenu.ShowDialogueWindow("Save file deleted. (ESCAPE to close this menu)");
				File.Delete(saveGamePath);
			}
		}
	}

}
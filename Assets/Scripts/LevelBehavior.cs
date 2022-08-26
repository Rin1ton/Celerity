using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;


public class LevelBehavior : MonoBehaviour
{
	//lists of 
	public List<string> NRGCollectedThisSession;
	public List<string> timeTrialsCompletedThisSession;
	public List<string> speedTrapsCompletedThisSession;
	bool levelJustLoaded = true;

	//saving and loading and deleting
	public static string saveGamePath;
	public GameObject playerPrefab;
	readonly KeyCode deleteSavedGameKey1 = KeyCode.LeftShift;
	readonly KeyCode deleteSavedGameKey2 = KeyCode.Semicolon;
	static Vector3 defaultPlayerPosition = new Vector3(-2.13818312f, 22, 23.389225f);
	static Quaternion defaultPlayerRotation = new Quaternion(0, 1, 0, 0);
	static bool isTryingToDeleteGame;

	private void Awake()
    {
		//
		saveGamePath = Application.persistentDataPath + "/PlayerSaveGame.cringe";

        References.theLevelLogic = this;
		References.startingEnergyCapsuleCount = 0;

		PlayerSavedGame myLoadedGame = LoadPlayerGame();

		foreach(string NRGname in myLoadedGame.NRGCollected)
			NRGCollectedThisSession.Add(NRGname);

		foreach(string timeTrialName in myLoadedGame.TimeTrialsCompleted)
			timeTrialsCompletedThisSession.Add(timeTrialName);

		foreach (string 
			speedTrapName 
			in 
			myLoadedGame.
			SpeedTrapsCompleted)
			speedTrapsCompletedThisSession.Add(speedTrapName);

		//use the information from the player's saved game to spawn them in the last place they saved the game.
		Vector3 playerSpawnPos = new Vector3(myLoadedGame.playerPosX, myLoadedGame.playerPosY, myLoadedGame.playerPosZ);
		Quaternion playerSpawnRot = new Quaternion(myLoadedGame.playerRotX, myLoadedGame.playerRotY, myLoadedGame.playerRotZ, myLoadedGame.playerRotW);
		Vector3 playerSpawnVel = new Vector3(myLoadedGame.playerVelX, myLoadedGame.playerVelY, myLoadedGame.playerVelZ);

		SpawnPlayer(playerSpawnPos, playerSpawnRot, playerSpawnVel).diveReady = myLoadedGame.playerDiveReady;
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
			References.currentEnergyCapsuleCount = References.startingEnergyCapsuleCount + References.currentEnergyCapsuleCount;
			levelJustLoaded = false;
		}
		TryToDeleteGame();
	}

	PlayerBehavior SpawnPlayer(Vector3 position, Quaternion rotation, Vector3 velocity)
	{
		GameObject thePlayer = Instantiate(playerPrefab, position, rotation);
		thePlayer.GetComponent<Rigidbody>().velocity = velocity;
		return thePlayer.GetComponent<PlayerBehavior>();
	}

	public void NRGCapCollect(NRGCapsuleBehavior collectedNRG)
	{
		NRGCollectedThisSession.Add(collectedNRG.name);
		NRGCollect();
	}

	public void NRGCollect()
	{
		References.currentEnergyCapsuleCount--;
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

	static PlayerSavedGame LoadPlayerGame()
	{
		PlayerSavedGame mySaveGame;
		if (File.Exists(saveGamePath))
		{
			BinaryFormatter myFormatter = new BinaryFormatter();
			FileStream myStream = new FileStream(saveGamePath, FileMode.Open);

			mySaveGame = myFormatter.Deserialize(myStream) as PlayerSavedGame;
			myStream.Close();
		}
		else
		{
			//if the save file does not exist, it should be generated
			mySaveGame = new PlayerSavedGame(defaultPlayerPosition, defaultPlayerRotation, Vector3.zero);
		}
			return mySaveGame;
	}

}
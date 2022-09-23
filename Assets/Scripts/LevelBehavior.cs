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

	//checkpoint stuff
	private List<CheckpointBehavior> Checkpoints = new List<CheckpointBehavior>();
	int currentCheckpoint = -1;

	private void Awake()
    {
		saveGamePath = Application.persistentDataPath + "/PlayerSaveGame.cringe";

        References.theLevelLogic = this;
		References.startingEnergyCapsuleCount = 0;


    }

    // Start is called before the first frame update
    void Start()
    {
		Checkpoints = References.Checkpoints;

		PlayerSavedGame myLoadedGame = LoadPlayerGame();

		for (int i = 0; i < Checkpoints.Count; i++)
		{
			if (Checkpoints[i].Name == myLoadedGame.currentCheckpoint)
				currentCheckpoint = i;
		}

		if (currentCheckpoint == -1)
			Debug.LogError("NO CHECKPOINT DECIDED!!! NO CHECKPOINT IN LOADED GAME!!!");

		Checkpoints[currentCheckpoint].Activate();

		foreach (string NRGname in myLoadedGame.NRGCollected)
			NRGCollectedThisSession.Add(NRGname);

		foreach(string timeTrialName in myLoadedGame.TimeTrialsCompleted)
			timeTrialsCompletedThisSession.Add(timeTrialName);

		foreach (string speedTrapName in myLoadedGame.SpeedTrapsCompleted)
			speedTrapsCompletedThisSession.Add(speedTrapName);

		//use the information from the player's saved game to spawn them in the last place they saved the game.
		Vector3 playerSpawnPos = new Vector3(myLoadedGame.playerPosX, myLoadedGame.playerPosY, myLoadedGame.playerPosZ);
		Quaternion playerSpawnRot = new Quaternion(myLoadedGame.playerRotX, myLoadedGame.playerRotY, myLoadedGame.playerRotZ, myLoadedGame.playerRotW);
		Vector3 playerSpawnVel = new Vector3(myLoadedGame.playerVelX, myLoadedGame.playerVelY, myLoadedGame.playerVelZ);
		SpawnPlayer(playerSpawnPos, playerSpawnRot, playerSpawnVel).diveReady = myLoadedGame.playerDiveReady;

    }

	void AfterStart()
	{
		References.currentEnergyCapsuleCount = References.startingEnergyCapsuleCount + References.currentEnergyCapsuleCount;

		//make sure we only have one default checkpoint, and make it active if there is no other checkpoint to be active
		int defaultCheckpoints = 0;
		int defaultIndex = 0;
		foreach(CheckpointBehavior checkpoint in Checkpoints)
		{
			if (checkpoint.isDefaultCheckpoint)
			{
				defaultCheckpoints++;
				defaultIndex = checkpoint.MyPlaceInList;
			}
		}
		if (defaultCheckpoints != 1)
			Debug.LogError("CANNOT DECIDE DEFAULT CHECKPOINT!!! NUMBER OF CANDIDATES: " + defaultCheckpoints);
		if (currentCheckpoint == -1)
		{
			currentCheckpoint = defaultIndex;
			Checkpoints[defaultIndex].Activate();
		}

		levelJustLoaded = false;
	}

    // Update is called once per frame
    void Update()
    {
		if (levelJustLoaded)
		{
			AfterStart();
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
			mySaveGame = new PlayerSavedGame(References.defaultCheckpoint.spawnPoint.position, defaultPlayerRotation, Vector3.zero);
		}
			return mySaveGame;
	}

	public void SetCurrentCheckpoint(int newCheckpoint)
	{
		if (currentCheckpoint != -1)
			Checkpoints[currentCheckpoint].Deactivate();
		currentCheckpoint = newCheckpoint;
		Checkpoints[currentCheckpoint].Activate();
	}

	public void RespawnPlayer()
	{
		References.thePlayer.gameObject.transform.position = Checkpoints[currentCheckpoint].spawnPoint.position;
		References.thePlayer.velocity = Vector3.zero;
		References.thePlayer.speedTrailRenderer.emitting = false;
	}

	public string CurrentCheckpointName => Checkpoints[currentCheckpoint].Name;

}
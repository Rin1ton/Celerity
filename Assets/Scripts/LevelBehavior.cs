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
	bool levelJustLoaded = true;

	//saving and loading
	public static string saveGamePath;

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

}
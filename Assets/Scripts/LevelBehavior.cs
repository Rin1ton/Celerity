using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class LevelBehavior : MonoBehaviour
{

	public List<NRGCapsuleBehavior> thisLevelsNRG;
	bool levelJustLoaded = true;

	private void Awake()
    {
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

}
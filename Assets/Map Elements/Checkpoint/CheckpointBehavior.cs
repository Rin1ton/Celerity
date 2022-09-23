using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointBehavior : MonoBehaviour
{
	//setable parameters
	public bool isDefaultCheckpoint;

    public string Name => gameObject.name;
    public int MyPlaceInList { get; private set; }
	public Renderer myColoredRing;
	public Transform spawnPoint;

	//
	Color activeColor = Color.green;
	Color inactiveColor = Color.cyan;

	private void Awake()
	{
		SetMyColor(inactiveColor);
		if (isDefaultCheckpoint)
			References.defaultCheckpoint = this;
	}

	// Start is called before the first frame update
	void Start()
    {
        MyPlaceInList = References.AddCheckpoint(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Activate()
	{
		SetMyColor(activeColor);
	}

    public void Deactivate()
	{
		SetMyColor(inactiveColor);
	}

    public void PlayerPassedThrough()
	{
        References.theLevelLogic.SetCurrentCheckpoint(MyPlaceInList);
	}

    void SetMyColor(Color color)
	{
		myColoredRing.material.SetColor("_Color", color);
	}

}

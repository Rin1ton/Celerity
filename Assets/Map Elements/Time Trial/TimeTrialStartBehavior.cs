using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeTrialStartBehavior : MonoBehaviour
{

	public GameObject myLeftPillarHead;
	public GameObject myRightPillarHead;
	public GameObject myCrossingLine;
	public TimeTrialEndBehavior myEnd;
	Renderer myLeftPillarHeadRenderer;
	Renderer myRightPillarHeadRenderer;
	Renderer myCrossingLineRenderer;

	// Start is called before the first frame update
	void Start()
	{
		//get the renderers for my parts
		myLeftPillarHeadRenderer = myLeftPillarHead.GetComponent<Renderer>();
		myRightPillarHeadRenderer = myRightPillarHead.GetComponent<Renderer>();
		myCrossingLineRenderer = myCrossingLine.GetComponent<Renderer>();
	}

	// Update is called once per frame
	void Update()
	{
		
	}

	void SetMyColor(Color color)
	{
		myLeftPillarHeadRenderer.material.SetColor("_Color", color);
		myRightPillarHeadRenderer.material.SetColor("_Color", color);
		myCrossingLineRenderer.material.SetColor("_Color", color);
		if (myEnd != null)
			myEnd.SetMyColor(color);
	}

	private void OnTriggerEnter(Collider other)
	{
		//if we're passed through by the player
		if (other.GetComponent<PlayerBehavior>() != null)
			SetMyColor(Color.green);
	}

}

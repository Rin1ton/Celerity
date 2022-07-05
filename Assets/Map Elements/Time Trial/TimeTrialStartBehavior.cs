using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeTrialStartBehavior : MonoBehaviour
{

	public GameObject myLeftPillarHead;
	public GameObject myRightPillarHead;
	public GameObject myCrossingLine;
	public TimeTrialEndBehavior myFinishLine;
	Renderer myLeftPillarHeadRenderer;
	Renderer myRightPillarHeadRenderer;
	Renderer myCrossingLineRenderer;

	//trail emitter stuff
	public GameObject trailEmitterPrefab;
	public Transform finishLineOrb;
	Vector3 finishPositionDelta;
	Vector3 startPosition;
	Vector3 emitterLaunchVector;
	float timeSinceSentTrailEmitter = 9999;
	GameObject currentTrailEmitter;
	readonly float emitterTimeToReachFinish = 2;
	readonly float playerDistanceToEmitTrail = 12;

	//awarding NRG stuff
	public GameObject finishLineCube;
	public GameObject blankNRGPrefab;
	bool raceIsRunning = false;

	// Start is called before the first frame update
	void Start()
	{
		if (myFinishLine != null)
			References.startingEnergyCapsuleCount++;
		else
			Debug.LogError("I don't have a finish line!!!");

		//get the renderers for my parts
		myLeftPillarHeadRenderer = myLeftPillarHead.GetComponent<Renderer>();
		myRightPillarHeadRenderer = myRightPillarHead.GetComponent<Renderer>();
		myCrossingLineRenderer = myCrossingLine.GetComponent<Renderer>();

		//set our start position
		startPosition = transform.position + new Vector3(0, 5, 0);

		//set our finishPositionDelta
		finishPositionDelta = finishLineOrb.position - startPosition;

		//do the math to see how we have to launch our trail emitter
		emitterLaunchVector = new Vector3();
		emitterLaunchVector.x = finishPositionDelta.x / emitterTimeToReachFinish;
		emitterLaunchVector.y = (finishPositionDelta.y / emitterTimeToReachFinish) - (0.5f * Physics.gravity.y * emitterTimeToReachFinish);
		emitterLaunchVector.z = finishPositionDelta.z / emitterTimeToReachFinish;

		//set my starting color
		SetMyColor(Color.cyan);
	}

	// Update is called once per frame
	void Update()
	{
		EmitTrailToFinish();
	}

	public void EmitTrailToFinish()
	{
		if (timeSinceSentTrailEmitter < emitterTimeToReachFinish)
			timeSinceSentTrailEmitter += Time.deltaTime;

		if (timeSinceSentTrailEmitter >= emitterTimeToReachFinish)
		{

			if (currentTrailEmitter != null)
				Destroy(currentTrailEmitter);

			if (Vector3.Distance(References.thePlayer.transform.position, startPosition) <= playerDistanceToEmitTrail || raceIsRunning)
			{
				currentTrailEmitter = Instantiate(trailEmitterPrefab, startPosition, transform.rotation);
				Rigidbody TERB = currentTrailEmitter.GetComponent<Rigidbody>();
				TERB.velocity = emitterLaunchVector;

				timeSinceSentTrailEmitter = 0;
			}
		
		}
	}

	void SetMyColor(Color color)
	{
		myLeftPillarHeadRenderer.material.SetColor("_Color", color);
		myRightPillarHeadRenderer.material.SetColor("_Color", color);
		myCrossingLineRenderer.material.SetColor("_Color", color);
		if (myFinishLine != null)
			myFinishLine.SetMyColor(color);
	}

	private void OnTriggerEnter(Collider other)
	{
		//if we're passed through by the player
		if (other.GetComponent<PlayerBehavior>() != null)
		{
			SetMyColor(Color.green);
			raceIsRunning = true;
		}
	}

	public void PlayerCrossedFinish()
	{
		if (raceIsRunning)
		{
			SetMyColor(Color.gray);
			Instantiate(blankNRGPrefab, finishLineOrb.position, Quaternion.identity);
			Destroy(finishLineOrb.gameObject);
			Destroy(finishLineCube.gameObject);

			raceIsRunning = false;

			if (currentTrailEmitter != null)
				Destroy(currentTrailEmitter);
			Destroy(myFinishLine);
			Destroy(this);
		}

	}

	/*
	 * we need to launch our trail so the player knows where to go
	 * what we always know: dX, dY, angle of launch, acceleration due to gravity
	 * what we need to solve for: initial velocity
	 * 
	 * NO wait, I have a better idea
	 * GIVEN: time, dX, dY, dZ, acceleration due to gravity,
	 * SOLVE FOR: xVi, yVi, zVi
	 * for Y: yVi = D/t - 1/2(at)
	 */

}

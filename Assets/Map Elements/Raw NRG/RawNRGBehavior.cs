using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.SocialPlatforms.Impl;

public class RawNRGBehavior : MonoBehaviour
{

	readonly float NRGSpinFactor = 600;
	readonly float timeToReachPlayer = 1.2f;
	public GameObject myLightning;
	public NRGSoundEmitterBehavior mySounds;
	public ParticleSystem myParticleSystem;
	public GameObject myPointLight;

	float timeSinceAlive = 0;
	Vector3 startingPosition;
	bool isCollected = false;

	// Start is called before the first frame update
	void Start()
	{
		startingPosition = transform.position;
	}

	// Update is called once per frame
	void Update()
	{
		Spin();
		UpdateTimer();
		Score();
		Move();
		Die();
	}

	void Spin()
	{
		if (myLightning != null) myLightning.transform.Rotate(Vector3.up * -NRGSpinFactor * Time.deltaTime, Space.World);
	}

	void UpdateTimer()
	{
		timeSinceAlive += Time.deltaTime;
	}

	void Move()
	{
		if (!isCollected) transform.position = Vector3.Lerp(startingPosition, References.thePlayer.transform.position, timeSinceAlive / timeToReachPlayer);
	}

	void Score()
	{
		if (timeSinceAlive >= timeToReachPlayer && !isCollected)
		{
			isCollected = true;

			mySounds.collectSound.Play();
			myParticleSystem.Stop();
			if (myPointLight != null) Destroy(myPointLight);
			if (myLightning != null) Destroy(myLightning);

			//if we are spawned by a time trial, DO NOT pass our name to the game saving script.
			//our time trial will keep track of our collected status.

			//we are already accounted for in the levelBehavior's list by whatever spawned us, so don't
			//
			References.theLevelLogic.NRGCollect();
		}
	}
	
	void Die()
	{
		if (timeSinceAlive > timeToReachPlayer + 2.5f) Destroy(gameObject);
	}

}

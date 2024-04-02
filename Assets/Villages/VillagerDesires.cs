using Newtonsoft.Json.Schema;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VillagerDesires : MonoBehaviour
{
	public enum desires
	{
		hunger = 0,
		exhaustion,
		boredom,
		poverty
	}

	public enum traits
	{
		openness = 0,
		conscientiousness,
		extraversion,
		agreeableness,
		neuroticism
	}

	float[] myDesires = new float[4];
	int[] myTraits = new int[5];
	int currentDesire = 0;

	VillagerMovement myMovement;

	private void Awake()
	{
		myMovement = GetComponent<VillagerMovement>();
	}

	private void Start()
	{

		for (int thisDesire = 0; thisDesire < myDesires.Length; thisDesire++)
		{
			myDesires[thisDesire] = Random.Range(0f, 30f);
		}
		for (int thisTrait = 0; thisTrait < myTraits.Length; thisTrait++)
		{
			myTraits[thisTrait] = Random.Range(0, 100);
		}

		myMovement.GoToStation(GetNextDesire());
	}

    private void Update()
    {
        for (int desire = 0; desire < myDesires.Length; desire++)
		{
			myDesires[desire] += Time.deltaTime;
		}
    }

    int GetNextDesire()
	{
		float maxDesireValue = 0;
		
		for (int desire = 0; desire < myDesires.Length; desire++)
		{
			if (myDesires[desire] > maxDesireValue)
			{
				maxDesireValue = myDesires[desire];
				currentDesire = desire;
			}
		}

		return currentDesire;
	}

	public void CurrentDesireFulfilled()
	{
		myDesires[currentDesire] = 0;
		myMovement.GoToStation(GetNextDesire());
	}

	public float TryToShove()
	{
		float thisTry = Random.Range(0f, 99f) + myDesires[(int)desires.hunger];
		return Mathf.Clamp(thisTry - myTraits[(int)traits.agreeableness], 0, Mathf.Infinity);
	}

}
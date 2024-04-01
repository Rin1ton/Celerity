using Newtonsoft.Json.Schema;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VillagerDesires : MonoBehaviour
{
	/*
	 * hunger
	 * exhaustion
	 * boredom
	 * poverty
	 */

	float[] desires = new float[4];
	int currentDesire = 0;

	VillagerMovement myMovement;

	private void Awake()
	{
		myMovement = GetComponent<VillagerMovement>();
	}

	private void Start()
	{

		for (int thisDesire = 0; thisDesire < desires.Length; thisDesire++)
		{
			desires[thisDesire] = Random.Range(0f, 30f);
		}

		myMovement.GoToStation(GetNextDesire());
	}

    private void Update()
    {
        for (int desire = 0; desire < desires.Length; desire++)
		{
			desires[desire] += Time.deltaTime;
		}
    }


    int GetNextDesire()
	{
		float maxDesireValue = 0;
		
		for (int desire = 0; desire < desires.Length; desire++)
		{
			if (desires[desire] > maxDesireValue)
			{
				maxDesireValue = desires[desire];
				currentDesire = desire;
			}
		}

		return currentDesire;
	}

	public void CurrentDesireFulfilled()
	{
		desires[currentDesire] = 0;
		myMovement.GoToStation(GetNextDesire());
	}

}
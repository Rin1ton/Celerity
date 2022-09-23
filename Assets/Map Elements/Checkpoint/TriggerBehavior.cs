using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerBehavior : MonoBehaviour
{

	public CheckpointBehavior myCheckpoint;

	// Start is called before the first frame update
	void Start()
	{
		
	}

	// Update is called once per frame
	void Update()
	{
		
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.GetComponent<PlayerBehavior>() != null)
			myCheckpoint.PlayerPassedThrough();
	}

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedTrapBehavior : MonoBehaviour
{

	public GameObject myCamera;
	readonly float trackingRange = 15;
	readonly float captureRange = 5;
	GameObject thePlayer;

	private void Awake()
	{
		if (myCamera == null)
			Debug.LogError("I DON'T HAVE MY CAMERA SET!!!");
	}

	// Start is called before the first frame update
	void Start()
	{
		thePlayer = References.thePlayer.gameObject;
	}

	// Update is called once per frame
	void Update()
	{
		LookAtObject(thePlayer);
	}

	void LookAtObject(GameObject trackedObject)
	{
		if (Vector3.Distance(trackedObject.transform.position, transform.position) <= trackingRange)
		{
			myCamera.transform.LookAt(trackedObject.transform);
		}
	}

}

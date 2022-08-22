using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedTrapBehavior : MonoBehaviour
{

	public GameObject myCamera;
	public float challengeSpeed;
	public GameObject NRGPrefab;
	readonly Vector3 NRGSpawnOffset = new Vector3(0, 4.5f, 0);
	readonly float trackingRange = 15;
	readonly float captureRange = 5;
	bool completed = false;
	Vector3 idleRotation = new Vector3(0, 15, 0);
	GameObject thePlayerCamera;
	Renderer myRenderer;
	ParticleSystemRenderer myPSR;

	//colors
	Color deadColor = Color.gray;
	Color activeColor = Color.cyan;


	private void Awake()
	{
		if (myCamera == null)
			Debug.LogError("I DON'T HAVE MY CAMERA SET!!!");

		myRenderer = myCamera.transform.GetChild(0).GetComponent<Renderer>();
		myPSR = GetComponent<ParticleSystemRenderer>();

		if (challengeSpeed == 0)
		{
			Debug.LogWarning("Speed not set, setting to default: " + 25);
		}
	}

	// Start is called before the first frame update
	void Start()
	{
		thePlayerCamera = References.thePlayer.myCamera;

		SetMyColor(activeColor);
	}

	// Update is called once per frame
	void Update()
	{
		LookAtObject(thePlayerCamera);
		ChallengePlayer();
	}

	void LookAtObject(GameObject trackedObject)
	{
		if (Vector3.Distance(trackedObject.transform.position, transform.position) <= trackingRange)
			myCamera.transform.LookAt(trackedObject.transform);
		else
			myCamera.transform.Rotate(idleRotation * Time.deltaTime, Space.World);
	}

	void ChallengePlayer()
	{
		if (!completed)
		{
			if (Vector3.Distance(thePlayerCamera.transform.position, transform.position) <= captureRange)
			{
				if (References.thePlayer.velocity.magnitude >= challengeSpeed)
				{
					Instantiate(NRGPrefab, transform.position + NRGSpawnOffset, transform.rotation);
					completed = true;
					SetMyColor(deadColor);
				}
			}
		}
	}

	void SetMyColor(Color color)
	{
		myRenderer.material.SetColor("_EmissionColor", color);
		myPSR.material.SetColor("_Color", color);
	}


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SpeedTrapBehavior : MonoBehaviour
{

	public GameObject myCamera;
	public float challengeSpeed;
	public GameObject NRGPrefab;
	public GameObject myBillBoard;
	TextMeshPro myText;
	readonly Vector3 NRGSpawnOffset = new Vector3(0, 4.5f, 0);
	readonly float trackingRange = 15;
	readonly float captureRange = 15;
	readonly string decPlaces = "F1";
	bool completed = false;
	Vector3 idleRotation = new Vector3(0, 15, 0);
	GameObject thePlayerCamera;
	Renderer myRenderer;
	ParticleSystemRenderer myPSR;

	//colors
	Color deadColor = Color.gray;
	Color activeColor = Color.cyan;

	bool levelJustLoaded = true;


	private void Awake()
	{
		if (myCamera == null)
			Debug.LogError("I DON'T HAVE MY CAMERA SET!!!");

		myText = myBillBoard.GetComponent<TextMeshPro>();

		myRenderer = myCamera.transform.GetChild(0).GetComponent<Renderer>();
		myPSR = GetComponent<ParticleSystemRenderer>();

		if (challengeSpeed == 0)
		{
			Debug.LogWarning("Speed not set, setting to default: " + 25);
			challengeSpeed = 25;
		}

		SetMyText(challengeSpeed);
		SetMyColor(activeColor);
	}

	// Start is called before the first frame update
	void Start()
	{
		References.startingEnergyCapsuleCount++;
	}

	// Update is called once per frame
	void Update()
	{
		if (levelJustLoaded)
		{
			levelJustLoaded = false;
			foreach (string speedTrapName in References.theLevelLogic.speedTrapsCompletedThisSession)
			{
				if (speedTrapName == name)
					FinishWithoutEffect("IMPLIMENT SAVING SPEED");
			}
			thePlayerCamera = References.thePlayer.myCamera;
		}
		if (!completed)
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
					SetMyText(References.thePlayer.velocity.magnitude);
					References.theLevelLogic.speedTrapsCompletedThisSession.Add(name);
				}
			}
		}
	}

	void SetMyColor(Color color)
	{
		myRenderer.material.SetColor("_EmissionColor", color);
		myPSR.material.SetColor("_Color", color);
		myText.color = color;
	}

	void SetMyText(string input)
	{
		myText.text = input;
	}

	void SetMyText(float input)
	{
		myText.text = input.ToString(decPlaces);
	}

	void FinishWithoutEffect(string inputString)
	{
		SetMyColor(deadColor);
		completed = true;
		SetMyText(inputString);
		References.theLevelLogic.NRGCollect();

		/*SetMyColor(deadColor);

		Destroy(finishLineOrb.gameObject);
		Destroy(finishLineCube.gameObject);

		References.theLevelLogic.NRGCollect();

		Destroy(myFinishLine);
		Destroy(this);*/
	}

}

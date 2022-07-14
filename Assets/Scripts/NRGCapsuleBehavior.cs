using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NRGCapsuleBehavior : MonoBehaviour
{
	readonly float spinFactor = 200;
	readonly float NRGSpinFactor = 600;
	readonly float xSpin = -0.25f;
	readonly float ySpin = 1.35f;
	readonly float zSpin = 0;
	readonly float speedOfPlayerMultiplier = 1.3f;
	readonly float distanceToPlayerToDie = 1;
	readonly float brokenCapExplosiveForce = 5;
	readonly float brokenCapFlyAwayForce = 8;
	GameObject player;
	NRGSoundEmitterBehavior mySounds;
	public GameObject myCapsule;
	public GameObject myNRG;
	public GameObject brokenCapsulePrefab;
	public ParticleSystem myParticleSystem;
	public GameObject myPointLight;
	public GameObject myOverheadLabel;
	public bool isAccountedFor;
	float timeToDieAfterCollected = 2.5f;
	bool collected = false;
	float chaseThePlayerSpeed;
	float chaseThePlayerMinSpeed = 15;
	bool hasBeenCollected = false;

    private void Awake()
    {
		
    }

    // Start is called before the first frame update
    void Start()
    {		
		if (myOverheadLabel == null && !isAccountedFor)
			References.startingEnergyCapsuleCount++;

        //get player
		player = References.thePlayer;

		//get my sound system
		mySounds = transform.Find("NRGSoundEmitter").GetComponent<NRGSoundEmitterBehavior>();

		//get my particle system
		myParticleSystem = myParticleSystem.GetComponent<ParticleSystem>();

		//add me to the list of NRG
		References.theLevelLogic.thisLevelsNRG.Add(this);

		Debug.Log(gameObject.name);
    }

    // Update is called once per frame
    void Update()
    {
		if (collected)
		{
			//countdown to die
			timeToDieAfterCollected -= Time.deltaTime;

			//set the speed to chase the player
			chaseThePlayerSpeed = player.GetComponent<Rigidbody>().velocity.magnitude * speedOfPlayerMultiplier < chaseThePlayerMinSpeed ?
				chaseThePlayerMinSpeed : 
				player.GetComponent<Rigidbody>().velocity.magnitude * speedOfPlayerMultiplier;

			//chase player
			Vector3 movementToPlayer = (player.transform.position - transform.position).normalized * chaseThePlayerSpeed;
			gameObject.transform.position += movementToPlayer * Time.deltaTime;


			//disappear completely if we made it to the player
			if ((player.transform.position - transform.position).magnitude < distanceToPlayerToDie)
			{
				if (myNRG != null)
				{
					mySounds.collectSound.Play();
					myParticleSystem.Stop();
					if (myPointLight != null)
						Destroy(myPointLight);
					References.theLevelLogic.NRGCollect();
					hasBeenCollected = true;

					//if I'm a main menu selection, Run the function of that menu option
					if (myOverheadLabel != null)
						myOverheadLabel.GetComponent<MenuSpriteBehavior>().MyFunction();
					
				}
				
				Destroy(myNRG);
			}

			//die if alive too long
			if (timeToDieAfterCollected <= 0)
			{
				if (!hasBeenCollected)
				{
					References.theLevelLogic.NRGCollect();
					hasBeenCollected = true;
				}
				Destroy(gameObject);
			}
		}
		else //if not collected, spiiiiiiiiiiiiiiiiin
		{
			if (myCapsule != null)
				myCapsule.transform.Rotate(xSpin * Time.deltaTime * spinFactor, 
										   ySpin * Time.deltaTime * spinFactor, 
										   zSpin * Time.deltaTime * spinFactor, 
										   Space.World);
		}

		//Lightning Bolt in the middle always spins
		if (myNRG != null)
			myNRG.transform.Rotate(Vector3.up * -NRGSpinFactor * Time.deltaTime, Space.World);
    }

    public void Collect()
    {
		//pull me from every list I'm apart of
		gameObject.GetComponent<RemoveMeFromListBehavior>().RemoveMeFromAllYeetLists();
		References.currentEnergyCapsuleCount--;
		References.theLevelLogic.thisLevelsNRG.Remove(this);
		
		//if we're a menu option, destroy every other one
		if (myOverheadLabel != null)
			References.theLevelLogic.DestroyAllNRG();

		//do the Collecting effects
		gameObject.GetComponent<Collider>().enabled = false;
		mySounds.breakSound.Play();
		if (brokenCapsulePrefab != null)
		{
			GameObject myBrokenCapsule = Instantiate(brokenCapsulePrefab, transform.position, myCapsule.transform.rotation);
			Destroy(myCapsule);
			List<GameObject> brokenCapPieces = myBrokenCapsule.GetComponent<BrokenCapBehavior>().myPieces;
			for (int k = 0; k < brokenCapPieces.Count; k++)
			{
				brokenCapPieces[k].GetComponent<Rigidbody>().AddExplosionForce(brokenCapExplosiveForce, transform.position, distanceToPlayerToDie, 0, ForceMode.Impulse);
				brokenCapPieces[k].GetComponent<Rigidbody>().AddForce((transform.position - player.transform.position).normalized * brokenCapFlyAwayForce, ForceMode.VelocityChange);
			}
		}
		else
			Debug.LogError("NO BROKEN CAPSULE PREFAB");
		collected = true;
    }

	private void OnDestroy()
	{
		//pull me from every list I'm apart of
		gameObject.GetComponent<RemoveMeFromListBehavior>().RemoveMeFromAllYeetLists();
		References.theLevelLogic.thisLevelsNRG.Remove(this);
	}

}

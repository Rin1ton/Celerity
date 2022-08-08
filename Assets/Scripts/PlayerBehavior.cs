using UnityEngine;

public class PlayerBehavior : MonoBehaviour
{
	//script-wide
	readonly float shortTimerStop = 120;                    //any short timers should stop before they can overflow
	float defaultMass;										//player default mass, initialized in start with what is saved in unity inspector

	//mouselook
	readonly float maxVerticalRotaion = 90;
	public GameObject myCamera;
	Quaternion myRBOriginalRotation;
	Quaternion myCameraOriginalRotation;
	float rotationX = 0;
	float rotationY = 0;

	//player alterable controls
	//I don't want ANY OF THIS SHIT being FUCKED UP by the inspector

	/*
	if you want to add a control, is has to be added to the following places:
	- PlayerBehavior
	- References
	- PlayerSettingsData
	- PauseMenuBehavior (Declare rebind UI button)
	- PauseMenuBehavior.ApplySettingsToOptionsMenu()
	- RebindButtonBehavior
	- SavedSettings.ApplyPlayerSettingsFromFile()
	- SavedSettings.TryToRebindAction()
	- Options Menu (add a rebind button to the UI)
	*/
	[System.NonSerialized] public KeyCode yeetButton = KeyCode.Mouse0;
	[System.NonSerialized] public KeyCode jumpButton = KeyCode.Space;
	[System.NonSerialized] public KeyCode skateButton = KeyCode.LeftShift;
	[System.NonSerialized] public KeyCode forwardButton = KeyCode.W;
	[System.NonSerialized] public KeyCode backwardButton = KeyCode.S;
	[System.NonSerialized] public KeyCode leftButton = KeyCode.A;
	[System.NonSerialized] public KeyCode rightButton = KeyCode.D;
	[System.NonSerialized] public KeyCode grabButton = KeyCode.Mouse1;
	[System.NonSerialized] public bool rawMouseInput = false;
	[System.NonSerialized] public bool invertY = false;
	[System.NonSerialized] public float xSens = 1;
	[System.NonSerialized] public float ySens = 1;

	//Moving
	readonly float maxGroundAngle = 45;						//if walking on a surface more than this steep, we're not grounded
	readonly float groundStickForce = 65;
	readonly float distanceFromCenterToEdgeOfPlayer = 0.95f;
	public bool isGrounded = false;
	public struct MoveStats
	{
		public float friction;
		public float topSpeed;
		public float airAcceleration;
		public float groundAcceleration;
	}
	Rigidbody myRB;
	Vector3 moveInput = new Vector3();
	Vector3 myImaginaryVel;
	MoveStats currentMove;                                  //move stats we have at any given time, only thing read directly by the keyboard movement functions
	MoveStats runningMove;                                  //default move stats. used when not skating or boosting
	MoveStats skatingMove;                                  //skating move stats. used when skating, but not boosting
	MoveStats wallRunMove;                                  //wall running move stats, used when wall running
	MoveStats grindingMove;
	Vector3 currentGround = Vector3.zero;					//normal of platform we're walking on. equals zero if we're not grounded
	float currentGroundAngle;								//0 means flat ground. 90 means sheer wall
	float timeSinceGrounded = 0;
	float savedSpeed = 0;

	//running
	readonly float runningFriction = 7.5f;                  //friction with the ground while running
	readonly float runningTopSpeed = 10;                    //our "max speed" (lel, like that matters)
	readonly float runningAirAcceleration = 1.22f;			//how fast we accelerate to our "top speed" in the air running
	readonly float runningGroundAcceleration = 12;          //how fast we accelerate to our "top speed" on land running

	//skating
	readonly float skatingFriction = 0;                     //friction with the ground while skating
	readonly float skatingTopSpeed = 12;                    //our "max speed" but for the skateboard
	readonly float skatingAirAcceleration = 1.22f;          //how fast we accelerate to our "top speed" in the air skating
	readonly float skatingGroundAcceleration = 1.22f;		//how fast we accelerate to our "top speed" on land skating (HAS TO BE SAME AS RUNNING AIR ACCELERATION OR MORE)
	readonly float slopeSkatingAssistForce = 0.85f;         //the fraction of gravity that is nullified when going up the slope
	readonly float slopeSkatingAssistStart = 12;            //the player must be going this fast to get the slope skating assists
	bool isSkating = false;

	//wall running
	readonly float wallRunFriction = 0.5f;
	readonly float wallRunTopSpeed = 58;
	readonly float wallRunAirAcceleration = Mathf.Infinity;
	readonly float wallRunGroundAcceleration = 0.6f;

	readonly float wallRunMinAngle = 74;
	readonly float wallRunMaxAngle = 106;
	readonly float wallAngleDifferenceThreshold = 5;        //if 2 planes are within this many degrees of each other, treat them as the same wall
	Vector3 currentWall;                                    //current wall we're running on, defined by the normal of a plane
	Vector3 lastWall;                                       //last wall we ran on
	float wallAngle = 0;                                    //angle of the current wall in relation to v3.up
	bool isWallRunning = false;

	readonly float wallRunDuration = 2;                     //duration of wall run, before the player falls off
	readonly float wallRunGroundedBuffer = 0.1f;            //can't wallrun if we've been on the ground recently
	readonly float wallPullOffAngle = 45;
	readonly float timeToPullOffWall = 0.1f;
	float timeSinceStartedPullingOff = 0;
	float timeSinceWallRunStart = 0;
	float timeSinceWallRunExit = 120;
	
	readonly float wallJumpAwayForce = 7;                   //force applied away from wall when player jumps off it
	readonly float wallJumpForwardForce = 5;                //force applied in the moveInput direction when they jump off a wall
	readonly float wallJumpMaxForce = 8;
	readonly float wallKickDuration = 0.1f;                 //friction isn't applied to the wall run until they've wall-ran for this long

	//jump
	readonly float jumpSpeed = 15;							//vertical velocity applied to the player when they jump
	readonly float hardJumpCooldown = 0.045f;               //it is impossible to jump more than once during this interval in seconds
	readonly float coyoteTime = 0.15f;						//if we're off a platform for this amount of time OR LESS we can still jump
	bool jumpReady = true;
	float timeSinceLastJump = 0;

	//super jump
	readonly float superJumpSpeedMaximum = 2.5f;            //the fastest the palyer can be moving and perform a super jump skating
	readonly float superJumpSpeedMinimum = 17;				//the slowest the palyer can be moving and perform a super jump running
	readonly float superJumpSpeed = 22.5f;

	//yeet
	readonly float yeetForce = 18;                          //the force with which objects are "yeeted"
	readonly float yeetCoolDown = 0.8f;
	readonly float requiredSpinToResetYeet = 270;
	bool yeetReady = true;
	float spinSinceLastYeet = 0;
	float timeSinceLastYeet = 100;
	public YeetTriggerBehavior myYeetTrigger;

	//dive
	readonly float maxDiveSpeed = 20;						//the most a dive will give by itself
	readonly float minDiveSpeed = 14;
	readonly float dischargedDivePenalty = 0.5f;            //the most the player can be slowed down by diving too early
	readonly float diveRegenRate = 4;						//the amount the dive recharges per second
	readonly float minAngleTowardUpDive = 60;				//the closest the dive vector can be to the up vector
	readonly float diveSpeedPenalty = 0.95f;
	float minMaxDiveSpeedDifference;                        //difference between maxDiveSpeed and minDiveSpeed, initialized in Start()
	public bool diveReady = false;
	float currentDiveSpeed;

	//thrust
	readonly float thrustForce = 1.75f;
	readonly float thrustMass = 50;
	readonly float thrustMaxSpeed = 32;
	readonly float thrustTurnAssist = 50;					//make it so the player can turn with thrust
	readonly float thrustSoundMinVolume = 0.15f;
	readonly float thrustMaxForce = 80;                     //affects the thrust sound(thrustMinForce is always 0)
	readonly float thrustingSlopeSkatingAssistForce = 1;	//how much gravity is negated on slopes when thrusting up them
	float thrustSoundDefaultVolume;							//volume set in the inspector, initialized in Start()
	bool isThrusting = false;

	//air kick
	readonly float airKickSpeed = 5;
	float spinSinceGrounded = 0;
	bool airKickReady = true;

	//double jump
	readonly float timeToDoubleJumpAfterWallRun = 1.6f;
	readonly float doubleJumpMinLateralSpeed = 12;
	readonly float doubleJumpHeight = 10;
	bool doubleJumpReady = false;

	//grab
	GameObject currentGrabbedObject;

	//sounds
	readonly float skatingGroundVolume = 0.35f;
	readonly float skatingAirVolume = 0.1f;
	readonly float skatingPitchCurveFlatten = 700;			//used to define the pitch curve verses the speed of the player. the higher this value, the less the pitch shifts due to speed.
	readonly float speedPitchCap = 34;                      //after this speed, the pitch of the board will stop going up
	readonly float minimumSkateboardSoundSpeed = 4;
	PlayerSoundEmitterBehavior mySounds;

	//animations
	public CharacterAnimationBehavior myAnimaions;

	//hud
	public GameObject speedometerPrefab;
	SpeedometerBehavior mySpeedometer;

	//effects
	public ParticleSystem speedLinesParticleSystem;
	public ParticleSystem yeetParticleSystem;
	public ParticleSystem diveParticleSystem;
	public ParticleSystem thrustParticleSystem;
	public ParticleSystem superJumpParticleSystem;
	public ParticleSystem superJumpTrailParticleSystem;
	public TrailRenderer speedTrailRenderer;
	readonly float speedLineParticleMultiplier = 35;
	readonly float speedLineParticleMax = 200;

	//water running
	readonly float waterRunTimeLimit = 2.5f;
	readonly float minSpeedToWaterRun = 20;
	float timeLeftToWaterRun;
	bool isOnWater = false;

	//animation
	public GameObject myLeftArm;
	public GameObject myRightArm;
	public GameObject myBoard;
	public Transform myArmRotationPoint;

	//grinding
	public GrindyRailBehavior currentRail;
	float railT = -1;
	readonly float grindingFriction = 0;
	readonly float grindingTopSpeed = 28;
	readonly float grindingAirAcceleration = Mathf.Infinity;
	readonly float grindingGroundAcceleration = 5;
	readonly float minDistanceToStayOnRail = 1.5f;
	bool isGrinding = false;
	Vector3 playerGrindingVerticalOffset = new Vector3(0, 1, 0);
	Vector3 pointOnRail;
	Vector3 tangentVector;


	/*====================================================================
	 * enough with the variable declaration
	 * let's get coding
	 * i'm a bad mam-ah jam-ah
	 * here it is:
	 * [my code]
	 =====================================================================*/






	//awake runs before start
	void Awake()
	{
		References.thePlayer = this;
	}

	// Start is called before the first frame update
	void Start()
	{
		
		//get my rigidbody
		myRB = GetComponent<Rigidbody>();

		//draw my speedometer if I have one
		if (speedometerPrefab != null)
		{
			GameObject speedometerObject = Instantiate(speedometerPrefab, References.canvas.transform);
			mySpeedometer = speedometerObject.GetComponent<SpeedometerBehavior>();
		}

		//lock the mouse and make it disappear
		UnityEngine.Cursor.lockState = CursorLockMode.Locked;
		UnityEngine.Cursor.visible = false;

		//camera broke; heinous fix inbound
		//this makes it so we can place the player in any rotation
		Quaternion cameraCorrection = Quaternion.AngleAxis(-myCamera.transform.rotation.eulerAngles.y, Vector3.up);
		Quaternion cameraIncorrectPosition = myCamera.transform.rotation;
		myCamera.transform.rotation = cameraIncorrectPosition * cameraCorrection;

		//get my original rotation
		myRBOriginalRotation = myRB.transform.rotation;
		myCameraOriginalRotation = myCamera.transform.rotation;

		//get our sound emitter
		mySounds = transform.Find("PlayerSoundEmitter").GetComponent<PlayerSoundEmitterBehavior>();

		//set our movement stats
		{
			skatingMove.friction = skatingFriction;
			skatingMove.topSpeed = skatingTopSpeed;
			skatingMove.airAcceleration = skatingAirAcceleration;
			skatingMove.groundAcceleration = skatingGroundAcceleration;
		}
		{
			runningMove.friction = runningFriction;
			runningMove.topSpeed = runningTopSpeed;
			runningMove.airAcceleration = runningAirAcceleration;
			runningMove.groundAcceleration = runningGroundAcceleration;
		}
		{
			wallRunMove.friction = wallRunFriction;
			wallRunMove.topSpeed = wallRunTopSpeed;
			wallRunMove.airAcceleration = wallRunAirAcceleration;
			wallRunMove.groundAcceleration = wallRunGroundAcceleration;
		}
		{
			grindingMove.friction = grindingFriction;
			grindingMove.topSpeed = grindingTopSpeed;
			grindingMove.airAcceleration = grindingAirAcceleration;
			grindingMove.groundAcceleration = grindingGroundAcceleration;
		}
		currentMove = runningMove;

		//initialize speed lines particle system
		if (speedLinesParticleSystem != null)
			speedLinesParticleSystem = speedLinesParticleSystem.GetComponent<ParticleSystem>();
		else
			Debug.LogError("NO speedLinesParticleSystem!");

		//initialize yeet particle system
		if (yeetParticleSystem != null)
			yeetParticleSystem = yeetParticleSystem.GetComponent<ParticleSystem>();
		else
			Debug.LogError("NO yeetParticleSystem!");

		//initialize dive particle system
		if (diveParticleSystem != null)
			diveParticleSystem = diveParticleSystem.GetComponent<ParticleSystem>();
		else
			Debug.LogError("NO diveParticleSystem!");

		//initialize thrust particle system
		if (thrustParticleSystem != null)
			thrustParticleSystem = thrustParticleSystem.GetComponent<ParticleSystem>();
		else
			Debug.LogError("NO thrustParticleSystem!");

		//set thrust sound volume
		thrustSoundDefaultVolume = mySounds.thrustSound.volume;

		//initialize super jump particle system
		if (superJumpParticleSystem != null)
			superJumpParticleSystem = superJumpParticleSystem.GetComponent<ParticleSystem>();
		else
			Debug.LogError("NO superJumpParticleSystem!");

		//initialize super jump trail particle system
		if (superJumpTrailParticleSystem != null)
			superJumpTrailParticleSystem = superJumpTrailParticleSystem.GetComponent<ParticleSystem>();
		else
			Debug.LogError("NO superJumpTrailParticleSystem!");

		//set our dive speed
		currentDiveSpeed = maxDiveSpeed;
		minMaxDiveSpeedDifference = maxDiveSpeed - minDiveSpeed;

		//get our mass
	 	defaultMass = myRB.mass;

		
	}






	//update is called once per frame
	void Update()
	{
		//can't play game if game is paused
		if (!References.isPaused)
		{
			MouseLook();
			MoveStatSet();
			KeyboardMovement();
			Timers();
			SkatingOrRunning();
			SuperJump();					//must be called *before* standard jump. Superjump must expend "jumpReady" so the standard jump cannot.
			Jump();
			SaveSpeed();
			DoubleJump();
			Yeet();
			Dive();
			Thrust();
			AirKick();
			Grab();
			WaterRun();
		}
		HUD();
	}

	//FixedUpdate is called once per "physics" frame
	void FixedUpdate()
	{
		//don't have to check if the game is paused for these because fixed updates stop while time-scale is 0
		WallRun();
		CheckIfGrounded();
		StickToGround();
		CounterSlope();
		SlopeSkatingAssist();
		ThrustForce();
		GrindOnRail();
		MyLateUpdate();             //MUST BE LAST IN FIXED UPDATE
	}

	private void LateUpdate()
	{
		MoveArmsWithCamera();
	}





	//called at the end of every fixed frame, but before unity calculates any collisions
	void MyLateUpdate()
	{
		//reset every collision before we calculate collisions to accurately keep track of what we're touching
		currentGroundAngle = 0;
		currentGround = Vector3.zero;
		currentWall = Vector3.zero;
	}


	void Timers()
	{
		if (timeSinceWallRunStart <= shortTimerStop)
			timeSinceWallRunStart += Time.deltaTime;

		if (timeSinceLastJump <= shortTimerStop)
			timeSinceLastJump += Time.deltaTime;

		if (!isGrounded && timeSinceGrounded < shortTimerStop)
			timeSinceGrounded += Time.deltaTime;

		if (timeSinceWallRunExit < shortTimerStop && !isWallRunning)
			timeSinceWallRunExit += Time.deltaTime;

		if (timeSinceLastYeet < yeetCoolDown)
			timeSinceLastYeet += Time.deltaTime;

		if (isOnWater && timeLeftToWaterRun >= 0)
			timeLeftToWaterRun -= Time.deltaTime;
	}

	void HUD()
	{
		if (References.isPaused)
			mySpeedometer.Disable();
		else
			mySpeedometer.Enable();
	}

	//keep the player stuck to the ground while they're grounded
	//this is striclty to keep the player grounded as long as they're supposed to be
	//if the player gains or loses velocity because of this force, it is doing too much
	void StickToGround()
	{

		Vector3 currentGroundStickForce = -currentGround * groundStickForce;

		if (currentGround != Vector3.zero && currentGroundAngle <= maxGroundAngle &&
			!isSkating)
		{
			myRB.AddForce(currentGroundStickForce);
		}
	}

	//this counters gravity to the extent that the player does not slide down slopes while running
	void CounterSlope()
	{
		if (!isSkating &&
			isGrounded &&
			currentGroundAngle != 0)
		{
			//set the direction of the vector to counter the force of gravity
			Vector3 counterForce = Vector3.ProjectOnPlane(Vector3.up, currentGround).normalized;

			//set the magnitude of the vector based on the angle of the ground
			counterForce *= Mathf.Sin(Mathf.Deg2Rad * currentGroundAngle) * Physics.gravity.magnitude * myRB.mass;
			myRB.AddForce(counterForce);
		}
	}

	//counters gravity to an extent when it favors the player's speed on a slope
	void SlopeSkatingAssist()
	{
		if (isSkating &&
			isGrounded &&
			currentGroundAngle != 0 &&
			(myRB.velocity.magnitude > slopeSkatingAssistStart || isThrusting))
		{
			//ask if we're thrusting to determine how much force to use
			float currentSlopeSkatingAssistForce = isThrusting ? thrustingSlopeSkatingAssistForce : slopeSkatingAssistForce;

			//set the direction of the vector to counter the force of gravity
			Vector3 counterForce = Vector3.ProjectOnPlane(Vector3.up, currentGround).normalized;

			//set the magnitude of the vector based on the angle of the ground
			counterForce *= Mathf.Sin(Mathf.Deg2Rad * currentGroundAngle) *                                         //sine of angle of the ground
							Physics.gravity.magnitude *                                                             //force of gravity on my rigidbodies
							myRB.mass *                                                                             //mass of my rigidbody
							currentSlopeSkatingAssistForce *                                                        //my own multiplier that can adjust the force assisting the player up the ramps
							Mathf.Clamp(Vector3.Dot(myRB.velocity.normalized, counterForce), 0, Mathf.Infinity);    //the similarity of the ramp's slope vector, and our normalized velocity
			myRB.AddForce(counterForce);
		}
	}
	
	void SkatingOrRunning()
	{
		if (Input.GetKey(skateButton))         //SKATING
		{	//then check our speed for the pitch
			float pitch = 0;
			if (MyLateralVelocity().magnitude <= skatingTopSpeed)
				pitch = 1;
			else if (MyLateralVelocity().magnitude < speedPitchCap)
				pitch = (Mathf.Pow(MyLateralVelocity().magnitude - skatingTopSpeed, 2)) / skatingPitchCurveFlatten + 1;
			else if (MyLateralVelocity().magnitude >= speedPitchCap)
				pitch = (Mathf.Pow(speedPitchCap - skatingTopSpeed, 2)) / skatingPitchCurveFlatten + 1;
			mySounds.skateboardSound.pitch = pitch;

			//mess with some of the sounds for the skateboard base on current player states
			//start with setting a volume inhibitor in the case we're not movings
			float skatingSoundVolumeInhibitor = MyLateralVelocity().magnitude < minimumSkateboardSoundSpeed ? MyLateralVelocity().magnitude / minimumSkateboardSoundSpeed : 1;

			//and if we're on the ground
			if (isGrounded)
				mySounds.skateboardSound.volume = skatingGroundVolume * skatingSoundVolumeInhibitor;
			else
				mySounds.skateboardSound.volume = skatingAirVolume * skatingSoundVolumeInhibitor;
			
			//this code runs once when skating starts
			if (!isSkating)
			{
				isSkating = true;
				if (isGrounded)
					mySounds.placingSkateboardSound.Play();
				mySounds.skateboardSound.Play();
			}
		}
		else			//RUNNING
		{
			//this code runs once when skating stops
			if (isSkating)
			{
				mySounds.skateboardSound.Pause();
				if (isGrounded)
					mySounds.comeOffSkateboardSound.Play();
				isSkating = false;
			}
		}
	}

	//set our movement based on what we're doing
	void MoveStatSet()
	{
		if (isWallRunning)
			currentMove = wallRunMove;
		else if (isSkating)
			currentMove = skatingMove;
		else if (isGrinding)
			currentMove = grindingMove;
		else
			currentMove = runningMove;
		
	}
	
	//change the player and camera orientation based on the mouse input
	void MouseLook()
	{
		//read the mouse inputs
		if (!rawMouseInput)
		{
			rotationX += Input.GetAxis("Mouse X") * xSens;
			if (!invertY)
				rotationY += Input.GetAxis("Mouse Y") * ySens;
			else
				rotationY -= Input.GetAxis("Mouse Y") * ySens;
		}
		else
		{
			rotationX += Input.GetAxisRaw("Mouse X") * xSens;
			if (!invertY)
				rotationY += Input.GetAxisRaw("Mouse Y") * ySens;
			else
				rotationY -= Input.GetAxisRaw("Mouse Y") * ySens;
		}

		//clamp rotation and apply floats to quaternions
		rotationY = Mathf.Clamp(rotationY, -maxVerticalRotaion, maxVerticalRotaion);
		Quaternion xQuaternion = Quaternion.AngleAxis(rotationX, Vector3.up);
		Quaternion yQuaternion = Quaternion.AngleAxis(rotationY, -Vector3.right);

		//rotate player and camera
		myRB.transform.localRotation = myRBOriginalRotation * xQuaternion;
		myCamera.transform.localRotation = myCameraOriginalRotation * yQuaternion;
	}

	void MoveArmsWithCamera()
	{
		float myArmAndBoardRotation = Mathf.Clamp(-rotationY, -37, 30);

		if (!isSkating)
		{
			myBoard.transform.RotateAround(myArmRotationPoint.position, myArmRotationPoint.right, myArmAndBoardRotation);
			myRightArm.transform.RotateAround(myArmRotationPoint.position, myArmRotationPoint.right, myArmAndBoardRotation);
			myLeftArm.transform.RotateAround(myArmRotationPoint.position, myArmRotationPoint.right, myArmAndBoardRotation);
		}
	}

	//move the player laterally based on the keyboard and physics settings
	void KeyboardMovement()
	{
		//keyboard inputs
		float xMovement = 0;
		float zMovement = 0;

		//use my own move input if it's applicable
		if (Input.GetKey(forwardButton))
			zMovement++;
		if (Input.GetKey(backwardButton))
			zMovement--;
		if (Input.GetKey(leftButton))
			xMovement--;
		if (Input.GetKey(rightButton))
			xMovement++;

		//use unity move input if it's applicable
		/*if (xMovement != 0 && zMovement != 0)
		{
			xMovement = Input.GetAxisRaw("Horizontal");
			zMovement = Input.GetAxisRaw("Vertical");
			Debug.Log("YOU'RE GAY YOU'RE GAY YOU'RE GAY YOU'RE GAY YOU'RE GAY YOU'RE GAY YOU'RE GAY");
		}*/

		//initialize our input vector
		moveInput = (transform.right * xMovement + transform.forward * zMovement).normalized;

		//if we're thrusting, no keyboard movement
		if (isThrusting)
			moveInput = myRB.velocity.magnitude > currentMove.topSpeed ? Vector3.zero :myRB.transform.forward;

		Vector3 velToPass = myImaginaryVel == Vector3.zero ? myRB.velocity : myImaginaryVel;

		//call the appropriate move function, whether we're on the ground or the air
		if (currentRail != null)
			MoveOnRail(velToPass, moveInput);
		else if (isGrounded)
			MoveWithFriction(velToPass, moveInput);
		else if (!isWallRunning)
			MoveWithNoFriction(velToPass, moveInput);
		else
			MoveOnWall(velToPass, moveInput);
	}

	/*
	 * moveOnLand applies fricion to our speed before calling accelerate
	 */
	void MoveWithFriction(Vector3 prevVelocity, Vector3 moveDir)
	{
		//save our magnitude
		float speed = prevVelocity.magnitude;
		//how much to "drop" the speed by this update
		float drop = 0;

		//set our friction
		float friction = currentMove.friction;
		
		if (speed != 0)//avoid divide by 0 errors
		{
			//define how much we have to slow down based on our speed and friction

			//if speed is less than 0, make control 0, otherwise, make control speed
			float control = speed;//< 0 ? 0 : speed;

			drop += control * friction * Time.deltaTime;

			//scale the velocity
			float newSpeed = speed - drop < 0 ? 0 : speed - drop;
			newSpeed = speed != 0 ? newSpeed / speed : 0;

			prevVelocity *= newSpeed;
			//prevVelocity *= Mathf.Max(speed - drop, 0) / speed; //Scale the velocity based on friction
		}

		//call our accelerate function
		Accelerate(prevVelocity, currentMove.groundAcceleration, moveDir);
	}

	/*
	 * Movement in air calls Accelerate without applying friction
	 */
	void MoveWithNoFriction(Vector3 prevVelocity, Vector3 moveDir)
	{
		Accelerate(prevVelocity, currentMove.airAcceleration, moveDir);
	}

	void MoveOnWall(Vector3 prevVelocity, Vector3 moveDir)
	{
		//flaten our wall plane; treat it as 90 degrees
		Vector3 ourWall = Vector3.ProjectOnPlane(currentWall, Vector3.up);

		//pulling off the wall nonsense
		//check if the player is trying to use moveDir to pull off the wall
		if (Vector3.Angle(moveDir, ourWall) <= wallPullOffAngle && moveDir != Vector3.zero)
			timeSinceStartedPullingOff += Time.deltaTime;
		else
			timeSinceStartedPullingOff = 0;
		//if they've pulled off for a significant amount of time, let them pull off
		if (timeSinceStartedPullingOff >= timeToPullOffWall)
			currentWall = Vector3.zero;

		//make sure the player can only move along the wall
		moveDir = Vector3.ProjectOnPlane(moveDir, ourWall);

		//save our magnitude
		float speed = prevVelocity.magnitude;
		float drop = 0;

		//set our friction
		float friction = currentMove.friction;

		if (speed != 0 && timeSinceWallRunStart > wallKickDuration)//avoid divide by 0 errors
		{
			//define how much we have to slow down based on our speed and friction

			//if speed is less than 0, make control 0, otherwise, make control speed
			float control = speed < 0 ? 0 : speed;

			drop += control * friction * Time.deltaTime;

			//scale the velocity
			float newSpeed = speed - drop < 0 ? 0 : speed - drop;
			newSpeed = speed != 0 ? newSpeed / speed : 0;

			prevVelocity *= newSpeed;
			//prevVelocity *= Mathf.Max(speed - drop, 0) / speed; //Scale the velocity based on friction
		}

		Accelerate(prevVelocity, currentMove.groundAcceleration, moveDir);
	}

	void MoveOnRail(Vector3 prevVelocity, Vector3 moveDir)
	{
		Vector3 tangent = currentRail.TangentAtPointOnSpline(pointOnRail);
		prevVelocity = Vector3.Dot(tangent, prevVelocity) < 0 ? -tangent.normalized * prevVelocity.magnitude : tangent.normalized * prevVelocity.magnitude;
		float speed = prevVelocity.magnitude;
		float drop = 0;
		float friction = currentMove.friction;
		moveDir = Vector3.Project(moveDir, tangent).normalized;

		if (speed != 0)//avoid divide by 0 errors
		{
			//define how much we have to slow down based on our speed and friction

			//if speed is less than 0, make control 0, otherwise, make control speed
			float control = speed < 0 ? 0 : speed;

			drop += control * friction * Time.deltaTime;

			//scale the velocity
			float newSpeed = speed - drop < 0 ? 0 : speed - drop;
			newSpeed = speed != 0 ? newSpeed / speed : 0;

			prevVelocity *= newSpeed;
			//prevVelocity *= Mathf.Max(speed - drop, 0) / speed; //Scale the velocity based on friction
		}

		AccelerateOnRail(prevVelocity, currentMove.groundAcceleration, moveDir, tangent);
	}

	/*===================================*\
    ||* the most important bit           |
    ||* MOVEMENT                         ||
    ||* thank you quake                  ||
    \*===================================*/
	void Accelerate(Vector3 prevVelocity, float accel, Vector3 wishDir)   //Kasokudo, 加速度
	{
		float wishSpeed, addSpeed, accelSpeed;

		wishSpeed = currentMove.topSpeed;

		//this is straight witchcraft. determine how much to accelerate
		float currentSpeed = Vector3.Dot(prevVelocity, wishDir); // Vector projection of Current velocity onto wishDir

		//the difference between our top speed and our current speed is what we'll be adding to our speed.
		addSpeed = wishSpeed - currentSpeed;

		//if we're going faster than our top speed, allow holding [W]
		if (addSpeed <= 0)
			addSpeed = 0;

		//bungus
		//apply our acceleration to a float
		accelSpeed = accel * Time.deltaTime * wishSpeed;

		//drop acceleration to top speed?
		if (accelSpeed > addSpeed)
			accelSpeed = addSpeed;

		//save what our new velocity should be
		Vector3 newVelocity = prevVelocity + wishDir * accelSpeed;

		//now apply our witchcraft to my velocity to accelerate
		myRB.velocity = newVelocity;
		myImaginaryVel = Vector3.zero;

		//make the player *feel* the speed
		SpeedEffects(myRB.velocity);
	}

	void AccelerateOnRail(Vector3 prevVel, float accel, Vector3 wishDir, Vector3 tangent)
	{
		float wishSpeed, addSpeed, accelSpeed;

		wishSpeed = currentMove.topSpeed;

		//this is straight witchcraft. determine how much to accelerate
		float currentSpeed = Vector3.Dot(prevVel, wishDir); // Vector projection of Current velocity onto wishDir

		//the difference between our top speed and our current speed is what we'll be adding to our speed.
		addSpeed = wishSpeed - currentSpeed;

		//if we're going faster than our top speed, allow holding [W]
		if (addSpeed <= 0)
			addSpeed = 0;

		//bungus
		//apply our acceleration to a float
		accelSpeed = accel * Time.deltaTime * wishSpeed;

		//drop acceleration to top speed?
		if (accelSpeed > addSpeed)
			accelSpeed = addSpeed;

		//save what our new velocity should be
		Vector3 newVelocity = prevVel + wishDir * accelSpeed;

		//move along the rail the appropriate distance
		float newSpeed = Vector3.Dot(newVelocity, tangent) < 0 ? -newVelocity.magnitude : newVelocity.magnitude;
		transform.position = currentRail.GetPointAtLinearDistance(railT, newSpeed * Time.deltaTime, out railT);

		//make sure our velocity is tangent to the spline now
		newVelocity = Vector3.Dot(newVelocity, tangent) < 0 ? -tangent.normalized * newVelocity.magnitude : tangent.normalized * newVelocity.magnitude;

		//now save the velocity to imaginary so everyone knows our "velocity"
		myImaginaryVel = newVelocity;
		myRB.velocity = Vector3.zero;

		SpeedEffects(myImaginaryVel);
	}

	//dependent on how fast the player is moving, add some elements to the HUD that indicates their speed
	void SpeedEffects(Vector3 velocity)
	{
		//update our speedometer
		mySpeedometer.SetSpeedDisplay(MyLateralVelocity().magnitude);

		//update our speed line generator
		int speedParticlesToGenerate = Mathf.RoundToInt(Mathf.Clamp(speedLineParticleMultiplier * (velocity.magnitude - skatingMove.topSpeed), 0, speedLineParticleMax));
		var emission = speedLinesParticleSystem.emission;
		emission.rateOverTime = speedParticlesToGenerate;

		//update our trail renderer
		if (MyLateralVelocity().magnitude > skatingTopSpeed && !speedTrailRenderer.emitting)
		{
			speedTrailRenderer.emitting = true;
		}
		else if (MyLateralVelocity().magnitude <= skatingTopSpeed && speedTrailRenderer.emitting)
		{
			speedTrailRenderer.emitting = false;
		}
	}

	//jump if the jump is available and the player wants to jump
	void Jump()
	{
		//can't jump while in the air
		if (timeSinceGrounded > coyoteTime && !isWallRunning && currentRail == null)
			jumpReady = false;

		//jump if jumping, and we have a jump ready
		if (Input.GetKeyDown(jumpButton) && jumpReady)
		{
			mySounds.jumpSound.Play();
			timeSinceLastJump = 0;
			jumpReady = false;
			isGrounded = false;
			currentRail = null;

			//should not be stuck to ground for the frame we jump, so nullify our ground stick force
			currentGround = Vector3.zero;

			//this is the actual jump. set the vertical velocity to [jump speed] + current vertical velocity.
			float thisJumpSpeed = myRB.velocity.y > 0 ? jumpSpeed + myRB.velocity.y : jumpSpeed;
			myRB.velocity = MyLateralVelocity() + new Vector3(0, thisJumpSpeed, 0);
		}

		if (isGrounded)
		{
			ReloadJump();
		}
	}

	/*===========================================================================================================\
	|* Super jump is the first "mario-style" ability                                                             |
	|* The player gets a higher jump by executing the super jump                                                 |
	|* the player must be skating and standing relatively still for their normal jump to turn into a super jump  |
	|* they can also super jump by moving at high speeds (superJumpSpeedMinimum) while running					 |
	\===========================================================================================================*/
	void SuperJump()
	{
		//jump if jumping, and we have a jump ready
		if (Input.GetKeyDown(jumpButton) && 
			jumpReady &&
			!isWallRunning &&
			((isSkating && MyLateralVelocity().magnitude <= superJumpSpeedMaximum) || 
			(!isSkating && MyLateralVelocity().magnitude >= superJumpSpeedMinimum)))				//only call super jump if we're under super jump conditions
		{
			//tell every function we'ce jumped
			timeSinceLastJump = 0;
			jumpReady = false;
			isGrounded = false;
			currentRail = null;

			//should not be stuck to ground for the frame we jump, so nullify our ground stick force
			currentGround = Vector3.zero;

			//this is the actual jump. set the vertical velocity to [super jump speed] + current vertical velocity.
			float thisJumpSpeed = myRB.velocity.y > 0 ? superJumpSpeed + myRB.velocity.y : superJumpSpeed;
			myRB.velocity = MyLateralVelocity() + new Vector3(0, thisJumpSpeed, 0);

			//play our particle system
			mySounds.superJumpSound.Play();//PlaySound("jumpSound");
			superJumpParticleSystem.Play();
			superJumpTrailParticleSystem.Play();
		}
	}

	//we save our speed while we're in the air to be used in other functions
	void SaveSpeed()
	{
		//only save a new speed if it's faster than any speed we've hit while we've been in the air so far
		if (MyLateralVelocity().magnitude > savedSpeed && !isGrounded)
			savedSpeed = MyLateralVelocity().magnitude;

		//reset saved speed if we touch the ground
		if (isGrounded)
			savedSpeed = 0;
	}

	//this is a second jump allowed in the air under specific conditions only; not any time the player is in the air.
	void DoubleJump()
	{
		//un-ready double jump if it's been too long since the wallrun
		if (timeSinceWallRunExit > timeToDoubleJumpAfterWallRun || isGrounded)
			doubleJumpReady = false;

		//determine our double jump height
		float thisDoubleJumpHeight = myRB.velocity.y > 0 ? doubleJumpHeight + myRB.velocity.y : doubleJumpHeight;

		//perform double jump
		if (Input.GetKeyDown(jumpButton) && doubleJumpReady && !isWallRunning)
		{
			//create some temp objects for defining our double jump vector
			Vector3 thisDoubleJumpVector = Vector3.zero;
			float thisDoubleJumpSpeed = 0;

			//decide what speed should be given to the player
			thisDoubleJumpSpeed = MyLateralVelocity().magnitude > doubleJumpMinLateralSpeed ? MyLateralVelocity().magnitude : doubleJumpMinLateralSpeed;

			//give our vector it's lateral direction
			thisDoubleJumpVector = moveInput * thisDoubleJumpSpeed;

			//if moveInput is 0, we should just use the lateralvelocity
			if (thisDoubleJumpVector == Vector3.zero)
				thisDoubleJumpVector = MyLateralVelocity();

			//and give it it's vertical strength
			thisDoubleJumpVector += new Vector3(0, thisDoubleJumpHeight, 0);

			//play my sound
			mySounds.doubleJumpSound.Play();

			//execute the double jump with the vector
			myRB.velocity = thisDoubleJumpVector;
			doubleJumpReady = false;
		}
	}

	/*
	 * yeeting happens when we're not skating
	 * this is how the player can throw objects
	 */
	void Yeet()
	{
		//create our thrust vector
		Vector3 yeetVector = myCamera.transform.forward;

		//if the player spins, they can reset yeet early
		if (!yeetReady)
		{
			spinSinceLastYeet += Input.GetAxisRaw("Mouse X") * xSens;
			if (Mathf.Abs(spinSinceLastYeet) >= requiredSpinToResetYeet)
				timeSinceLastYeet = yeetCoolDown;
		}

		//get yeet ready if it should be and isn't
		if(!yeetReady && timeSinceLastYeet >= yeetCoolDown)
		{
			yeetReady = true;
			mySounds.yeetReadySound.Play();
		}

		if (Input.GetKeyDown(yeetButton) && !isSkating && yeetReady)
		{
			//keep track that we've yeeted
			timeSinceLastYeet = 0;
			yeetReady = false;
			spinSinceLastYeet = 0;

			//give thrustVector it's magnitude
			yeetVector *= Mathf.Clamp(MyLateralVelocity().magnitude, yeetForce, Mathf.Infinity);

			//start our cool effect and make sound
			yeetParticleSystem.Play();
			mySounds.yeetSound.Play();

			//Throw everything around us in the direction we're looking
			for (int i = 0; i < myYeetTrigger.currentColliders.Count; i++)
			{
				GameObject currentObject = myYeetTrigger.currentColliders[i];

				if (currentObject == null)
					break;

				//if it isn't an NRG, throw it
				if (currentObject.GetComponent<NRGCapsuleBehavior>() == null)
					currentObject.GetComponent<Rigidbody>().AddForce(yeetVector.magnitude * (myCamera.transform.forward), ForceMode.Impulse);
				//if it IS an NRG, collect it
				else
				{
					//collect capsule
					currentObject.GetComponent<NRGCapsuleBehavior>().Collect();

					//recharge our yeet
					timeSinceLastYeet = yeetCoolDown;
				}
			}
		}
	}

	/*
	 * diving happens when we're skating in the air and click
	 */
	void Dive()
	{
		//regen our dive speed
		if (currentDiveSpeed != maxDiveSpeed)
		{
			if (currentDiveSpeed > maxDiveSpeed || isWallRunning)       //if our we're past the max dive speed or wall running, reset our dive speed
				currentDiveSpeed = maxDiveSpeed;

			if (currentDiveSpeed < maxDiveSpeed)                        //if we're under our max dive speed, recharge it
				currentDiveSpeed += Time.deltaTime * diveRegenRate;
		}

		//how close are we to our max dive? calculate
		float diveChargePercentage = (currentDiveSpeed - minDiveSpeed) / minMaxDiveSpeedDifference;

		//if we're in the air and skating; dive based on the camera
		if (Input.GetKeyDown(yeetButton) && diveReady && isSkating && !isGrounded && !isWallRunning)
		{
			//initialize our dive vector to our rigidbody's face
			Vector3 diveVector = myRB.transform.forward;

			//change the dive vector to the camera's face
			diveVector = myCamera.transform.forward;

			//get how close it is to the up vector (don't want the player to be able to dive "up")
			float upAngle = Vector3.Angle(diveVector, Vector3.up);
			//if it's too close, snap it down
			if (upAngle < minAngleTowardUpDive)
			{
				diveVector = Quaternion.AngleAxis(minAngleTowardUpDive - upAngle, myCamera.transform.right) * diveVector;
			}

			//tell every function we've dove
			diveReady = false;
			
			//give diveVector it's magnitude. use the largest of currentSpeed, savedSpeed, and minDiveSpeed
			float thisDiveSpeed = MyLateralVelocity().magnitude * diveSpeedPenalty > currentDiveSpeed ? MyLateralVelocity().magnitude * diveSpeedPenalty : currentDiveSpeed;
			if (thisDiveSpeed < savedSpeed * diveSpeedPenalty)
				thisDiveSpeed = savedSpeed * diveSpeedPenalty;

			diveVector *= thisDiveSpeed * 0.95f;

			//apply discharged dive penalty
			diveVector *= dischargedDivePenalty + ((1 - dischargedDivePenalty) * diveChargePercentage);

			//play our particle system and make sound (and stop superjump's particle system if it's going)
			diveParticleSystem.Play();
			superJumpTrailParticleSystem.Stop();
			mySounds.diveSound.pitch = diveChargePercentage;
			mySounds.diveSound.Play();

			//apply thrustVector to my velocity
			myRB.velocity = diveVector;
			
			//put our dive on cooldown
			currentDiveSpeed = minDiveSpeed;
		}
	}

	//called every frame
	void Thrust()
	{
		if (Input.GetKey(yeetButton) && isGrounded && isSkating && !isThrusting)
		{
			isThrusting = true;
			thrustParticleSystem.Play();
			mySounds.thrustSound.Play();
			mySounds.thrustStartSound.Play();
			myRB.mass = thrustMass;
		}
		else if ((!Input.GetKey(yeetButton) || !isGrounded || !isSkating ) && isThrusting)
		{
			isThrusting = false;
			thrustParticleSystem.Stop();
			mySounds.thrustSound.Pause();
			mySounds.thrustEndSound.Play();
			myRB.mass = defaultMass;
		}
	}

	//called every fixed frame
	void ThrustForce()
	{
		if (isThrusting)
		{
			//create vectors
			Vector3 wishVector = Vector3.ProjectOnPlane(myRB.transform.forward, currentGround).normalized;
			Vector3 thrustVector;
			Vector3 lateralVelocityOnPlatform = Vector3.ProjectOnPlane(MyLateralVelocity(), currentGround);
			float currentSpeed = Vector3.Dot(wishVector, lateralVelocityOnPlatform);

			//determine how to calibrate the force based on how fast we're currently going
			if (lateralVelocityOnPlatform.magnitude < thrustMaxSpeed || currentSpeed <= 0)
			{
				//give our thrust vector magnitude based on it's direction and our velocity's direction
				thrustVector = wishVector * (thrustMaxSpeed - currentSpeed) * thrustForce;
			}
			else
			{
				//if we're at our top speed, apply a force specifically to turn, and minimally to strafe.
				thrustVector = Vector3.ProjectOnPlane(wishVector * thrustForce * thrustTurnAssist, lateralVelocityOnPlatform);
			}

			//set some effects variables based on the thrust force for this frame
			float currentThrustForce = thrustVector.magnitude;

			//change our thrust particle system based on the force it applies to us
			int thrustParticlesToGenerate = Mathf.RoundToInt(currentThrustForce);
			var emission = thrustParticleSystem.emission;
			emission.rateOverTime = thrustParticlesToGenerate;

			//change our thrust sound with the same criteria
			mySounds.thrustSound.volume = ((thrustSoundDefaultVolume - thrustSoundMinVolume) / thrustMaxForce) * currentThrustForce + thrustSoundMinVolume;

			//apply force
			myRB.AddForce(thrustVector, ForceMode.Acceleration);
		}
	}

	//airkick is a glorified, kinda shitty double jump
	void AirKick()
	{

		if (!isGrounded && !isWallRunning)
		{
			//get the degree of spin while we're in the air
			spinSinceGrounded += Input.GetAxisRaw("Mouse X") * xSens;

			//execute air kick if we've spun enough in the air
			if (Mathf.Abs(spinSinceGrounded) >= requiredSpinToResetYeet && airKickReady)
			{
				mySounds.airKickSound.Play();
				airKickReady = false;
				float thisAirKickSpeed = myRB.velocity.y > 0 ? airKickSpeed + myRB.velocity.y : airKickSpeed;
				myRB.velocity = MyLateralVelocity() + new Vector3(0, thisAirKickSpeed, 0);
			}
		}
	}

	//this is how the player can grab things and use them
	void Grab()
	{

	}

	void WaterRun()
	{
		if (timeLeftToWaterRun <= 0 || MyLateralVelocity().magnitude < minSpeedToWaterRun)
		{
			Physics.IgnoreLayerCollision(9, 4, true);
		}
		else
		{
			Physics.IgnoreLayerCollision(9, 4, false);
		}
	}

	void GrindOnRail()
	{
		if (currentRail != null)
		{
			tangentVector = currentRail.TangentAtPointOnSpline(transform.position);
			pointOnRail = currentRail.ClosestPoint(transform.position);
			
			//This code runs once when grinding starts
			if (!isGrinding)
			{
				Physics.IgnoreLayerCollision(9, 6, true);
				myRB.useGravity = false;

				isGrinding = true;
				myRB.velocity = Vector3.Dot(myRB.velocity, tangentVector) < 0 ? -tangentVector.normalized * myRB.velocity.magnitude : tangentVector.normalized * myRB.velocity.magnitude;
				transform.position = currentRail.ClosestPoint(transform.position) + playerGrindingVerticalOffset;
				Debug.LogError("nice");
			}
			//this code runs continuously while grinding

			Vector3 playerFeet = transform.position - playerGrindingVerticalOffset;

			if (!isSkating || Vector3.Distance(pointOnRail, playerFeet) > minDistanceToStayOnRail)
				StopGrinding();

		}
		else if(isGrinding)		//this code runs once when grinding stops
		{
			Physics.IgnoreLayerCollision(9, 6, false);
			myRB.useGravity = true;

			isGrinding = false;
		}

	}

	void StopGrinding()
	{
		currentRail = null;
		railT = -1;
	}

	/*
	 * resets the jump so the player can jump again
	 */
	public void ReloadJump()
	{
		if (!jumpReady)
		{
			mySounds.landSound.Play();
			jumpReady = true;
			diveReady = true;
			airKickReady = true;
			spinSinceGrounded = 0;		//this is for airkick
		}
	}
	
	void WallRun()
	{
		Vector3 ourWall = Vector3.ProjectOnPlane(currentWall, Vector3.up);      //determine our wall run candidate and treat it as perfectly vertical

		if (currentWall != Vector3.zero &&																				//if we're in contact with a viable wall
			isSkating &&                                                                                                //can't wall run while *not* skating
			timeSinceGrounded > wallRunGroundedBuffer &&                                                                //have to be off the ground for a period of time before a wall run can be initiated
			timeSinceLastJump > wallRunGroundedBuffer &&                                                                //jumping exits the wall run
			(isWallRunning || !TwoWallsTooClose(lastWall, currentWall)) &&                                              //can't initiate a wall run on a wall that's too similar in angle to the last one
			(!(Vector3.Angle(moveInput, ourWall) <= wallPullOffAngle) || moveInput == Vector3.zero || isWallRunning) && //don't start wall run if we're pulling away from the wall
			(!isWallRunning || timeSinceWallRunStart < wallRunDuration))												//being on the wall for too long exits the wall run
		{
			//(this code runs once when the wall run starts) 
			if (!isWallRunning)
			{
				//reset our jump
				ReloadJump();

				//start the wall run timer
				timeSinceWallRunStart = 0;

				//zero out my lateral velocity so we don't just bounce off the wall
				Vector3 myProposedWallRunVelocity = Vector3.ProjectOnPlane(MyLateralVelocity(), ourWall).normalized;
				myRB.velocity = myProposedWallRunVelocity * Vector3.Dot(myProposedWallRunVelocity, myRB.velocity);

				//stop superJump trail
				superJumpTrailParticleSystem.Stop();
			}
			
			//(this code runs continuously while wall running)

			//negate the force of gravity on the player while wall running
			myRB.AddForce(-Physics.gravity * myRB.mass);

			//kill vertical velocity
			myRB.velocity = MyLateralVelocity();

			//tell every other function we're wall running
			isWallRunning = true;

		}else if (isWallRunning)    //(this code runs once when the wall run ends)
		{
			//tell every other function we've stopped wall running
			isWallRunning = false;
			timeSinceWallRunExit = 0;

			//set this wall as the last wall we ran on
			lastWall = ourWall;

			//reset our double jump
			doubleJumpReady = true;

			//jump off the wall
			if (timeSinceLastJump < wallRunGroundedBuffer)                  //don't apply this force if the player pulled or fell off the wall
			{
				//create the force vector with which we will jump off the wall
				Vector3 wallJumpForce = (wallJumpAwayForce * ourWall) + (moveInput * wallJumpForwardForce);
				wallJumpForce = wallJumpForce.normalized * wallJumpMaxForce;

				myRB.AddForce(wallJumpForce, ForceMode.Impulse);
			}

		}
		
	}

	//return the horizontal velocity of the player character
	Vector3 MyLateralVelocity()
	{
		Vector3 latVel;
		if (myImaginaryVel == Vector3.zero)
			latVel = new Vector3(myRB.velocity.x, 0, myRB.velocity.z);
		else
			latVel = new Vector3(myImaginaryVel.x, 0, myImaginaryVel.z);
		return latVel;
	}


	private void OnCollisionExit(Collision other)
	{
		GroundAndWallNormal(other);
	}

	private void OnCollisionStay(Collision other)
	{
		GroundAndWallNormal(other);
	}

	private void OnCollisionEnter(Collision collision)
	{
		//if we've collided with water
		if (collision.gameObject.layer == 4)
		{
			isOnWater = true;
		} else
		{
			isOnWater = false;
			timeLeftToWaterRun = waterRunTimeLimit;
		}

	}

	void GroundAndWallNormal(Collision other)
	{
		//see if any of the contacts of this collision are shallow enough to be the ground
		float lowestNormalAngle = 180;
		Vector3 lowestNormal = Vector3.zero;
		for (int k = 0; k < other.contactCount; k++)
		{
			Vector3 normal = other.contacts[k].normal;
			if (Vector3.Angle(Vector3.up, normal) < lowestNormalAngle)
			{
				lowestNormalAngle = Vector3.Angle(Vector3.up, normal);
				lowestNormal = normal;
			}
		}
		
		//if any part of this collision is shallow enough to be the ground, then this is the collision with the ground
		if (lowestNormalAngle <= maxGroundAngle)
		{
			currentGroundAngle = lowestNormalAngle;
			currentGround = lowestNormal;
		}

		//check if we're touching a wall before we start asking unity about it
		if (other.contactCount != 0)
		{
			/*====================================================================*\
			 * set our wallrun information if we're in contact with a viable wall *
			 * very important code here											  *
			 * use this to determine which walls can be run on					  *
			\*====================================================================*/

			bool wallRunViable = false;

			if (other.contactCount >= 2)
				wallRunViable = true;
			if (other.contactCount == 1 && !wallRunViable)		//if the single contact point is within the flat part of the bean, the wall is viable
			{
				wallRunViable = Mathf.Abs(other.contacts[0].point.y - myRB.transform.position.y) <= distanceFromCenterToEdgeOfPlayer;
			}

			wallAngle = Vector3.Angle(Vector3.up, other.contacts[0].normal);
			if (wallAngle >= wallRunMinAngle &&														//wall has to be steep...
				wallAngle <= wallRunMaxAngle &&                                                     //...but not too steep
				other.gameObject.GetComponent<Rigidbody>() == null &&                               //can't wall run on physics objects
				wallRunViable &&                                                                    //collision between us and the wall can't occur on our head or feet
				other.collider.gameObject.layer != LayerMask.NameToLayer("Non-Wall"))               //can't wall run on "non-walls"
			{
				//if the conditions for wall running are met, set this as our wall
				if (currentWall == Vector3.zero && savedSpeed < other.relativeVelocity.magnitude)
					savedSpeed = other.relativeVelocity.magnitude;

				currentWall = other.contacts[0].normal;
			}
			
		}

	}

	//returns whether the player is on the ground or not, with the assistance of "GroundAndWallNormal()"
	void CheckIfGrounded()
	{
		bool grounded;

		//new way of checking grounded: when we touch the ground, save the collision, and always check to see if *that* collision ever exits in "OnCollisionExit"
		grounded = currentGround != Vector3.zero;
		grounded &= timeSinceLastJump >= hardJumpCooldown;

		if (grounded)
		{
			timeSinceGrounded = 0;
			currentWall = Vector3.zero;
			lastWall = Vector3.zero;
		}

		isGrounded = grounded || currentRail != null;
	}
	
	//two walls need to be different by a certain margin for a wall run to be possible
	bool TwoWallsTooClose(Vector3 a, Vector3 b)
	{
		bool tooClose;
		tooClose = (Vector3.Angle(a, b)) < wallAngleDifferenceThreshold;
		tooClose &= (a != Vector3.zero) && (b != Vector3.zero);
		return tooClose;
	}

	public void SetCurrentRail (GrindyRailBehavior theRail)
	{
		//make it so you can't grind for [0.1f] seconds after you come off a rail.
		//make it so you can't grind for [0.1f] seconds after you come off a rail.
		//make it so you can't grind for [0.1f] seconds after you come off a rail.
		//make it so you can't grind for [0.1f] seconds after you come off a rail.
		//make it so you can't grind for [0.1f] seconds after you come off a rail.
		//make it so you can't grind for [0.1f] seconds after you come off a rail.
		//make it so you can't grind for [0.1f] seconds after you come off a rail.
		//make it so you can't grind for [0.1f] seconds after you come off a rail.
		if (isSkating)
			currentRail = theRail;
	}

	//these two getters are for the character animation controller script.
	//thanks C# for not having friends.
	public bool GetBool(string boolName) 
	{
		switch (boolName)
		{
			case "isThrusting":
				return isThrusting;
			case "isSkating":
				return isSkating;
			case "isGrounded":
				return isGrounded;
			case "isWallRunning":
				return isWallRunning;
			default:
				Debug.LogError("Asked to get Boolean that doesn't exist [retard]: " + boolName);
				return false;
		}
	}

	public Vector3 GetVector3(string vectorName)
	{
		switch (vectorName)
		{
			case "currentGround":
				return currentGround;
			case "currentWall":
				return currentWall;
			case "myCamera.transform.forward":
				return myCamera.transform.forward;
			case "myRB.transform.forward":
				return myRB.transform.forward;
			case "MyLateralVelocity()":
				return MyLateralVelocity();
			default:
				Debug.LogError("Asked to get a V3 that doesn't exist [moron]: " + vectorName);
				return Vector3.zero;
		}
	}

	public Vector3 velocity => myImaginaryVel == Vector3.zero ? myRB.velocity : myRB.velocity;
}
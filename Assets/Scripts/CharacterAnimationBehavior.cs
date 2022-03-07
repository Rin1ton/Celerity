using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimationBehavior : MonoBehaviour
{


	readonly float minRunAnimSpeed = 1.5f;
    public Animator myAnimator;
	PlayerBehavior ourPlayer;
	

    // Start is called before the first frame update
    void Start()
    {
        //anim = GetComponent<Animator>();
		ourPlayer = References.thePlayer.GetComponent<PlayerBehavior>();
		//runForwardAnim = myAnimatorController.Get
    }

    void Update()
    {
        if (!References.isPaused)
        {
			SkatingOrRunning();
			Running();
			Thrust();
		}
    }

	void SkatingOrRunning()
	{
		//be on the board or not
		if (ourPlayer.GetBool("isSkating") && !myAnimator.GetBool("isSkating"))
		{
			myAnimator.Play("Get On Board");
			myAnimator.SetBool("isSkating", true);
		}
		else if (!ourPlayer.GetBool("isSkating") && myAnimator.GetBool("isSkating"))
		{
			myAnimator.Play("Get Off Board");
			myAnimator.SetBool("isSkating", false);
		}
	}

	void Running()
	{
		if (!ourPlayer.GetBool("isSkating"))
		{
			//move our legs while running
			if (ourPlayer.GetVector3("MyLateralVelocity()").magnitude > minRunAnimSpeed && !myAnimator.GetBool("isSkating"))
			{
				myAnimator.SetBool("isStill", false);
				myAnimator.SetFloat("runAnimSpeed", ourPlayer.GetVector3("MyLateralVelocity()").magnitude * 0.1f);
			}
			else
			{
				myAnimator.SetBool("isStill", true);
			}

			//play jump and fall anims
			if (!ourPlayer.GetBool("isGrounded"))
			{
				myAnimator.SetBool("isGrounded", false);
				if (ourPlayer.GetVector3("myRB.velocity").y >= 0)
					myAnimator.SetBool("isFalling", false);
				else
					myAnimator.SetBool("isFalling", true);
			}

			//play land anim
			if (ourPlayer.GetBool("isGrounded") && !myAnimator.GetBool("isGrounded"))
			{
				myAnimator.Play("Running Land");
				myAnimator.SetBool("isGrounded", true);
			}
		}
	}

	void Thrust()
	{
		//Thrust
		if (ourPlayer.GetBool("isThrusting") && !myAnimator.GetBool("isThrusting"))
		{
			myAnimator.Play("Start Thrusting");
			myAnimator.SetBool("isThrusting", true);
		}
		else if (!ourPlayer.GetBool("isThrusting") && myAnimator.GetBool("isThrusting") && ourPlayer.GetBool("isSkating"))
		{
			myAnimator.Play("Stop Thrusting");
			myAnimator.SetBool("isThrusting", false);
		}
	}
}

/*if (Input.GetKey(ourPlayer.skateButton))
{
	anim.SetBool("isRunning", false);
	anim.SetBool("isSkating", true);
}
else
{
	anim.SetBool("isRunning", true);
	anim.SetBool("isSkating", false);
}*/

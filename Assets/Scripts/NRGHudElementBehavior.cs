using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NRGHudElementBehavior : MonoBehaviour
{

	readonly float spinFactor = 200;
	readonly float NRGSpinFactor = 600;
	readonly float xSpin = -0.25f;
	readonly float ySpin = 1.35f;
	readonly float zSpin = 0;
	public GameObject myCapsule;
	public GameObject myNRG;

	// Start is called before the first frame update
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
		if (myCapsule != null)
			myCapsule.transform.Rotate(xSpin * Time.deltaTime * spinFactor,
									   ySpin * Time.deltaTime * spinFactor,
									   zSpin * Time.deltaTime * spinFactor,
									   Space.World);
		if (myNRG != null)
				myNRG.transform.Rotate(-Vector3.forward* -NRGSpinFactor* Time.deltaTime, Space.Self);
	}
}

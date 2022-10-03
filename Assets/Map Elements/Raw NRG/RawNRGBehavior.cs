using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class RawNRGBehavior : MonoBehaviour
{

    public GameObject myLightning;
	readonly float NRGSpinFactor = 600;

	// Start is called before the first frame update
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Spin();
    }

    void Spin()
    {
		myLightning.transform.Rotate(Vector3.up * -NRGSpinFactor * Time.deltaTime, Space.World);
	}

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeTrialEndBehavior : MonoBehaviour
{
    //NOTE: this script runs on the the colored ring below
    public TimeTrialStartBehavior myStart;
    public Renderer myOrbRenderer;
    Renderer myRingRenderer;

    // Start is called before the first frame update
    void Start()
    {
        myRingRenderer = GetComponent<Renderer>();
    }

    public void SetMyColor(Color color)
    {
        if (myOrbRenderer != null)
        myOrbRenderer.material.SetColor("_Color", color);
        myRingRenderer.material.SetColor("_Color", color);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	private void OnTriggerEnter(Collider other)
	{
        if (other.GetComponent<PlayerBehavior>() != null) 
            myStart.PlayerCrossedFinish();
	}

}

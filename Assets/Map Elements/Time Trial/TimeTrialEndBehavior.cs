using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeTrialEndBehavior : MonoBehaviour
{
    //NOTE: this script runs on the the colored ring below
    public GameObject myOrb;
    Renderer myOrbRenderer;
    Renderer myRingRenderer;

    // Start is called before the first frame update
    void Start()
    {
        myOrbRenderer = myOrb.GetComponent<Renderer>();
        myRingRenderer = GetComponent<Renderer>();
    }

    public void SetMyColor(Color color)
    {
        myOrbRenderer.material.SetColor("_Color", color);
        myRingRenderer.material.SetColor("_Color", color);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

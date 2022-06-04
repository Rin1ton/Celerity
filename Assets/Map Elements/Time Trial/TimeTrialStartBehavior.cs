using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeTrialStartBehavior : MonoBehaviour
{

    public GameObject myLeftPillarHead;
    public GameObject myRightPillarHead;
    public GameObject myCrossingLine;
    Renderer myLeftPillarHeadRenderer;
    Renderer myRightPillarHeadRenderer;
    Renderer myCrossingLineRenderer;

    // Start is called before the first frame update
    void Start()
    {
        myLeftPillarHeadRenderer = myLeftPillarHead.GetComponent<Renderer>();
        myRightPillarHeadRenderer = myRightPillarHead.GetComponent<Renderer>();
        myCrossingLineRenderer = myCrossingLine.GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

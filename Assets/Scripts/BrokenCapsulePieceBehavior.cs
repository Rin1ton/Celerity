using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrokenCapsulePieceBehavior : MonoBehaviour
{

	readonly float minlife = 2.2f;
	readonly float maxLife = 4.5f;
	readonly float timeToShrink = 0.5f;
	readonly float shrinkFactor = 4;
	float timeToLive;

	void Awake()
	{
		transform.parent.gameObject.GetComponent<BrokenCapBehavior>().myPieces.Add(gameObject);
	}

    // Start is called before the first frame update
    void Start()
    {
		timeToLive = Random.Range(minlife, maxLife);
	}

    // Update is called once per frame
    void Update()
    {
		timeToLive -= Time.deltaTime;
		if (timeToLive <= 0)
		{
			transform.localScale -= new Vector3(shrinkFactor * Time.deltaTime, shrinkFactor * Time.deltaTime, shrinkFactor * Time.deltaTime);
			if (timeToLive <= 0 - timeToShrink)
			{
				gameObject.GetComponent<RemoveMeFromListBehavior>().RemoveMeFromAllYeetLists();
				Destroy(gameObject);
			}
		}
    }
}

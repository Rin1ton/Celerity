using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VillagerMovement : MonoBehaviour
{
    public Transform target;
    float movementSpeed = 10;
	float acceptableWaypointDistance = .5f;
    Vector3[] path;
    int targetIndex;
	Rigidbody myRB;

	private void Awake()
	{
		myRB = GetComponent<Rigidbody>();
	}

	private void Start()
	{
		PathRequestManager.RequestPath(transform.position, target.position, OnPathFound);
	}

	public void OnPathFound(Vector3[] newPath, bool pathSuccessful)
	{
		if (pathSuccessful)
		{
			path = newPath;
			StopCoroutine("FollowPath");
			StartCoroutine("FollowPath");
		}
	}

	IEnumerator FollowPath()
	{
		Vector3 currentWaypoint = path[0];

		while (true)
		{
			if (Vector3.Distance(transform.position, currentWaypoint) <= acceptableWaypointDistance)
			{
				targetIndex++;
				if (targetIndex >= path.Length)
				{
					yield break;
				}
				currentWaypoint = path[targetIndex];
			}

			myRB.velocity = (currentWaypoint - transform.position).normalized * movementSpeed;
			yield return null;

		}
	}
}

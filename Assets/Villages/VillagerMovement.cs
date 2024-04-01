using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VillagerMovement : MonoBehaviour
{
    float movementSpeed = 10;
	float acceptableWaypointDistance = .125f;
    Vector3[] path;
    int targetIndex;
	Rigidbody myRB;
	VillagerDesires myDesires;

	private void Awake()
	{
		myRB = GetComponent<Rigidbody>();
		myDesires = GetComponent<VillagerDesires>();
	}

	public enum station
	{
		store = 0,
		barracks = 1,
		theater = 2,
		factory = 3
	}

	public void GoToStation(station targetStation)
	{
		Transform targetPosition;

		switch(targetStation)
		{
			case station.store:
				targetPosition = Grid.instance.store;
				break;
			case station.barracks:
				targetPosition = Grid.instance.barracks;
				break;
			case station.theater:
				targetPosition = Grid.instance.theater;
				break;
			case station.factory:
				targetPosition = Grid.instance.factory;
				break;
			default:
				targetPosition = null;
				Debug.LogError($"NO SUCH STATION: {targetStation}!!!");
				break;
		}

		PathRequestManager.RequestPath(transform.position, targetPosition.position, OnPathFound);
	}

	public void GoToStation(int targetStation)
	{
        Transform targetPosition;

        switch (targetStation)
        {
            case 0:
                targetPosition = Grid.instance.store;
                break;
            case 1:
                targetPosition = Grid.instance.barracks;
                break;
            case 2:
                targetPosition = Grid.instance.theater;
                break;
            case 3:
                targetPosition = Grid.instance.factory;
                break;
            default:
                targetPosition = null;
                Debug.LogError($"NO SUCH STATION: {targetStation}!!!");
                break;
        }

        PathRequestManager.RequestPath(transform.position, targetPosition.position, OnPathFound);
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
			if (Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(currentWaypoint.x, currentWaypoint.z)) <= acceptableWaypointDistance)
			{
				targetIndex++;
				if (targetIndex >= path.Length)
				{
					myDesires.CurrentDesireFulfilled();
					yield break;
				}
				currentWaypoint = path[targetIndex];
			}

			Vector3 newVelocity = new Vector3(
				((currentWaypoint - transform.position).normalized * movementSpeed).x, 
				0, 
				((currentWaypoint - transform.position).normalized * movementSpeed).z);

			myRB.velocity = newVelocity;
			yield return null;

		}
	}

    private void OnDrawGizmos()
    {
        if (path != null)
		{
			for (int i = targetIndex; i < path.Length; i++)
			{
				Gizmos.color = Color.green;
				Gizmos.DrawSphere(path[i], .5f);

				if (i == targetIndex)
				{
					Gizmos.DrawLine(transform.position, path[i]);
				}
				else
				{
					Gizmos.DrawLine(path[i - 1], path[i]);
				}
			}
		}
    }
}

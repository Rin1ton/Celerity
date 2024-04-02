using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VillagerMovement : MonoBehaviour
{
	public enum station
	{
		store = 0,
		barracks = 1,
		theater = 2,
		factory = 3
	}

	readonly float movementForce = 12;
	readonly float movementFriction = 12;
	readonly float movementSpeed = 8;
	readonly float stuckCheckInterval = 1;
	readonly float stuckCheckTolerance = 0.1f;
	readonly float shoveModifier = 0.15f;

	float acceptableWaypointDistance = .125f;
	Vector3[] path;
	Vector3 currentWaypoint = Vector3.zero;
	int targetIndex;
	Rigidbody myRB;
	VillagerDesires myDesires;

	private void Awake()
	{
		myRB = GetComponent<Rigidbody>();
		myDesires = GetComponent<VillagerDesires>();
	}

	private void FixedUpdate()
	{
		ApplyMovementForce();
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
		currentWaypoint = path[0];
		targetIndex = 0;
		Vector3 oldPosition = transform.position;
		float timeSinceCheckedIfStuck = 0;

		while (true)
		{
			timeSinceCheckedIfStuck += Time.deltaTime;
			if (timeSinceCheckedIfStuck >= stuckCheckInterval)
			{
				if (Vector3.Distance(oldPosition, transform.position) <= stuckCheckTolerance)
				{
					Vector3 thisShove = new Vector3(
					((currentWaypoint - transform.position).normalized * movementSpeed).x,
					0,
					((currentWaypoint - transform.position).normalized * movementSpeed).z) * myDesires.TryToShove() * shoveModifier;
					if (thisShove.magnitude != 0)
						myRB.AddForce(thisShove, ForceMode.Impulse);
				}
				else
				{
					oldPosition = transform.position;
					timeSinceCheckedIfStuck = 0;
				}
			}

			if (Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(currentWaypoint.x, currentWaypoint.z)) <= acceptableWaypointDistance)
			{
				targetIndex++;
				if (targetIndex >= path.Length)
				{
					myDesires.CurrentDesireFulfilled();
					path = null;
					yield break;
				}
				currentWaypoint = path[targetIndex];
			}

			/*Vector3 newVelocity = new Vector3(
				((currentWaypoint - transform.position).normalized * movementSpeed).x, 
				0, 
				((currentWaypoint - transform.position).normalized * movementSpeed).z);

			myRB.velocity = newVelocity;*/
			yield return null;

		}
	}

	void ApplyMovementForce()
	{
		float speed = myRB.velocity.magnitude;
		float drop = 0;

		float friction = movementFriction;

		if (speed != 0)
		{
			float control = speed;

			drop += control * friction * Time.deltaTime;

			float newSpeed = speed - drop < 0 ? 0 : speed - drop;
			newSpeed = speed != 0 ? newSpeed / speed : 0;

			myRB.velocity *= newSpeed;
		}
		if (path != null)
		{
			Vector3 wishVector = new Vector3(
				((currentWaypoint - transform.position).normalized * movementSpeed).x,
				0,
				((currentWaypoint - transform.position).normalized * movementSpeed).z).normalized;

			float wishSpeed, addSpeed, accelSpeed;

			wishSpeed = movementSpeed;

			float currentSpeed = Vector3.Dot(myRB.velocity, wishVector);

			addSpeed = wishSpeed - currentSpeed;

			if (addSpeed <= 0)
				addSpeed = 0;

			accelSpeed = movementForce * Time.deltaTime * wishSpeed;

			if (accelSpeed > addSpeed)
				accelSpeed = addSpeed;

			Vector3 newVelocity = myRB.velocity + wishVector * accelSpeed;

			myRB.velocity = newVelocity;
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

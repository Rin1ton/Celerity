using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;

//[RequireComponent(typeof(LineRenderer), typeof(SplineContainer))]

public class GrindyRailBehavior : MonoBehaviour
{

	Spline m_Spline;
	//LineRenderer m_Line;
	bool m_Dirty;
	Vector3[] m_Points;
	float segmentAvgLength = 0;
	float segmentMaxLength = 0;
	PlayerBehavior thePlayerBehavior;

	//
	readonly int segmentsPerMeter = 10;

	int m_Segments;

	void Awake()
	{
		m_Spline = GetComponent<SplineContainer>().Spline;
		//m_Line = GetComponent<LineRenderer>();
		m_Spline.changed += () => m_Dirty = true;
		m_Segments = Mathf.RoundToInt(m_Spline.GetLength() * segmentsPerMeter);
		
	}

	void Start()
	{
		//
		thePlayerBehavior = References.thePlayer.GetComponent<PlayerBehavior>();

		// It's nice to be able to see resolution changes at runtime
		if (m_Points?.Length != m_Segments)
		{
			m_Dirty = true;
			m_Points = new Vector3[m_Segments];
			//m_Line.loop = m_Spline.Closed;
			//m_Line.positionCount = m_Segments;
		}

		if (!m_Dirty)
			return;

		m_Dirty = false;

		for (int i = 0; i < m_Segments; i++)
		{
			m_Points[i] = m_Spline.EvaluatePosition(i / (m_Segments - 1f));
			//adjust by the transform of the spline, so we aren't at world origin
			m_Points[i] += transform.position;

			if (i > 0)
			{
				float thisSegmentsLength = Vector3.Distance(m_Points[i], m_Points[i - 1]);
				segmentAvgLength += thisSegmentsLength;
				segmentMaxLength = thisSegmentsLength > segmentMaxLength ? thisSegmentsLength : segmentMaxLength;

			}
		}

		segmentAvgLength /= m_Segments;

		Debug.Log("AVG length: " + segmentAvgLength + " MAX length: " + segmentMaxLength);
		Debug.Log(m_Spline.GetLength() / m_Segments);

		//m_Line.SetPositions(m_Points);
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject == References.thePlayer)
		{
			thePlayerBehavior.SetCurrentRail(this);
			Debug.Log("Ow!");
		}
	}

	public Vector3 ClosestPoint(Vector3 queryPoint)
	{
		int relevantPoint = 0;
		float distanceToClosestPoint = Mathf.Infinity;
		float currentDistance;
		for (int currentPoint = 0; currentPoint < m_Points.Length; currentPoint++)
		{
			currentDistance = Vector3.Distance(m_Points[currentPoint], queryPoint);
			if (currentDistance < distanceToClosestPoint)
			{
				distanceToClosestPoint = currentDistance;
				relevantPoint = currentPoint;
			}
		}

		return m_Points[relevantPoint];
	}

	private float ClosestT(Vector3 queryPoint)
	{
		int relevantPoint = 0;
		float distanceToClosestPoint = Mathf.Infinity;
		float currentDistance;
		for (int currentPoint = 0; currentPoint < m_Points.Length; currentPoint++)
		{
			currentDistance = Vector3.Distance(m_Points[currentPoint], queryPoint);
			if (currentDistance < distanceToClosestPoint)
			{
				distanceToClosestPoint = currentDistance;
				relevantPoint = currentPoint;
			}
		}

		return relevantPoint;
	}

	public Vector3 TangentAtPointOnSpline(Vector3 queryPoint)
	{
		Vector3 relevantPointOnSpline = ClosestPoint(queryPoint);
		return m_Spline.EvaluateTangent(ClosestT(queryPoint) / (m_Segments - 1f));
	}

	public Vector3 GetPointAtLinearDistance(Vector3 queryPoint, float distance)
	{

		return m_Spline.GetPointAtLinearDistance(ClosestT(queryPoint), distance, out distance);
	}

}

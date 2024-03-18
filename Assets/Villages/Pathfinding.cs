using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

public class Pathfinding : MonoBehaviour
{
	public Transform villager;
	public Transform target;

	Grid grid;

	private void Awake()
	{
		grid = GetComponent<Grid>();
	}

    private void Update()
    {
		FindPath(villager.position, target.position);
    }

    void FindPath(Vector3 startPos, Vector3 targetPos)
	{

		Stopwatch sw = new Stopwatch();
		sw.Start();
		Node startNode = grid.GetNodeFromWorldPoint(startPos);
		Node targetNode = grid.GetNodeFromWorldPoint(targetPos);

		Heap<Node> openSet = new Heap<Node>(grid.MaxSize);
		HashSet<Node> closedSet = new HashSet<Node>();

		openSet.Add(startNode);

		while(openSet.Count > 0)
		{
			Node currentNode = openSet.RemoveFirst();
			/*for (int thisNodeIndex = 1; thisNodeIndex < openSet.Count; thisNodeIndex++)
			{
				if (openSet[thisNodeIndex].fCost < currentNode.fCost 
					|| (openSet[thisNodeIndex].fCost == currentNode.fCost && openSet[thisNodeIndex].hCost < currentNode.hCost))
				{
					currentNode = openSet[thisNodeIndex];
				}
			}

			openSet.Remove(currentNode);*/
			closedSet.Add(currentNode);

			if (currentNode == targetNode)
			{
				sw.Stop();
				print($"Path Found: {sw.ElapsedMilliseconds}ms");
				RetracePath(startNode, targetNode);
                return;
			}

			foreach(Node neighbor in grid.GetNeighbors(currentNode))
			{
				if (!neighbor.walkable || closedSet.Contains(neighbor))
				{
					continue;
				}

				int newMovementCostToNeighbor = currentNode.gCost + GetDistance(currentNode, neighbor);
				if (newMovementCostToNeighbor < neighbor.gCost || !openSet.Contains(neighbor))
				{
					neighbor.gCost = newMovementCostToNeighbor;
					neighbor.hCost = GetDistance(neighbor, targetNode);
					neighbor.parent = currentNode;
					if (!openSet.Contains(neighbor))
						openSet.Add(neighbor);
				}
			}
		}
	}

	void RetracePath(Node startNode, Node endNode)
	{
		List<Node> path = new List<Node>();
		Node currentNode = endNode;

		while (currentNode != startNode)
		{
			path.Add(currentNode);
			currentNode = currentNode.parent;
		}

		path.Reverse();

		grid.path = path;
	}

	int GetDistance(Node nodeA, Node nodeB)
	{
		int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
		int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

		if (dstX > dstY) return 14 * dstY + 10 * (dstX - dstY);
		else return 14 * dstX + 10 * (dstY - dstX);
	}
}

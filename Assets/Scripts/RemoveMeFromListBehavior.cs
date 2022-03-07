using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveMeFromListBehavior : MonoBehaviour
{
	public void RemoveMeFromAllYeetLists()
	{
		//for every yeetTrigger we're being tracked by, remove ourselves from their list.
		for (int k = 0; k < References.allYeetTriggers.Count; k++)
		{
			if (References.allYeetTriggers[k].currentColliders.Contains(gameObject))
				References.allYeetTriggers[k].currentColliders.Remove(gameObject);
		}
	}
}

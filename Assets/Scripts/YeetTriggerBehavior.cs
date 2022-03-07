using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YeetTriggerBehavior : MonoBehaviour
{

    public List<GameObject> currentColliders = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
		References.allYeetTriggers.Add(gameObject.GetComponent<YeetTriggerBehavior>());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Rigidbody>() != null)
        {
            currentColliders.Capacity++;
            currentColliders.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<Rigidbody>() != null)
        {
            currentColliders.Remove(other.gameObject);
            currentColliders.Capacity--;
        }
    }
}

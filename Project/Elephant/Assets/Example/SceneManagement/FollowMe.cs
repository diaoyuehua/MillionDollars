using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FollowMe : MonoBehaviour {

    public Transform me = null;
    private NavMeshPath path = null;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyUp(KeyCode.F))
        {
            NavMeshAgent agent = GetComponent<NavMeshAgent>();
            path = new NavMeshPath();
            agent.CalculatePath(me.position, path);
        }
        if(path != null)
        {
            for(int i = 0; i < path.corners.Length-1; ++i)
            {
                Debug.DrawLine(path.corners[i], path.corners[i + 1]);
            }
        }
	}
}

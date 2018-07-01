using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class PlayTimeline : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("OnCollisionEnter" + other.gameObject.name);
        if(other.gameObject.name == "RPG-Character")
        {
            PlayableDirector pd = GetComponent<PlayableDirector>();
            if(pd.state == PlayState.Paused)
            {
                other.gameObject.GetComponent<Move>().canMove = false;
                pd.Play();
                System.Action<PlayableDirector> callback = (director) =>
                {
                    other.gameObject.GetComponent<Move>().canMove = true;
                };
                pd.stopped += callback;
            }
        }
    }
}

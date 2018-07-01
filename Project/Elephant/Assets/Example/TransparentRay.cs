using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransparentRay : MonoBehaviour {

    public GameObject role = null;

    private Dictionary<GameObject, float> transparentObjects = new Dictionary<GameObject, float>();

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 pos = role.transform.position;
        Vector3 dir = (pos - transform.position).normalized;
        float dis = Vector3.Distance(transform.position, pos);
        Ray ray = new Ray(transform.position, dir);
        RaycastHit[] hits = Physics.RaycastAll(ray, dis, 1 << 8);

        Debug.DrawRay(transform.position, dir * dis, Color.green);

        HashSet<GameObject> hitGameObjects = new HashSet<GameObject>();
        foreach(RaycastHit hit in hits)
        {
            hitGameObjects.Add(hit.collider.gameObject);
        }
        List<GameObject> dels = new List<GameObject>();
        foreach (GameObject go in transparentObjects.Keys)
        {
            if (!hitGameObjects.Contains(go))
            {
                dels.Add(go);
            }
            else
            {
                hitGameObjects.Remove(go);
            }
        }
        foreach(GameObject del in dels)
        {
            Renderer renderer = del.GetComponent<Renderer>();
            if (renderer)
            {
                renderer.material.SetFloat("_Alpha", 1);
            }
            transparentObjects.Remove(del);
        }
        foreach (GameObject add in hitGameObjects)
        {
            transparentObjects.Add(add, 1);
        }
    }

    private void FixedUpdate()
    {
        UpdateTransparent();
    }

    void UpdateTransparent()
    {
        foreach(GameObject go in transparentObjects.Keys)
        {
            Renderer renderer = go.GetComponent<Renderer>();
            if (renderer)
            {
                if(transparentObjects[go] > 0)
                {
                    renderer.material.SetFloat("_Alpha", transparentObjects[go] - 0.05f);
                    transparentObjects[go] = transparentObjects[go] - 0.05f;
                }
            }
        }
    }
}

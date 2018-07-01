using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Move : MonoBehaviour {

    private const int SideLength = 32;
    private const int TileNumber = 8;

    float inputHorizontal = 0f;
    float inputVertical = 0f;
    bool inputShift = false;

    public GameObject roleVirtualCamera = null;
    public float speed = 1f;

    Animator animator = null;

    bool isMoving = false;

    public bool canMove = true;

    GameObject terrainRoot;

    private Dictionary<string, GameObject> loadedTile = new Dictionary<string, GameObject>();

    // Use this for initialization
    void Start () {
        animator = GetComponent<Animator>();

        terrainRoot = GameObject.Find("Terrain");
        int childrenCount = terrainRoot.transform.childCount;
        for (int i = childrenCount - 1; i >= 0; --i)
        {
            terrainRoot.transform.GetChild(i).gameObject.SetActive(false);
        }

        float hight = GetHeight(transform.position);
        Vector3 newPos = transform.position;
        newPos.y = hight;
        transform.position = newPos;

        LoadTile(transform.position);

    }
	
    void LoadTile(Vector3 pos)
    {
        int xi = Mathf.Clamp((int)(pos.x / SideLength), 0, TileNumber - 1);
        int yi = Mathf.Clamp((int)(pos.z / SideLength), 0, TileNumber - 1);

        List<string> removeList = new List<string>();
        foreach(string key in loadedTile.Keys)
        {
            string[] xy = key.Split('_');
            int x = int.Parse(xy[0]);
            int y = int.Parse(xy[1]);
            if(Mathf.Abs(x - xi) > 1 || Mathf.Abs(y - yi) > 1)
            {
                removeList.Add(key);
            }
        }

        foreach (string key in removeList)
        {
            loadedTile[key].SetActive(false);
            loadedTile.Remove(key);
        }

        for (int i = xi - 1; i <= xi + 1; ++i)
        {
            if (i < 0 || i > TileNumber - 1) continue;
            for (int j = yi - 1; j <= yi + 1; ++j)
            {
                if (j < 0 || j > TileNumber - 1) continue;

                if (!loadedTile.ContainsKey(i + "_" + j))
                {
                    Transform tile = terrainRoot.transform.Find(i + "_" + j);
                    if(tile != null)
                    {
                        tile.gameObject.SetActive(true);
                        loadedTile.Add(tile.name, tile.gameObject);
                    }
                }
            }
        }
    }

    float GetHeight(Vector3 pos)
    {
        int xi = Mathf.Clamp((int)(pos.x / SideLength), 0, TileNumber - 1);
        int yi = Mathf.Clamp((int)(pos.z / SideLength), 0, TileNumber - 1);
        Transform tile = terrainRoot.transform.Find(xi + "_" + yi);
        if(tile != null)
        {
            return tile.GetComponent<Terrain>().SampleHeight(pos);
        }
        else
        {
            return pos.y;
        }
        
    }

    public void LockMove(bool canMove)
    {
        if (canMove)
        {
            this.canMove = true;
        }
        else
        {
            this.canMove = false;
            animator.SetBool("Move", false);
        }
    }

	// Update is called once per frame
	void Update () {
        HandleInput();
        if (!canMove) return;
        if(Mathf.Abs(inputVertical) > 0.001f || Mathf.Abs(inputHorizontal) > 0.001f)
        {
            isMoving = true;
            float y = inputVertical > 0.001f ? 0.5f : 0f;
            float x = inputVertical > 0.001f ? 0.5f : 0f;
            y = inputShift ? y * 2 : y;
            x = inputShift ? x * 2 : x;
            animator.SetFloat("Move Y", y);
            animator.SetFloat("Move X", x);
            animator.SetBool("Move", true);
        }
        else
        {
            isMoving = false;
            animator.SetBool("Move", false);
        }

        if (isMoving)
        {
            Vector3 pos = transform.position;
            if (Mathf.Abs(inputVertical) > 0.001f)
            {
                float v = inputVertical > 0 ? speed : -speed;
                v = inputShift ? v : v * 0.5f;
                pos.z += v;
            }
            if (Mathf.Abs(inputHorizontal) > 0.001f)
            {
                float v = inputHorizontal > 0 ? speed : -speed;
                v = inputShift ? v : v * 0.5f;
                pos.x += v;
            }
            float hight = GetHeight(pos);
            if(hight-pos.y < 0.1)
            {
                pos.y = hight;
                transform.position = pos;
                LoadTile(transform.position);
            }

        }

        if(roleVirtualCamera != null)
        {
            CinemachineVirtualCamera cmp = roleVirtualCamera.GetComponent<CinemachineVirtualCamera>();
            if(Input.GetKey(KeyCode.Q))
            {
                CinemachineOrbitalTransposer orbitalTransposer = cmp.GetCinemachineComponent(CinemachineCore.Stage.Body) as CinemachineOrbitalTransposer;
                if (orbitalTransposer)
                {
                    orbitalTransposer.m_XAxis.m_InputAxisValue = 1;
                }
            }
            else if (Input.GetKey(KeyCode.E))
            {
                CinemachineOrbitalTransposer orbitalTransposer = cmp.GetCinemachineComponent(CinemachineCore.Stage.Body) as CinemachineOrbitalTransposer;
                if (orbitalTransposer)
                {
                    orbitalTransposer.m_XAxis.m_InputAxisValue = -1;
                }
            }
            else
            {
                CinemachineOrbitalTransposer orbitalTransposer = cmp.GetCinemachineComponent(CinemachineCore.Stage.Body) as CinemachineOrbitalTransposer;
                if (orbitalTransposer)
                {
                    orbitalTransposer.m_XAxis.m_InputAxisValue = 0;
                }
            }
        }
    }

    void HandleInput()
    {
        inputHorizontal = Input.GetAxisRaw("Horizontal");
        inputVertical = Input.GetAxisRaw("Vertical");
        inputShift = Input.GetButton("Shift");
    }

    public void Hit()
    {
    }

    public void Shoot()
    {
    }

    public void FootR()
    {
    }

    public void FootL()
    {
    }

    public void Land()
    {
    }

    public void Jump()
    {
    }
}

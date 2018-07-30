using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public PlayerRole playerRole;

    private float inputHorizontal = 0;
    private float inputVertical = 0;
    private bool inputShift = false;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (playerRole != null)
        {
            float _inputHorizontal = Input.GetAxisRaw("Horizontal");
            float _inputVertical = Input.GetAxisRaw("Vertical");
            bool _inputShift = Input.GetButton("Shift");

            if(inputHorizontal != _inputHorizontal || inputVertical != _inputVertical || inputShift != _inputShift)
            {
                inputHorizontal = _inputHorizontal;
                inputVertical = _inputVertical;
                inputShift = _inputShift;
                MyLogger.Debug("x:{0},y:{1},s:{2}", inputHorizontal, inputVertical, inputShift);
                if(inputHorizontal != 0 || inputVertical != 0)
                {
                    if (_inputShift)
                    {
                        playerRole.Command(RoleCommand.Run, new Vector2(inputHorizontal, inputVertical));
                    }
                    else
                    {
                        playerRole.Command(RoleCommand.Walk, new Vector2(inputHorizontal, inputVertical));
                    }
                }
                else
                {
                    playerRole.Command(RoleCommand.Stand);
                }
            }
        }
	}
}

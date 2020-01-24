using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputHandler : MonoBehaviour
{
	//Detect mouse  input and stuff.
	//Responds with UI inputs and sends data outward mainly the controller

	CharacterController controller;

	private void Start()
	{
		controller = GameManager.Instance.GetCharacterControl();
	}

	// Update is called once per frame
	void Update()
    {
		//check for mouse input
		mouseInput();
    }

	void mouseInput()
	{
		if (Input.GetMouseButtonDown(0))
		{
			//get mouse position
			//extract the tile
			//send player controller the tile
			//player controller should check if a unit is on that tile also check if interacting with interactible in a interactible manager

			//if is unit -> player controller now selects that unit
			//if is interactible-> do interactible script

			Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			Vector2Int tilePos = new Vector2Int(Mathf.FloorToInt(mousePos.x), Mathf.FloorToInt(mousePos.y));
			TileNode tile = GameManager.Instance.getTile(tilePos);
			if(tile != null)
			{
				//send to player controller
				controller.selectUnit (tile);
				//send to interactible manager
			}
		}

		if(Input.GetMouseButtonDown(1))
		{
			//mainly is for moving a unit -> send the tile of the target and send that information to the player controller
			Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			Vector2Int tilePos = new Vector2Int(Mathf.FloorToInt(mousePos.x), Mathf.FloorToInt(mousePos.y));
			TileNode tile = GameManager.Instance.getTile(tilePos);
			if(tile != null)
			{
				//send to player controller
				controller.moveUnit(tile);
			}
		}
	}
}

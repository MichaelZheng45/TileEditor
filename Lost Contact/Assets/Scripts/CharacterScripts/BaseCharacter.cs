using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseCharacter : MonoBehaviour
{
	TileNode currentTile; //the tile this unit is on
	List<TileNode> path;
	int tilePathIndex;
	bool isMoving = false;
	float moveLerpParameter;	

	[Range(.1f, .3f)]
	public float moveSpeed;

	void moveToTile(TileNode moveTo)
	{
		if(Pathfinder.moveTo(ref path, currentTile.index, moveTo.index))
		{
			isMoving = true;
			tilePathIndex = 0;
			moveLerpParameter = 0;
		}		
	}

	void updateMoving()
	{
		//follow the path given
		if(isMoving)
		{
			TileNode fromTile = path[tilePathIndex];
			TileNode nextTile = path[tilePathIndex + 1];

			Vector2 position = Vector2.Lerp(fromTile.index, nextTile.index, moveLerpParameter);
			gameObject.transform.position = position;			

			moveLerpParameter += moveSpeed;

			if(moveLerpParameter == 1)
			{
				moveLerpParameter = 0;
				tilePathIndex++;
			}
			else if(moveLerpParameter > 1)
			{
				moveLerpParameter = 1;
			}

			if (tilePathIndex >= path.Count)
			{
				isMoving = false;
			}
		}
	}
}

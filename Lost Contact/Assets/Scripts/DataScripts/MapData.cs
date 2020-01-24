using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileTypes
{
	TILE_FLOOR,
	TILE_WALL
}

public class MapData : MonoBehaviour
{
	//colums (X)
	public NodeList[] tileList;

	int lengthX;
	int lengthY;

	public void setLength(int xLength, int yLength)
	{
		lengthX = xLength;
		lengthY = yLength;
	}

	public TileNode getTile(Vector2Int index)
	{
		if(index.x < lengthX && index.x >= 0 && index.y < lengthY && index.y >=0 )
		{
			return tileList[index.x].rows[index.y];
		}
		return null;
	}
}

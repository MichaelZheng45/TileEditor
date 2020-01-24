using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NodeList
{
	public TileNode[] rows;
}

[System.Serializable]
public class TileNode
{
	// current index x and y
	public Vector2Int index;
	//tileType 
	public TileTypes thisType;

	public GameObject tileObj;

	// connections
	public NodeList[] adjTiles;

	//pathfinding data
	public float costSoFar;
	public float costWithHeuristic; //this is the cost so far and combination of distance from target

	public TileNode previousNode;
}

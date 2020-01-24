using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Pathfinder 
{
	//need to make a event system
	public static bool moveTo(ref List<TileNode> pathList, Vector2Int from, Vector2Int to)
	{
		MapData mapData = GameManager.Instance.mapData;
		List<TileNode> openList = new List<TileNode>();
		HashSet<TileNode> closedList = new HashSet<TileNode>();

		//create new path
		pathList = new List<TileNode>();

		int openListCount = 0;
		int tilesProcessed = 0; // tiles in the closed list

		//collect the first node 
		TileNode firstNode = mapData.getTile(from);

		//collect target node and do basic checks
		TileNode targetNode = mapData.getTile(to);
		if (targetNode == null || targetNode == firstNode || targetNode.thisType == TileTypes.TILE_WALL)
			return false;

		//set the current cost so far and how far it is from target
		firstNode.costSoFar = 0;
		firstNode.costWithHeuristic = (to - from).magnitude;

		//add node
		openList.Add(firstNode);
		openListCount++;

		bool foundToTile = false;

		//continues while the open list is not empty and target tile is not found
		while(openListCount > 0 && !foundToTile) 
		{
			TileNode curNode = openList[0];
			Vector2Int index = curNode.index;

			//look at the adjacent tiles of current node
			for (int i = 0; i < 3; i++)
			{
				for(int j = 0; j < 3; j++)
				{
					//get adj tile
					TileNode adjTile = curNode.adjTiles[i].rows[j];

					//for each tile, check if it is target tile
					if(adjTile == targetNode)
					{
						foundToTile = true;
					}

					//check if it can be passed to,
					if(adjTile.thisType == TileTypes.TILE_FLOOR)
					{
						// if so then insert to open list with new data
						adjTile.costSoFar = curNode.costSoFar + (adjTile.index - index).magnitude;
						adjTile.costWithHeuristic = adjTile.costSoFar + (targetNode.index - adjTile.index).magnitude;
						adjTile.previousNode = curNode;

						//hop through the list until the heuristic cost is in the right order
						bool placed = false;
						for (int k = 0; k < openListCount; k++)
						{
							if (openList[k].costWithHeuristic > adjTile.costWithHeuristic)
							{
								placed = true;
								openList.Insert(k, adjTile);
								openListCount++;
								k = openListCount;
							}
						}

						//in case if it doesn't fit, place in the back
						if (placed == false)
						{
							openList.Insert(openListCount, adjTile);
							openListCount++;
						}
					}	
				}
			}

			//node is finished processing 
			//add to closed list 
			tilesProcessed++;
			closedList.Add(curNode);

			//and remove from open list
			openList.RemoveAt(0);
			openListCount--;
		}

		//if the tile is found then create path other wise return false
		if(foundToTile)
		{
			pathList.Add(targetNode);
			bool tracedPath = false;
			TileNode curTile = targetNode;

			while(!tracedPath)
			{
				curTile = curTile.previousNode;
				pathList.Add(curTile);
				if(curTile == firstNode)
				{
					tracedPath = true;
				}
			}
			return true;
		}

		return false;
	}

}

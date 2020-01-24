using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

//Really got to understand the concepts of some of the functions from here
//https://www.synnaxium.com/en/2019/01/unity-custom-map-editor-part-1/
//https://answers.unity.com/questions/1260602/how-to-get-mouse-click-world-position-in-the-scene.html
//https://www.quora.com/How-can-a-4-dimensional-array-be-visualised-I-e-a-2-dimensional-array-can-be-visualised-as-a-sequence-of-matrix
public class MapEditor : EditorWindow
{
	bool editorMode = false;
	bool removeMode = false;
	GameManager gm;

	Sprite currentTileSprite;
	TileTypes tileType;

	NodeList[] tileList;

	[MenuItem("Window/MapEditor")]
	public static void ShowWindow()
	{
		EditorWindow.GetWindow(typeof(MapEditor));
	}

	public void Update()
	{
		if(gm == null)
		{
			gm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
		}

	
	}

	public void OnGUI()
	{
		editorMode = GUILayout.Toggle(editorMode, "EditorMode: " + editorMode);
		if(editorMode)
		{
			removeMode = GUILayout.Toggle(removeMode, "Erase Mode : " + removeMode);
		}

		GUILayout.Space(25);

		GUILayout.Label("Tile Tools", EditorStyles.boldLabel);
		currentTileSprite = EditorGUILayout.ObjectField("Tile Sprite", currentTileSprite, typeof(Sprite), true) as Sprite;
		tileType = (TileTypes)EditorGUILayout.EnumPopup("Tile Type", tileType);

		if(GUILayout.Button("Update Map"))
		{
			//redo all connections and all
			//take the current tile list
			tileList = gm.mapData.tileList;
			int xLength = tileList.Length;
			int yLength = tileList[0].rows.Length;

			for (int indX = 0; indX < xLength; indX++)
			{
				for (int indY = 0; indY < yLength; indY++) 
				{
					TileNode node = tileList[indX].rows[indY];
					if (node != null)
					{
						node.adjTiles = createNewList(3, 3);
						//if the current index is at edge, zero is -1, 1 is 0, 2 is 1
						// 0,2  1,2  2,2
						// 0,1  1,1  2,1
						// 0,0  1,0  2,0

						//bottom left
						if (indX != 0 && indY != 0)
						{
							node.adjTiles[0].rows[0] = tileList[indX - 1].rows[ indY - 1];
						}

						//bottom mid
						if (indY != 0)
						{
							node.adjTiles[1].rows[0] = tileList[indX].rows[ indY - 1];
						}

						//bottom right
						if (indY != 0 && indX != xLength - 1)
						{
							node.adjTiles[2].rows[0] = tileList[indX + 1].rows[indY - 1];
						}

						//mid right
						if (indX != xLength - 1)
						{
							node.adjTiles[2].rows[1] = tileList[indX + 1].rows[indY];
						}

						//top right
						if (indX != xLength - 1 && indY != yLength - 1)
						{
							node.adjTiles[2].rows[2] = tileList[indX + 1].rows[indY + 1];
						}

						//top mid
						if (indY != yLength - 1)
						{
							node.adjTiles[1].rows[2] = tileList[indX].rows[indY + 1];
						}

						//top left
						if (indY != yLength - 1 && indX != 0)
						{
							node.adjTiles[0].rows[2] = tileList[indX - 1].rows[indY + 1];
						}

						//mid left
						if (indX != 0)
						{
							node.adjTiles[0].rows[1] = tileList[indX - 1].rows[indY];
						}
					}
				}
			}
			gm.mapData.tileList = tileList;
		}

		if(GUILayout.Button("Remove All"))
		{
			gm.mapData.tileList = null;
		}

		if(gm.mapData.tileList != null)
		{
			for (int i = 0; i < gm.mapData.tileList[0].rows.Length; i++)
			{
				for (int j = 0; j < gm.mapData.tileList.Length; j++)
				{
					if (gm.mapData.tileList[j].rows[i] != null)
					{
						GUILayout.Label(j + "," + i, EditorStyles.miniLabel);
					}

				}

			}
		}

	}

	private void OnSceneGUI(SceneView sceneView)
	{

		if(editorMode)
		{
			//draw tileoutline
			drawOutLine();
		}

		Event evt = Event.current;

		switch (evt.type)
		{
			case EventType.MouseDown:
				{
					if(editorMode && evt.button == 0)
					{
						if(removeMode)
						{
							//option to remove tiles as well <---------------------------------------
							//when removing, check if last collum or row has nothing if so then shrink the list
							removeTile();
						}
						else
						{
							drawTile();
						}
						
					}
		
					break;
				}
			case EventType.KeyUp:
				{
				
					if (evt.keyCode == (KeyCode.Space))
					{

					}
					break;
				}
		}
	}

	void removeTile()
	{
		Ray guiRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
		Vector3 mousePosition = guiRay.origin - guiRay.direction * (guiRay.origin.z / guiRay.direction.z);

		Vector2Int cell = new Vector2Int(Mathf.FloorToInt(mousePosition.x), Mathf.FloorToInt(mousePosition.y));

		tileList = gm.mapData.tileList;

		//make sure it is greater than 0
		if (cell.x >= 0 && cell.y >= 0)
		{
			if (tileList[cell.x].rows[cell.y] != null)
			{
				DestroyImmediate(tileList[cell.x].rows[cell.y].tileObj);
				tileList[cell.x].rows[cell.y] = null;
			}
		}
	}

	void drawTile()
	{
		Ray guiRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
		Vector3 mousePosition = guiRay.origin - guiRay.direction * (guiRay.origin.z / guiRay.direction.z);

		Vector2Int cell = new Vector2Int(Mathf.FloorToInt(mousePosition.x), Mathf.FloorToInt(mousePosition.y));

		tileList = gm.mapData.tileList;

		//make sure it is greater than 0
		if(cell.x >=0 && cell.y >= 0)
		{
			//generate tile
			Vector2 tilePosition = cell + new Vector2(.5f, .5f);
			GameObject prefab = Resources.Load<GameObject>("Prefabs/Tile");
			GameObject newObj = Instantiate(prefab, tilePosition, Quaternion.identity, gm.gameObject.transform);

			//add sprites and change name
			newObj.GetComponent<SpriteRenderer>().sprite = currentTileSprite;
			newObj.name = cell.x + "," + cell.y;

			//create tile node
			TileNode newNode = new TileNode();
			newNode.index = cell;
			newNode.thisType = tileType;
			newNode.tileObj = newObj;

			//add to list
			if (tileList == null)
			{
				tileList = createNewList(cell.x +1, cell.y+1);
				tileList[cell.x].rows[cell.y] = newNode;		
			}
			else
			{
				//if new node is greater than tilelist then create new one
				NodeList[] tempTileList = null;
				int xLength = tileList.Length;
				int yLength = tileList[0].rows.Length;
				if(cell.x >= xLength)
				{
					xLength = cell.x +1;
					tempTileList = createNewList(xLength, yLength);
				}
				
				if(cell.y >= yLength)
				{
					yLength = cell.y + 1;
					tempTileList = createNewList(xLength, yLength);
				}

				//if the tempTileList is not null we know it is updated so we need to transfer data if not then just ignore
				if(tempTileList != null)
				{
					for(int i = 0; i < tileList.Length; i++)
					{
						for(int j = 0; j < tileList[0].rows.Length; j++)
						{
							tempTileList[i].rows[j] = tileList[i].rows[j];
						}
					}
					tileList = tempTileList;
				}
				//checks if there exists a tile already, if so remove that old tile
				if (tileList[cell.x].rows[cell.y] != null)
				{
					DestroyImmediate(tileList[cell.x].rows[cell.y].tileObj);
				}

				tileList[cell.x].rows[cell.y] = newNode;
			}

			gm.mapData.tileList = tileList;
			gm.mapData.setLength(tileList.Length, tileList[0].rows.Length);
		}
	}

	void drawOutLine()
	{
		//get the mouse position in world space
		Ray guiRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
		Vector3 mousePosition = guiRay.origin - guiRay.direction * (guiRay.origin.z / guiRay.direction.z);

		Vector2Int cell = new Vector2Int(Mathf.FloorToInt(mousePosition.x), Mathf.FloorToInt(mousePosition.y));

		// Vertices of our square
		Vector2 topLeft = cell + new Vector2(0, 1);
		Vector2 topRight = cell  + new Vector2(1, 1);
		Vector2 bottomLeft = cell;
		Vector2 bottomRight = cell + new Vector2(1,0);

		if (removeMode)
		{
			Handles.color = Color.red;
		}
		else
		{
			Handles.color = Color.green;
		}
		Vector3[] lines = { topLeft, topRight, topRight, bottomRight, bottomRight, bottomLeft, bottomLeft, topLeft };
		Handles.DrawLines(lines);
	}

	NodeList[] createNewList(int xLength, int yLength)
	{
		NodeList[] tempList = new NodeList[xLength];
		for (int i = 0; i < xLength; i++)
		{
			tempList[i].rows = new TileNode[yLength];
		}

		return tempList;
	}

	void OnFocus()
	{
		SceneView.onSceneGUIDelegate -= this.OnSceneGUI; 
		SceneView.onSceneGUIDelegate += this.OnSceneGUI;
	}

	void OnDestroy()
	{
		SceneView.onSceneGUIDelegate -= this.OnSceneGUI;
	}
}

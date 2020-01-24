using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

	private static GameManager _instance;

	public static GameManager Instance { get { return _instance; } }

	private void Awake()
	{
		_instance = this;
	}

	public MapData mapData;
	public GameObject[,] objTileMap;
	public CharacterController controller;

	// Start is called before the first frame update
	void Start()
    {        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public CharacterController GetCharacterControl()
	{
		return controller;
	}

	public TileNode getTile(Vector2Int index)
	{
		return mapData.getTile(index);
	}
}

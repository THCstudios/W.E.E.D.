using UnityEngine;
using System.Collections.Generic;
using System.Threading;

public class Building : MonoBehaviour {
	
	public Level Level;
	public List<Tile> Tiles;
	public List<Vector2> TileCoords;
	public bool isPlaced = true;
	public Tile Root {
		get {
			if (Level.tiles == null) {
				Level.GenerateTiles (Level.Terrain.terrainData);
			}
			return Level.tiles[(int)transform.position.x, (int)transform.position.z];
		}
	}
	public int Width;
	public int Height;

	void Awake() {
		Level = GameObject.FindGameObjectWithTag("GameController").GetComponent<Level> ();
		Tiles = new List<Tile> ();

	}
	// Use this for initialization
	void Start () {
		transform.position = new Vector3 (Root.pos.x, transform.position.y, Root.pos.y);
		if(isPlaced) {
			Init();
		}
	}

	public void Init() {

		Tiles = GetCurrentTiles ();

	}
	// Update is called once per frame
	void Update () {
	
	}
	public List<Tile> GetCurrentTiles() {
		List<Tile> tiles = new List<Tile>();
		TileCoords = new List<Vector2> ();
		
		int rootX = (int)Root.pos.x;
		int rootY = (int)Root.pos.y;
		for (int i = 0; i < Width; i++) {
			for (int j = 0; j < Height; j++) {
				tiles.Add (Level.tiles[rootX + i, rootY + j]);
				TileCoords.Add (new Vector2 (rootX + i, rootY + j));
			}
		}
		for (int i = 0; i > Width; i--) {
			for (int j = 0; j > Height; j--) {
				tiles.Add (Level.tiles[rootX + i, rootY + j]);
				TileCoords.Add (new Vector2 (rootX + i, rootY + j));
			}
		}
		return tiles;
	}
}

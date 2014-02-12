using UnityEngine;
using System.Collections.Generic;
using System.Threading;

public class Building : MonoBehaviour {
	
	public Level Level;
	public List<Tile> Tiles;
	public List<Vector2> TileCoords;
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

	// Use this for initialization
	void Start () {
		Tiles = new List<Tile>();

		int rootX = (int)Root.pos.x;
		int rootY = (int)Root.pos.y;
		for (int i = 0; i < Width; i++) {
			for (int j = 0; j < Height; j++) {
				Tiles.Add (Level.tiles[rootX + i, rootY + j]);
				TileCoords.Add (new Vector2 (rootX + i, rootY + j));
			}
		}
		for (int i = 0; i > Width; i--) {
			for (int j = 0; j > Height; j--) {
				Tiles.Add (Level.tiles[rootX + i, rootY + j]);
				TileCoords.Add (new Vector2 (rootX + i, rootY + j));
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

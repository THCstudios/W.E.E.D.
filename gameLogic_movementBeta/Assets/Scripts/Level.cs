using UnityEngine;
using System.Collections.Generic;

public class Level : MonoBehaviour
{
	public Terrain Terrain;
	public Tile[,] tiles;
	public CalculationTileMap calculationTiles;

	void Start() {
		if (tiles == null) {
			GenerateTiles (Terrain.terrainData);
		}
		calculationTiles = new CalculationTileMap(tiles);
	}

	public void GenerateTiles (TerrainData data) {
		tiles = new Tile[data.heightmapWidth + 1, data.heightmapHeight + 1];
		for (int i = 0; i < tiles.GetLength (0); i++) {
			for (int j = 0; j < tiles.GetLength (1); j++) {
				Tile currentTile = tiles[i, j] = new Tile ();
				currentTile.level = this;
				currentTile.pos.x = i;
				currentTile.pos.y = j;
			}
		}
	}
	public void UpdateCalculationTiles(List<Tile> tiles, bool isOccupied) {
		foreach (Tile t in tiles) {
			calculationTiles.Tiles[(int)t.pos.x,(int)t.pos.y].IsOccupied = isOccupied;
		}
	}
}


using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Text;

public class CalculationTileMap {
	private CalculationTile[,] tiles;

	public CalculationTile[,] Tiles {
		get { return tiles; }
		set { tiles = value; }
	}

	public CalculationTileMap (Tile[,] tiles){
		this.tiles = new CalculationTile[tiles.GetLength(0),tiles.GetLength(1)];
		for (int i = 0; i < tiles.GetLength(0); i++) {
			for (int j = 0; j < tiles.GetLength(1); j++) {
				this.tiles[i, j] = new CalculationTile(tiles[i, j].IsOccupied, tiles[i, j].pos.x, tiles[i, j].pos.y);
			}
		}
	}
	public List<CalculationTile> Neighbors (CalculationTile c) {
		List<CalculationTile> neighbors = new List<CalculationTile>();
		int i = (int) c.Pos.x, j = (int) c.Pos.y;
		bool borderLeft = i == 0, borderTop = j == 0, 
		borderRight = i == tiles.GetLength (0) - 1, 
		borderBottom = j == tiles.GetLength (1) - 1;
		if (!borderLeft) {
			neighbors.Add (tiles[i - 1, j]);
		}
		if (!borderTop) {
			neighbors.Add (tiles[i, j - 1]);
		}
		if (!borderRight) {
			neighbors.Add (tiles[i + 1, j]);
		}
		if (!borderBottom) {
			neighbors.Add (tiles[i, j + 1]);
		}
		return neighbors;
	}
	public float HeuristicCost(CalculationTile src, CalculationTile dst) {
		return Math.Abs((dst.Pos - src.Pos).sqrMagnitude);
	}

	public float NeighborCost(CalculationTile src, CalculationTile neighbor) {
		if (neighbor.IsOccupied) {
			return 3.5f;
		}
		return HeuristicCost(src, neighbor);
	}
}


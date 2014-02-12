using UnityEngine;
using System.Collections.Generic;

public class Tile {
	public Level level;
	
	public List<Tile> Neighbors {
		get {
			List<Tile> neighbors = new List<Tile>();
			int i = (int) pos.x, j = (int) pos.y;
			bool borderLeft = i == 0, borderTop = j == 0, 
			borderRight = i == level.tiles.GetLength (0) - 1, 
			borderBottom = j == level.tiles.GetLength (1) - 1;
			if (!borderLeft) {
				neighbors.Add (level.tiles[i - 1, j]);
				/*if (!borderTop) {
					neighbors.Add (level.tiles[i - 1, j - 1]);
				}
				if (!borderBottom) {
					neighbors.Add (level.tiles[i - 1, j + 1]);
				}*/
			}
			if (!borderTop) {
				neighbors.Add (level.tiles[i, j - 1]);
			}
			if (!borderRight) {
				neighbors.Add (level.tiles[i + 1, j]);
				/*if (!borderTop) {
					neighbors.Add (level.tiles[i + 1, j - 1]);
				}
				if (!borderBottom) {
					neighbors.Add (level.tiles[i + 1, j + 1]);
				}*/
			}
			if (!borderBottom) {
				neighbors.Add (level.tiles[i, j + 1]);
			}
			return neighbors;
		}
	}
	
	public Vector3 WorldPos {
		get {
			return new Vector3 (pos.x, 0.5f, pos.y);
		}
	}
	
	public bool IsOccupied {
		get {
			GameObject[] buildings = GameObject.FindGameObjectsWithTag ("Building");
			// Cache
			//Vector3 minWorldPos = WorldPos;
			//Vector3 maxWorldPos = level.tiles[(int)pos.x + 1, (int)pos.y + 1].WorldPos;
			foreach (GameObject building in buildings) {
				Building b = building.GetComponent<Building>();
				if (b.Tiles.Contains(this)) {
					return true;
				}
				//Debug.Log (building.GetComponent<Collider>().bounds);
			}
			return false;
		}
	}
	
	public Vector2 pos;
	
	public float HeuristicCost (Tile dst) {
		return Mathf.Abs((dst.pos - pos).magnitude);
	}
	
	public float NeighborCost (Tile neighbor) {
		if (neighbor.IsOccupied) {
			return 200;
		}
		return HeuristicCost (neighbor);
	}
}
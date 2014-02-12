using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class PathFinder : MonoBehaviour
{
	private Level level;

	void Start() {
		level = GetComponent<Level>();
	}

		// Update is called once per frame
	void Update ()
	{
		for (int i = 0; i < level.tiles.GetLength (0) + 1; i++) {
			Debug.DrawLine (new Vector3 (i, 0.5f, 0), new Vector3 (i, 0.5f, level.tiles.GetLength (0)), Color.red);
		}
		for (int i = 0; i < level.tiles.GetLength (1) + 1; i++) {
			Debug.DrawLine (new Vector3 (0, 0.5f, i), new Vector3 (level.tiles.GetLength (1), 0.5f, i), Color.red);
		}
	}

	public List<Tile> FindPath (Vector3 src, Vector3 dst) {
		Tile tSrc = level.tiles[(int) src.x, (int) src.z];
		Tile tDst = level.tiles[(int) dst.x, (int) dst.z];
		return FindPath (tSrc, tDst);
	}

	public List<Tile> FindPath (Tile src, Tile dst) {
		HashSet<Tile> closedSet = new HashSet<Tile>();
		HashSet<Tile> openSet = new HashSet<Tile>();
		Dictionary<Tile, Tile> cameFrom = new Dictionary<Tile, Tile>();
		openSet.Add (src);
		Dictionary<Tile, float> gScore = new Dictionary<Tile, float>();
		Dictionary<Tile, float> fScore = new Dictionary<Tile, float>();
		gScore.Add (src, 0);
		fScore.Add (src, src.HeuristicCost (dst));

		int cnt = 0;
		
		while (cnt < 500 && openSet.Count > 0) {
			Tile current = openSet.ElementAt (0);
			for (int i = 1; i < openSet.Count; i++) {
				if (fScore[current] > fScore[openSet.ElementAt (i)]) {
					current = openSet.ElementAt (i);
				}
			}
			if (current == dst) {
				List<Tile> path = new List<Tile>();
				reconstructPath (cameFrom, dst, path);
				return path;
			}
			if (current.IsOccupied) {
				closedSet.Add (current);
				openSet.Remove (current);
				continue;
			}
			openSet.Remove (current);
			closedSet.Add (current);

			List<Tile> neighbors = current.Neighbors;
			foreach (Tile neighbor in neighbors) {
				if (closedSet.Contains (neighbor)) {
					continue;
				}
				float tentativeGScore = gScore[current] + current.NeighborCost (neighbor);
				if (!openSet.Contains (neighbor) || tentativeGScore < gScore[neighbor]) {
					cameFrom[neighbor] = current;
					gScore[neighbor] = tentativeGScore;
					fScore[neighbor] = tentativeGScore + neighbor.HeuristicCost (dst);
					if (!openSet.Contains (neighbor)) {
						openSet.Add (neighbor);
					}
				}
			}
			cnt++;
		}
		Tile min = closedSet.ElementAt (0);
		for (int i = 1; i < closedSet.Count; i++) {
			if (fScore[min] > fScore[closedSet.ElementAt (i)]) {
				min = closedSet.ElementAt (i);
			}
		}
		return reconstructPath (cameFrom, min);
	}

	private List<Tile> reconstructPath (Dictionary<Tile, Tile> cameFrom, Tile current) {
		return reconstructPath (cameFrom, current, new List<Tile>());
	}
	
	private List<Tile> reconstructPath (Dictionary<Tile, Tile> cameFrom, Tile current, List<Tile> path) {
		if (cameFrom.ContainsKey (current)) {
			reconstructPath (cameFrom, cameFrom[current], path);
			path.Add (current);
		} else {
			path.Add (current);
		}
		return path;
	}
}


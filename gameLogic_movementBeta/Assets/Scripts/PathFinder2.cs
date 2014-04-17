using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


class PathFinder2 {
	public static List<CalculationTile> FindPath(CalculationTileMap tileSet, CalculationTile src, CalculationTile dst) {
		//int start = getTime();
		HashSet<CalculationTile> closedSet = new HashSet<CalculationTile>();
		HashSet<CalculationTile> openSet = new HashSet<CalculationTile>();
		Dictionary<CalculationTile, CalculationTile> cameFrom = new Dictionary<CalculationTile, CalculationTile>();
		openSet.Add(src);
		Dictionary<CalculationTile, float> gScore = new Dictionary<CalculationTile, float>();
		Dictionary<CalculationTile, float> fScore = new Dictionary<CalculationTile, float>();
		gScore.Add(src, 0);
		fScore.Add(src, tileSet.HeuristicCost(src, dst));

		int cnt = 0;

		while (cnt < 500 && openSet.Count > 0) {
			CalculationTile current = openSet.ElementAt(0);
			for (int i = 1; i < openSet.Count; i++) {
				if (fScore[current] > fScore[openSet.ElementAt(i)]) {
					current = openSet.ElementAt(i);
				}
			}
			if (current == dst) {
				List<CalculationTile> path = new List<CalculationTile>();
				reconstructPath(cameFrom, dst, path);
				return path;
			}
			//if (current.IsOccupied) {
			//	closedSet.Add (current);
			//	openSet.Remove (current);
			//	continue;
			//}
			openSet.Remove(current);
			closedSet.Add(current);

			List<CalculationTile> neighbors = tileSet.Neighbors(current);
			foreach (CalculationTile neighbor in neighbors) {
				if (closedSet.Contains(neighbor)) {
					continue;
				}
				float tentativeGScore = gScore[current] + tileSet.NeighborCost(current, neighbor);
				if (!openSet.Contains(neighbor) || tentativeGScore < gScore[neighbor]) {
					cameFrom[neighbor] = current;
					gScore[neighbor] = tentativeGScore;
					fScore[neighbor] = tentativeGScore + tileSet.HeuristicCost(neighbor, dst);
					if (!openSet.Contains(neighbor)) {
						openSet.Add(neighbor);
					}
				}
			}
			cnt++;
		}
		CalculationTile min = closedSet.ElementAt(0);
		for (int i = 1; i < closedSet.Count; i++) {
			if (fScore[min] > fScore[closedSet.ElementAt(i)]) {
				min = closedSet.ElementAt(i);
			}
		}
		var Path = reconstructPath(cameFrom, min);
		return Path;
	}
	private static List<CalculationTile> reconstructPath(Dictionary<CalculationTile, CalculationTile> cameFrom, CalculationTile current) {
		return reconstructPath(cameFrom, current, new List<CalculationTile>());
	}

	private static List<CalculationTile> reconstructPath(Dictionary<CalculationTile, CalculationTile> cameFrom, CalculationTile current, List<CalculationTile> path) {
		if (cameFrom.ContainsKey(current)) {
			reconstructPath(cameFrom, cameFrom[current], path);
			path.Add(current);
		} else {
			path.Add(current);
		}
		return path;
	}
}


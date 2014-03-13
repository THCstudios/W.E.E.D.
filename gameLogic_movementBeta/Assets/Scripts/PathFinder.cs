using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

class PathInfo {
	public Tile src;
	public Tile dst;
	
	public PathInfo(Tile src, Tile dst) {
		this.src = src;
		this.dst = dst;
	}
	
	public override bool Equals(object o) {
		if (o == null || !(o is PathInfo)) {
			return false;
		}
		PathInfo info = o as PathInfo;
		return info.src == src && info.dst == dst;
	}
	
	public static bool operator== (PathInfo o, object o1) {
		return o.Equals (o1);
	}
	
	public static bool operator!= (PathInfo o, object o1) {
		return !(o == o1);
	}
	
	public override int GetHashCode() {
		return src.GetHashCode () ^ dst.GetHashCode ();
	}
}

public class PathFinder : MonoBehaviour
{
	private Level level;
	
	private Dictionary<PathInfo, ProcessingPath> processingPaths = new Dictionary<PathInfo, ProcessingPath>(); 
	
	void Start() {
		level = GetComponent<Level>();
	}
	
	// Update is called once per frame
	/*void Update ()
	{
		for (int i = 0; i < level.tiles.GetLength (0) + 1; i++) {
			Debug.DrawLine (new Vector3 (i, 0.5f, 0), new Vector3 (i, 0.5f, level.tiles.GetLength (0)), Color.red);
		}
		for (int i = 0; i < level.tiles.GetLength (1) + 1; i++) {
			Debug.DrawLine (new Vector3 (0, 0.5f, i), new Vector3 (level.tiles.GetLength (1), 0.5f, i), Color.red);
		}
	}*/
	
	public ProcessingPath FindPath (Vector3 src, Vector3 dst) {
		Tile tSrc = level.tiles[(int) src.x, (int) src.z];
		Tile tDst = level.tiles[(int) dst.x, (int) dst.z];
		return FindPath (tSrc, tDst);
	}
	
	public ProcessingPath FindPath (Tile src, Tile dst) {
		PathInfo info = new PathInfo(src, dst);
		if (processingPaths.ContainsKey (info)) {
			ProcessingPath path;
			if (processingPaths.TryGetValue (info, out path)) {
				if (path.finished) {
					processingPaths.Remove (info);
				}
				return path;
			}
		}
		StartCoroutine (PathRoutine (src, dst));
		return FindPath (src, dst);
	}
	
	private IEnumerator PathRoutine (Tile src, Tile dst) {
		ProcessingPath path = new ProcessingPath();
		processingPaths.Add (new PathInfo(src, dst), path);
		HashSet<Tile> closedSet = new HashSet<Tile>();
		HashSet<Tile> openSet = new HashSet<Tile>();
		Dictionary<Tile, Tile> cameFrom = new Dictionary<Tile, Tile>();
		openSet.Add (src);
		Dictionary<Tile, float> gScore = new Dictionary<Tile, float>();
		Dictionary<Tile, float> fScore = new Dictionary<Tile, float>();
		gScore.Add (src, 0);
		fScore.Add (src, src.HeuristicCost (dst));
		
		int start = now();
		int cnt = 0;
		
		while (cnt < 500 && openSet.Count > 0 && !path.cancelled) {
			Tile current = openSet.ElementAt (0);
			for (int i = 1; i < openSet.Count; i++) {
				if (fScore[current] > fScore[openSet.ElementAt (i)]) {
					current = openSet.ElementAt (i);
				}
			}
			if (current == dst) {
				path.path = reconstructPath (cameFrom, current);
				path.finished = true;
				Debug.Log ("SUCCESS");
				Debug.Log (now () - start);
				return true;
			}
			openSet.Remove (current);
			closedSet.Add (current);
			
			if (now () - start >= 10) {
				path.path = reconstructPath (cameFrom, current);
				start = now ();
				Debug.Log (now () - start);
				yield return null;
			}
			
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
		path.path = reconstructPath (cameFrom, min);
		path.finished = true;
		Debug.Log ("GAVE UP");
		Debug.Log (now () - start);
		return true;
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
	
	private int now() {
		return DateTime.Now.Millisecond;
	}
}


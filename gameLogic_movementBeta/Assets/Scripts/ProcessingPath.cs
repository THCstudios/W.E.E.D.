using System.Collections.Generic;

public class ProcessingPath {
	public List<Tile> path = new List<Tile>();
	public bool finished;
	public bool cancelled;

	public Tile this[int index] {
		get {
			return path[index];
		}

		set {
			path[index] = value;
		}
	}

	public int Count {
		get {
			return path.Count;
		}
	}
}
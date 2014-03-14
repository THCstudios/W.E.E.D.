using UnityEngine;
using System.Collections.Generic;
using System.Threading;

public class Building : MonoBehaviour {
	
	public Level Level;
	public List<Tile> Tiles;
	public List<Vector2> TileCoords;
	public bool isPlaced;
	public bool isSelected;
	public List<GameObject> BuildingQueue;
	public GameObject CubePrefab;

	public bool IsPlaced {
		set {
			isPlaced = value;
		}
		get {
			return isPlaced;
		}
	}

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
	private Dictionary<string,string> constructables;

	public Dictionary<string,string> Constructables { get; set; }


	public bool IsSelected {
		set {
			isSelected = value;
			if(isSelected) {
				GetComponentInChildren<Projector>().enabled = true;
			} else {
				GetComponentInChildren<Projector>().enabled = false;
			}
		}
		get {
			return isSelected;
		}
	}

	void Awake () {
		Level = GameObject.FindGameObjectWithTag("GameController").GetComponent<Level> ();
		IsPlaced = true;
		IsSelected = false;
		Tiles = new List<Tile> ();
		BuildingQueue = new List<GameObject> ();
	}
	// Use this for initialization
	void Start () {
		transform.position = new Vector3 (Root.pos.x, transform.position.y, Root.pos.y);
		if(isPlaced) {
			Init();
		}
		Constructables = new Dictionary<string,string> ();
		Constructables.Add ("Cube", "MakeCube");
		Constructables.Add ("Destroy", "DestroyBuilding");
	}

	public void Init() {

		Tiles = GetCurrentTiles ();

	}
	// Update is called once per frame
	void Update () {
		UpdateQueue ();
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
	void OnMouseDown() {
		if(IsSelected) {
			IsSelected = false;
		} else {
			GameObject[] buildings = GameObject.FindGameObjectsWithTag("Building");
			foreach(GameObject go in buildings) {
				if(go.GetComponent<Building>().IsSelected) {
					go.GetComponent<Building>().IsSelected = false;
				}
			}
			IsSelected = true;
		}
	}
	public void DestroyBuilding() {
		Object.Destroy (this.gameObject, 0);
	}
	public void MakeCube() {
		GameObject newEntry = (GameObject)Instantiate(CubePrefab, FindFreeAdjacentTile(), Quaternion.Euler(0,0,0));
		newEntry.SetActive (false);
		BuildingQueue.Add (newEntry);
	}
	public Vector3 FindFreeAdjacentTile() {
		foreach(Tile t in this.Tiles) {
			foreach(Tile t2 in t.Neighbors) {
				if(!t2.IsOccupied) {
					return new Vector3(t2.pos.x, transform.position.y +2, t2.pos.y);
				}
			}
		}
		return new Vector3(0,0,0);
	}
	public void UpdateQueue() {
		if(BuildingQueue.Count != 0) {
			BuildingQueue [0].GetComponent<GameUnit> ().BuildTimeTemp -= Time.deltaTime;
			Debug.Log(BuildingQueue [0].GetComponent<GameUnit> ().BuildTimeTemp);
			if(BuildingQueue [0].GetComponent<GameUnit> ().BuildTimeTemp <= 0) {
				BuildingQueue[0].SetActive(true);
				BuildingQueue.Remove(BuildingQueue[0]);
			}
		}
	}
	public void RemoveFromQueue(GameObject go) {
		BuildingQueue.Remove (go);
		Object.Destroy (go);
	}
}

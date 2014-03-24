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
	public float ConstructionTimeAbsolute;
	private float constructionTimeTemp;
	public static Dictionary<ResourceType, int> ResourceCosts;

	public float ConstructionTimeTemp {
		get { return constructionTimeTemp; }
		set { constructionTimeTemp = value; }
	}

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

		Constructables = new Dictionary<string, string>();
		Constructables.Add("Cube", "MakeCube");
		Constructables.Add("Destroy", "DestroyBuilding");

		constructionTimeTemp = ConstructionTimeAbsolute;
		
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
		renderer.enabled = false;

	}
	// Update is called once per frame
	void Update () {
		if (isPlaced) {
			if (constructionTimeTemp <= 0) {
				renderer.enabled = true;
			} else {
				constructionTimeTemp -= Time.deltaTime;
			}
		}
		UpdateQueue ();
	}
	void OnGUI() {
		if (constructionTimeTemp > 0 && isPlaced) {

			Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);

			Texture2D progressBarTexture = new Texture2D(1, 1);
			progressBarTexture.SetPixel(0,0,Color.blue);
			progressBarTexture.Apply();

			GUI.Box(new Rect(screenPos.x, Screen.height - screenPos.y, Screen.width / 10, 20), "");
			GUIStyle progressBarStyle = new GUIStyle();
			progressBarStyle.normal.background = progressBarTexture;
			GUI.Box(new Rect(screenPos.x, Screen.height - screenPos.y,
				 Screen.width / 10 / ConstructionTimeAbsolute * (ConstructionTimeAbsolute - constructionTimeTemp),
				20),
				"", progressBarStyle);
		}
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
			if (isPlaced) {
				GameObject[] buildings = GameObject.FindGameObjectsWithTag("Building");
				foreach (GameObject go in buildings) {
					if (go.GetComponent<Building>().IsSelected) {
						go.GetComponent<Building>().IsSelected = false;
					}
				}
				GameObject[] units = GameObject.FindGameObjectsWithTag("Unit");
				foreach (GameObject go in units) {
					if (go.GetComponent<GameUnit>().IsSelected) {
						go.GetComponent<GameUnit>().IsSelected = false;
					}
				}
				IsSelected = true;
			}
		}
	}
	public void DestroyBuilding() {
		Object.Destroy (this.gameObject, 0);
	}
	public void MakeCube() {
		if(BuildingQueue.Count < 5) {
			GameObject newEntry = (GameObject)Instantiate(CubePrefab, new Vector3(0,0,0), Quaternion.Euler(0,0,0));
			newEntry.SetActive (false);
			BuildingQueue.Add (newEntry);
		}
	}
	public Vector3 FindFreeAdjacentTile() {
		foreach(Tile t in this.Tiles) {
			foreach(Tile t2 in t.Neighbors) {
				bool occupiedByUnit = false;
				foreach (GameObject go in GameObject.FindGameObjectsWithTag("Unit")) {
					if ((int)go.transform.position.x == t2.pos.x &&  (int)go.transform.position.z == t2.pos.y) {
						occupiedByUnit = true;
						Debug.Log("occupied");
					}
				}
				if(!t2.IsOccupied && !occupiedByUnit) {
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
				BuildingQueue[0].transform.position = FindFreeAdjacentTile();
				BuildingQueue[0].SetActive(true);
				BuildingQueue.Remove(BuildingQueue[0]);
			}
		}
	}
	public void RemoveFromQueue(GameObject go) {
		BuildingQueue.Remove (go);
		Object.Destroy (go, 0);
	}
}

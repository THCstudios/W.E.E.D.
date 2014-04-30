using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Collections.Generic;
using System.Threading;

public enum ResourceType { Wood, Gold, Food };
public class Resource : MonoBehaviour{

	private float capacityAbsolute = 50;

	public float CapacityAbsolute {
		get { return capacityAbsolute; }
		set { capacityAbsolute = value; }
	}
	private float capacityTemp;

	public float CapacityTemp {
		get { return capacityTemp; }
		set { capacityTemp = value; }
	}

	private Level Level;
	public List<Tile> Tiles;
	public List<Vector2> TileCoords;

	private bool isSelected;
	private GameObject controlling;

	public int Width;
	public int Height;

	public ResourceType resourceType;

	public bool IsSelected {
		set {
			isSelected = value;
			if (isSelected) {
				GetComponentInChildren<Projector>().enabled = true;
			} else {
				GetComponentInChildren<Projector>().enabled = false;
			}
		}
		get {
			return isSelected;
		}
	}

	public Tile Root {
		get {
			if (Level.tiles == null) {
				Level.GenerateTiles(Level.Terrain.terrainData);
			}
			return Level.tiles[(int)transform.position.x, (int)transform.position.z];
		}
	}

	public void Start() {
		controlling = GameObject.FindGameObjectWithTag("GameController");
		Level = controlling.GetComponent<Level>();
		Tiles = GetCurrentTiles();
		capacityTemp = capacityAbsolute;
		resourceType = ResourceType.Wood;

	}
	public List<Tile> GetCurrentTiles() {
		List<Tile> tiles = new List<Tile>();
		TileCoords = new List<Vector2>();

		int rootX = (int)Root.pos.x;
		int rootY = (int)Root.pos.y;
		for (int i = 0; i < Width; i++) {
			for (int j = 0; j < Height; j++) {
				tiles.Add(Level.tiles[rootX + i, rootY + j]);
				TileCoords.Add(new Vector2(rootX + i, rootY + j));
			}
		}
		for (int i = 0; i > Width; i--) {
			for (int j = 0; j > Height; j--) {
				tiles.Add(Level.tiles[rootX + i, rootY + j]);
				TileCoords.Add(new Vector2(rootX + i, rootY + j));
			}
		}
		return tiles;
	}
	public void Update() {
		if (capacityTemp == 0) {
			UnityEngine.Object.Destroy(this.gameObject, 0);
		}
	}
	void OnMouseDown() {
		if (IsSelected) {
			IsSelected = false;
			controlling.GetComponent<GUIControl>().selectedResource = null;
		} else {
			Controller.RemoveAllSelections();
			IsSelected = true;
			controlling.GetComponent<GUIControl>().selectedResource = this;
		}
	}
}


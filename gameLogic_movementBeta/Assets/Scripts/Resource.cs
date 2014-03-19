using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts {
	class Resource : MonoBehaviour{

		private float capacityAbsolute;
		private float capacityTemp;

		private Level Level;
		public List<Tile> Tiles;
		public List<Vector2> TileCoords;

		public int Width;
		public int Height;

		public Tile Root {
			get {
				if (Level.tiles == null) {
					Level.GenerateTiles(Level.Terrain.terrainData);
				}
				return Level.tiles[(int)transform.position.x, (int)transform.position.z];
			}
		}

		public void Start() {
			Level = GameObject.FindGameObjectWithTag("GameController").GetComponent<Level>();
			Tiles = GetCurrentTiles();
			capacityTemp = capacityAbsolute;

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

	}
}

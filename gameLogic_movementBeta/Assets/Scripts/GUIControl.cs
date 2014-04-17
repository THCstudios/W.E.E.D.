using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

public class GUIControl : MonoBehaviour {

	public GameObject Controlling;
	public GameObject BuildingPrefab;
	private bool placeable;
	private GameObject newBuilding;
	public Building selectedBuilding;
	private GUIStyle progressBarStyle;
	Player me;

	void Awake() {
		
	}
	// Use this for initialization
	void Start () {

		Controlling = GameObject.FindGameObjectWithTag("GameController");
		Debug.Log(Controlling);
		foreach (Player p in Controlling.GetComponent<Controller>().players) {
			if (p.isMe) {
				me = p;
			}
		}
		Debug.Log(me);
		selectedBuilding = null;
		newBuilding = null;
		
	}
	
	// Update is called once per frame
	void Update () {

		selectedBuilding = null;
		GameObject[] buildings = GameObject.FindGameObjectsWithTag ("Building");
		foreach(GameObject b in buildings) {
			if(b.GetComponent<Building>().IsSelected) {
				selectedBuilding = b.GetComponent<Building>();
			}
		}
	
		if (newBuilding != null) {
			if (Input.GetKey(KeyCode.Escape)) {
				Object.Destroy(newBuilding);
				newBuilding = null;
			} else {
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				RaycastHit hit;
				if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << 8)) {

					newBuilding.GetComponent<Transform>().position = new Vector3(Controlling.GetComponent<Level>().tiles[(int)hit.point.x, (int)hit.point.z].pos.x,
																				 hit.point.y,
																				 Controlling.GetComponent<Level>().tiles[(int)hit.point.x, (int)hit.point.z].pos.y);
					placeable = true;
					List<Tile> buildingTiles = newBuilding.GetComponent<Building>().GetCurrentTiles();
					foreach (Tile t in buildingTiles) {
						if (t.IsOccupied) {
							placeable = false;
						}
					}
					newBuilding.renderer.material.SetColor("_Color", (placeable) ? Color.green : Color.gray);
				}
				if (Input.GetMouseButton(0) && placeable) {
					newBuilding.GetComponent<Building>().Init();
					newBuilding.GetComponent<Building>().isPlaced = true;
					TerrainData terrain = GameObject.FindGameObjectWithTag("Terrain").GetComponent<Terrain>().terrainData;
					int terrX = (int)((newBuilding.GetComponent<Building>().Root.pos.x - GameObject.FindGameObjectWithTag("Terrain").transform.position.x) / terrain.size.x * terrain.heightmapWidth);
					int terrY = (int)((newBuilding.GetComponent<Building>().Root.pos.y - GameObject.FindGameObjectWithTag("Terrain").transform.position.z) / terrain.size.z * terrain.heightmapHeight);
					Debug.Log((int)(5 / terrain.size.x * terrain.heightmapWidth) + 1);
					float[,] heightInfo = terrain.GetHeights(terrX,
															 terrY,
															 (int)(5 / terrain.size.x * terrain.heightmapWidth) + 2,//(int)(newBuilding.renderer.bounds.size.x / terrain.size.x * terrain.heightmapWidth),
															 (int)(5 / terrain.size.z * terrain.heightmapWidth) + 2);//(int)(newBuilding.renderer.bounds.size.z / terrain.size.z * terrain.heightmapHeight));
					for (int i = 0; i < heightInfo.GetLength(0); i++) {
						for (int j = 0; j < heightInfo.GetLength(1); j++) {
							heightInfo[i, j] = newBuilding.transform.position.y / terrain.size.y;
						}
					}
					terrain.SetHeights(terrX,
									   terrY,
									   heightInfo);
					foreach (ResourceType r in Building.ResourceCosts.Keys) {
						me.resources[r] -= Building.ResourceCosts[r];
					}
					newBuilding = null;
				}
			}
		}
	}
	void OnGUI(){
		
		GUILayout.BeginArea(new Rect(0, 0, Screen.width / 5, Screen.height / 16));
		GUI.Box(new Rect(0,0,Screen.width / 5, Screen.height / 8), "");
		GUILayout.BeginHorizontal();
		GUILayout.Label((me.resources[ResourceType.Wood]).ToString(), GUILayout.Width(Screen.width / 15));
		GUILayout.Label((me.resources[ResourceType.Food]).ToString(), GUILayout.Width(Screen.width / 15));
		GUILayout.Label((me.resources[ResourceType.Gold]).ToString(), GUILayout.Width(Screen.width / 15));
		GUILayout.EndHorizontal();
		GUILayout.EndArea();



		if(progressBarStyle == null) {
			Texture2D progressBarTexture = new Texture2D(1, 1);
			progressBarTexture.SetPixel(0,0,Color.blue);
			progressBarTexture.Apply();
			progressBarStyle = new GUIStyle (GUI.skin.label);
			progressBarStyle.normal.background = progressBarTexture;
		}

		GUIContent content1 = new GUIContent ();
		content1.image = (Texture2D)Resources.Load ("house_icon");

		foreach (ResourceType r in Building.ResourceCosts.Keys) {
			if (Building.ResourceCosts[r] > me.resources[r]) {
				GUI.enabled = false;
			}
		}
		GUI.enabled = (GUI.enabled) ? newBuilding == null : false;
		if (GUI.Button (new Rect (0, Screen.height - 200, 50, 50), content1)) {
			InstantiateNewBuilding();
		}
		GUI.enabled = true;

		if(selectedBuilding != null) {
			Rect groupRect = new Rect(Screen.width/5,Screen.height/5*4,Screen.width/5,Screen.height/5);
			GUILayout.BeginArea(groupRect);
			GUI.Box(new Rect(0,0,groupRect.width,groupRect.height),"");
			GUILayout.BeginVertical();
			GUILayout.BeginHorizontal();
			foreach(GameObject go in selectedBuilding.BuildingQueue) {
				if(GUILayout.Button((new GUIContent().image = go.GetComponent<GameUnit>().Icon),GUILayout.Width(40),GUILayout.Height(40))) {
					selectedBuilding.RemoveFromQueue(go);
				}
			}
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			GUIStyle style = new GUIStyle(GUI.skin.label);
			style.margin = new RectOffset(4,4,4,4);
			GUILayout.Label("",style, GUILayout.Width(groupRect.width));
			if(selectedBuilding.BuildingQueue.Count != 0) {


				GameUnit tempGU = selectedBuilding.BuildingQueue[0].GetComponent<GameUnit>();
				GUI.Label(new Rect(GUILayoutUtility.GetLastRect().x,
				                   GUILayoutUtility.GetLastRect().y,
				                   GUILayoutUtility.GetLastRect().width/tempGU.BuildTimeAbsolute*(tempGU.BuildTimeAbsolute-tempGU.BuildTimeTemp),
				                   GUILayoutUtility.GetLastRect().height),
				          "",progressBarStyle);
			}
			GUILayout.EndHorizontal();
			GUILayout.EndVertical();
			GUILayout.EndArea();

			GUILayout.BeginArea(new Rect(Screen.width/5*2, Screen.height/5*4,Screen.width/5*2,Screen.height/5));
			GUI.Box(new Rect(0,0,Screen.width/5*2,Screen.width/5),"");
			GUILayout.BeginHorizontal();
			foreach(string key in selectedBuilding.Constructables.Keys) {
				if(GUILayout.Button((new GUIContent().image = (Texture2D)Resources.Load(key)), GUILayout.Height(40), GUILayout.Width(40))) {
					MethodInfo info = selectedBuilding.GetType().GetMethod(selectedBuilding.Constructables[key]);
					info.Invoke(selectedBuilding,null);
				}
			}
			GUILayout.EndHorizontal();
			GUILayout.EndArea();
		}
	}
	public void InstantiateNewBuilding() {

		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << 8)) {

			newBuilding = (GameObject)Instantiate(BuildingPrefab, hit.point, Quaternion.Euler(0, 0, 0));
			newBuilding.GetComponent<Building>().isPlaced = false;
			placeable = true;
		} else {
			newBuilding = (GameObject)Instantiate(BuildingPrefab, new Vector3(-1,0,-1), Quaternion.Euler(0, 0, 0));
			newBuilding.GetComponent<Building>().isPlaced = false;
			placeable = true;
		}
	}

}

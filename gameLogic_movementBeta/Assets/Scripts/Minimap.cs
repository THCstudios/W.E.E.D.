using UnityEngine;
using System.Collections;

public class Minimap : MonoBehaviour {

	private Rect minimapRect;
	private Terrain terrain;
	// Use this for initialization
	void Start () {
		minimapRect = new Rect (Screen.width / 5 * 4, Screen.height - Screen.width / 5, Screen.width / 5, Screen.width / 5);
		terrain = GameObject.FindGameObjectWithTag ("Terrain").GetComponent<Terrain> ();
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetMouseButtonDown(0)) {
			if(minimapRect.Contains(new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y))) {
				Debug.Log("minimap clicked!");
				Camera.main.transform.position = new Vector3(((Input.mousePosition.x - minimapRect.x) / minimapRect.width) * terrain.terrainData.size.x,
				                                             Camera.main.transform.position.y,
				                                             ((Input.mousePosition.y) / minimapRect.height) * terrain.terrainData.size.z);
			}
		}
	
	}
	void OnGUI(){
		/*Experimental Mini-Map
		 * The Building and Unit colors will later be exchanged by the player's own color
		 */
		GUI.BeginGroup (minimapRect);
		GUI.Box(new Rect(0,0,Screen.width/5,Screen.width/5),""); 
		GameObject[] units = GameObject.FindGameObjectsWithTag ("Unit");
		GameObject[] buildings = GameObject.FindGameObjectsWithTag ("Building");
		
		Terrain terrain = GameObject.FindGameObjectWithTag ("Terrain").GetComponent<Terrain> ();
		Texture2D unitIcon = (Texture2D)Resources.Load ("minimap_unit");
		Texture2D buildingIcon = (Texture2D)Resources.Load ("minimap_building");
		Texture2D cameraIcon = (Texture2D)Resources.Load ("camera_icon");
		foreach(GameObject go in units) {
			GUI.DrawTexture(new Rect(go.transform.position.x/terrain.terrainData.size.x*Screen.width / 5, 
			                         (Screen.width/5) - (go.transform.position.z/terrain.terrainData.size.z*Screen.width / 5) - 5,
			                         5,5),
			                unitIcon);
		}
		foreach(GameObject go in buildings) {
			GUI.DrawTexture(new Rect(go.transform.position.x/terrain.terrainData.size.x*Screen.width / 5, 
			                         (Screen.width/5) - (go.transform.position.z/terrain.terrainData.size.z*Screen.width / 5) - 10,
			                         10,10),
			                buildingIcon);
		}
		GUI.DrawTexture(new Rect(Camera.main.transform.position.x/terrain.terrainData.size.x*Screen.width / 5, 
		                         (Screen.width/5) - (Camera.main.transform.position.z/terrain.terrainData.size.z*Screen.width / 5) - 20,
		                         20,20),
		                cameraIcon);
		GUI.EndGroup ();
	}
}

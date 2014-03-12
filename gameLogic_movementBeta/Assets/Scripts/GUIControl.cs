using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GUIControl : MonoBehaviour {

	public GameObject Controlling;
	public GameObject BuildingPrefab;
	private bool placeable;
	private GameObject newBuilding;

	// Use this for initialization
	void Start () {
	
		newBuilding = null;
		Controlling = GameObject.FindGameObjectWithTag ("GameController");
	}
	
	// Update is called once per frame
	void Update () {
	
		if (newBuilding != null) {
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;
			if(Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << 8)){
				
				newBuilding.GetComponent<Transform>().position = new Vector3(Controlling.GetComponent<Level>().tiles[(int)hit.point.x, (int)hit.point.z].pos.x, 
				                                                             hit.point.y, 
				                                                             Controlling.GetComponent<Level>().tiles[(int)hit.point.x, (int)hit.point.z].pos.y);
				placeable = true;
				List<Tile> buildingTiles = newBuilding.GetComponent<Building>().GetCurrentTiles();
				foreach(Tile t in buildingTiles) {
					if(t.IsOccupied) {
						placeable = false;
					}
				}
				newBuilding.renderer.material.SetColor("_Color",(placeable)?Color.green:Color.gray);
			}
			if (Input.GetMouseButton (0) && placeable) {
				newBuilding.GetComponent<Building>().Init();
				newBuilding = null;
			}
		}

	}
	void OnGUI(){

		GUIContent content1 = new GUIContent ();
		content1.image = (Texture2D)Resources.Load ("house_icon");
		if (GUI.Button (new Rect (0, Screen.height - 200, 50, 50), content1)) {
			InstantiateNewBuilding();
		}
	}
	public void InstantiateNewBuilding() {

		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit hit;
		if(Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << 8)){

			newBuilding = (GameObject)Instantiate(BuildingPrefab, hit.point, Quaternion.Euler(0,0,0));
			newBuilding.GetComponent<Building>().isPlaced = false;
			placeable = true;
		}
	}

}

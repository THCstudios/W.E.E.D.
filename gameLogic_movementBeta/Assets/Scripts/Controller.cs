using UnityEngine;
using System.Collections;

public class Controller : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetMouseButtonDown(1)){
			Debug.Log("Pressed.");

			Ray ray= Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit = new RaycastHit();

			if(Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << 8)){
				GameObject[] units = GameObject.FindGameObjectsWithTag("Unit");

				foreach(GameObject unit in units) {
					if(unit.GetComponent<GameUnit>().IsSelected == true){
						unit.GetComponent<GameUnit>().IsAtTarget = false;
						//hit.point = new Vector3(hit.point.x, 0 , hit.point.z);
						unit.GetComponent<GameUnit>().DestinationPoint = hit.point;

						Debug.Log (hit.point);
					}
				}
			}
		}
	}
}

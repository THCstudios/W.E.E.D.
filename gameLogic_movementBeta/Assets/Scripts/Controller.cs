using UnityEngine;
using System.Collections;


/*
 * Changes:
 * 	+	Units now update their destination if mouse button 1 (right button) is being held down
 *  +	Added selection rectangle (TODO: rectangle design)
 */
public class Controller : MonoBehaviour {

	// Selection starting position
	private Vector2 startPos;
	// Selection ending position
	private Vector2 endPos;
	// Generates selection rectangle
	private Rect selection {
		get {
			int minX, minY, maxX, maxY;
			if (startPos.x < endPos.x) {
				minX = (int)startPos.x;
				maxX = (int)endPos.x;
			} else {
				minX = (int)endPos.x;
				maxX = (int)startPos.x;
			}
			if (startPos.y < endPos.y) {
				minY = (int)startPos.y;
				maxY = (int)endPos.y;
			} else {
				minY = (int)endPos.y;
				maxY = (int)startPos.y;
			}
			return new Rect (minX, minY, maxX - minX, maxY - minY);
		}
	}
	// Only true while the mouse button 0 (left button) is being held down
	private bool isSelecting;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetMouseButtonDown(1) || Input.GetMouseButton (1)){
			Debug.Log("Pressed.");

			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;

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
		if (Input.GetMouseButtonUp (0)) {
			isSelecting = false;
			startPos = endPos = Vector2.zero;
		}
		if (Input.GetMouseButtonDown (0)) {
			isSelecting = true;
			startPos = Input.mousePosition;
		}
		if (isSelecting && Input.GetMouseButton (0)) {
			// Only update selection if the mouse actually moved!
			// Subject for debate (what if units moved inbetween?)
			if (endPos != (Vector2) Input.mousePosition) {
				endPos = Input.mousePosition;

				GameObject[] units = GameObject.FindGameObjectsWithTag ("Unit");

				foreach (GameObject unit in units) {
					Vector3 screenCoords = Camera.main.WorldToScreenPoint (unit.transform.position);
					unit.GetComponent<GameUnit>().IsSelected = selection.Contains (screenCoords);
				}
			}
		}
	}

	// Adapts the rectangle's Y coordinate for gui usage
	private static Rect fixRectForGui(Rect r) {
		Rect rect = new Rect (r);
		rect.yMin = Screen.height - rect.yMin;
		rect.yMax = Screen.height - rect.yMax;
		return rect;
	}

	void OnGUI() {
		GUI.Box (fixRectForGui(selection), "");
	}
}

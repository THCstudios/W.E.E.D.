using UnityEngine;
using System.Collections.Generic;


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
	// The style used to draw the selection rectangle
	private GUIStyle selectionStyle;

	public List<Player> players;

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


	void Awake() {
		players = new List<Player>();
		Player me = new Player();
		me.isMe = true;
		Debug.Log(me);
		players.Add(me);

		Building.ResourceCosts = new Dictionary<ResourceType, int>() {
		   {ResourceType.Wood, 5},
		   {ResourceType.Gold, 5}
		};
	}
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
						//hit.point = new Vector3(hit.point.x, 0 , hit.point.z);
						if (unit.GetComponent<GameUnit>() is WorkerUnit) {
							unit.GetComponent<GameUnit>().DestinationPoint = Input.mousePosition;
						} else {
							unit.GetComponent<GameUnit>().DestinationPoint = hit.point;
						}
						

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
			startPos = Input.mousePosition;
		}
		if (!isSelecting && Input.GetMouseButton (0)) {
			if(((Mathf.Abs(((Vector2)Input.mousePosition).x - startPos.x) > 5)) || (Mathf.Abs(((Vector2)Input.mousePosition).y - startPos.y) > 5)) {
				isSelecting = true;
			}
		}

		if (isSelecting) {
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

	private void initStyle() {
		selectionStyle = new GUIStyle ("Box");
		selectionStyle.border.top = selectionStyle.border.bottom = 
			selectionStyle.border.left = selectionStyle.border.right = 2; 
	}

	void OnGUI() {
		if (selectionStyle == null) {
			initStyle();
		}
		if (isSelecting) {
			GUI.Box (fixRectForGui(selection), "", selectionStyle);
		}
	}
}

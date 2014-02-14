using UnityEngine;
using System.Collections.Generic;

/*
 * Changes:
 * 	#	Changed property IsSelected: now automatically adapts color on set
 */
public class GameUnit :  TopLevelUnits{

	[SerializeField]
	private float movementSpeed;
	[SerializeField]
	private bool isSelected;
	//[SerializeField]
	//private bool isAtTarget;
	//[SerializeField]
	//private Vector3 destinationPoint;
	[SerializeField]
	private double collisionTime;
	private bool moveOverload;

	public GameObject Level;
	public List<Vector3> Path;
	Vector3 halfBounds;

	public float MaximumDistance = 0f;

	// Use this for initialization
	void Start () {
		isSelected = false;
		movementSpeed = 2.0f;
		moveOverload = false;
		Collider collider = GetComponent<Collider>();
		halfBounds = new Vector3 (collider.bounds.size.x / 2, 0, collider.bounds.size.z / 2);
	}
	
	// Update is called once per frame
	public void Update () {
		if(!IsAtTarget) {
			MoveUnit();
		}
		// USE?
		/*RaycastHit hit;
		if(Physics.Raycast(transform.position,-Vector3.up,out hit)) {
			Vector3 forwd = Vector3.Cross(transform.right,hit.normal);
			transform.rotation = Quaternion.Slerp(transform.rotation,Quaternion.LookRotation(forwd, hit.normal), 3*Time.deltaTime);
		}*/
	}
	public void OnMouseDown(){
		IsSelected = !IsSelected;
	}
	public void OnCollisionEnter(Collision collision){
		if(collision.gameObject.tag != "Unit" && collision.gameObject.tag != "Terrain"){
			Debug.Log("collided");
			collisionTime = 0.2;
		}
	}
	public void OnCollisionStay(Collision collision) {
		if(collision.gameObject.tag != "Unit" && collision.gameObject.tag != "Terrain"){
			if(collisionTime > 0) {
				//Debug.Log(collisionTime);
				collisionTime -= Time.deltaTime;
			} else {
				if (!IsAtTarget) {
					//Debug.DrawRay(transform.position,(DestinationPoint - transform.position).normalized, Color.red);
					if(!Physics.Raycast(transform.position, (DestinationPoint - transform.position).normalized, 2f)) {
						//Debug.Log("view clear");
					} else {
						//Debug.Log ("!view clear");
						//Debug.DrawRay(transform.position, Vector3.Cross(transform.up, (DestinationPoint - transform.position).normalized), Color.red);
						/*if(!Physics.Raycast(transform.position, Vector3.Cross(transform.up, (destinationPoint - transform.position).normalized), 2f)) {
							moveOverload = true;
							rigidbody.velocity = Vector3.Cross(transform.up, (destinationPoint - transform.position).normalized).normalized * movementSpeed;
						}*/
					}
				}
			}
		}
	}
	public void FixedUpdate() {
		if(!IsAtTarget) {
			for (int i = 0; i < Path.Count - 1; i++) {
				Vector3 dir = Path[i + 1] - Path[i];
				Debug.DrawRay (Path[i], dir, Color.green);
				Debug.DrawRay (Path[i] + halfBounds, dir, Color.green);
				Debug.DrawRay (Path[i] - halfBounds, dir, Color.green);
				Debug.DrawRay (Path[i] + halfBounds * 1.1f, dir, Color.blue);
				Debug.DrawRay (Path[i] - halfBounds * 1.1f, dir, Color.blue);
			}
			//rigidbody.MovePosition(Vector3.MoveTowards (transform.position, destinationPoint, (float)(movementSpeed * Time.deltaTime)));
			//rigidbody.velocity = movementSpeed * rigidbody.velocity.normalized;
			if(!moveOverload) {
				Vector3 dir = (DestinationPoint - transform.position).normalized * movementSpeed;
				if ((DestinationPoint - transform.position - dir).magnitude < MaximumDistance) {
					dir = DestinationPoint - transform.position;
				}
				dir.y = rigidbody.velocity.y;
				rigidbody.velocity =  dir; //Vector3.MoveTowards (transform.position, destinationPoint, (float)(movementSpeed * Time.deltaTime));
			}
		}
	}
	public void MoveUnit(){
		var newRotation = Quaternion.LookRotation (DestinationPoint - transform.position).eulerAngles;
		newRotation.x = transform.rotation.eulerAngles.x;
		float tempY;
		if (Mathf.Abs(transform.rotation.eulerAngles.y - newRotation.y) >180) {
			if(transform.rotation.eulerAngles.y > newRotation.y) {
				tempY = Mathf.Lerp (transform.rotation.eulerAngles.y, newRotation.y+360f, Time.deltaTime * 3f);
			} else {
				tempY = Mathf.Lerp (transform.rotation.eulerAngles.y+360f, newRotation.y, Time.deltaTime * 3f);
			}
		} else {
			tempY = Mathf.Lerp (transform.rotation.eulerAngles.y, newRotation.y, Time.deltaTime * 3f);
		}
		newRotation.y = (tempY > 360f) ? tempY - 360f : tempY;
		newRotation.z = transform.rotation.eulerAngles.z;

		transform.rotation = Quaternion.Euler (newRotation);

		//this.rigidbody.MovePosition(Vector3.MoveTowards(transform.position, destinationPoint, (float) (movementSpeed * Time.deltaTime)));
		//rigidbody.AddForce(Vector3.MoveTowards (transform.position, destinationPoint, (float)(movementSpeed * Time.deltaTime)));
		//rigidbody.AddForce (Vector3.forward);
		//Debug.Log (Vector3.Distance (transform.position, DestinationPoint));
		if(Vector3.Distance(transform.position, DestinationPoint) <= MaximumDistance) {
			Path.RemoveAt (0);
		}
	}

	private Vector3 convert(Tile tile) {
		return new Vector3 (tile.pos.x * 2, 0, tile.pos.y * 2);
	}

	public List<Vector3> Optimize (List<Tile> path, Vector3 finalDest) {
		if (path == null) {
			return null;
		}
		List<Vector3> opped = new List<Vector3>();
		foreach (Tile tile in path) {
			opped.Add (new Vector3 (tile.pos.x + 0.5f, 0.5f, tile.pos.y + 0.5f));
		}
		opped.Add (finalDest);
		for (int i = 0; i < opped.Count - 2; i++) {
			for (int j = i + 2; j < opped.Count; j++) {
				Vector3 dir = opped[j] - opped[i];
				float distance = dir.magnitude;
				RaycastHit[] hits;
				hits = Physics.RaycastAll (opped[i], dir, distance);
				Collider c;
				foreach (RaycastHit hit in hits) {
					if (hit.collider != collider && hit.collider.gameObject.tag != "Terrain") {
						c = hit.collider;
						goto NotDirect;
					}
				}
				hits = Physics.RaycastAll (opped[i] - halfBounds * 1.1f, dir, distance);
				foreach (RaycastHit hit in hits) {
					if (hit.collider != collider && hit.collider.gameObject.tag != "Terrain") {
						c = hit.collider;
						goto NotDirect;
					}
				}
				hits = Physics.RaycastAll (opped[i] + halfBounds * 1.1f, dir, distance);
				foreach (RaycastHit hit in hits) {
					if (hit.collider != collider && hit.collider.gameObject.tag != "Terrain") {
						c = hit.collider;
						goto NotDirect;
					}
				}
				opped.RemoveRange (i + 1, j - i - 1);
				j = i + 1;
				continue;
			NotDirect:
					;
			}
		}
		return opped;
	}

	public float MovementSpeed {
		get {
			return this.movementSpeed;
		}
		set {
			movementSpeed = value;
		}
	}
	public bool IsSelected {
		get {
			return this.isSelected;
		}
		set {
			isSelected = value;
			if (isSelected) {
				renderer.material.color = Color.blue;
			} else {
				renderer.material.color = Color.red;
			}
		}
	}
	public bool IsAtTarget {
		get {
			return Path == null || Path.Count == 0;
		}
	}
	public Vector3 DestinationPoint {
		get {
			return Path[0];
		}
		set {
			Path = Optimize(Level.GetComponent<PathFinder>().FindPath (transform.position, value), value);
		}
	}

	public Tile Tile {
		get {
			return Level.GetComponent<Level>().tiles
				[(int)transform.position.x, (int) transform.position.z];
		}
	}
}

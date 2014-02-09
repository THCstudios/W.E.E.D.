using UnityEngine;
using System.Collections;

/*
 * Changes:
 * 	#	Changed property IsSelected: now automatically adapts color on set
 */
public class GameUnit :  TopLevelUnits{

	[SerializeField]
	private float movementSpeed;
	[SerializeField]
	private bool isSelected;
	[SerializeField]
	private bool isAtTarget;
	[SerializeField]
	private Vector3 destinationPoint;
	[SerializeField]
	private double collisionTime;
	private bool moveOverload;


	// Use this for initialization
	void Start () {
		isSelected = false;
		isAtTarget = true;
		movementSpeed = 2.0f;
		moveOverload = false;
	}
	
	// Update is called once per frame
	public void Update () {
		if(!isAtTarget) {
			MoveUnit();
		}
		RaycastHit hit;
		if(Physics.Raycast(transform.position,-Vector3.up,out hit)) {
			Vector3 forwd = Vector3.Cross(transform.right,hit.normal);
			transform.rotation = Quaternion.Slerp(transform.rotation,Quaternion.LookRotation(forwd, hit.normal), 3*Time.deltaTime);
		}
	}
	public void OnMouseDown(){
		if (!isSelected) {
			isSelected = true;
			renderer.material.color = Color.blue;
		} else {
			isSelected = false;
			renderer.material.color = Color.red;
		}
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
				Debug.Log(collisionTime);
				collisionTime -= Time.deltaTime;
			} else {
				Debug.DrawRay(transform.position,(destinationPoint - transform.position).normalized, Color.red);
				if(!Physics.Raycast(transform.position, (destinationPoint - transform.position).normalized, 2f)) {
					Debug.Log("view clear");
				} else {
					Debug.Log ("!view clear");
					Debug.DrawRay(transform.position, Vector3.Cross(transform.up, (destinationPoint - transform.position).normalized), Color.red);
					if(!Physics.Raycast(transform.position, Vector3.Cross(transform.up, (destinationPoint - transform.position).normalized), 2f)) {
						moveOverload = true;
						rigidbody.velocity = Vector3.Cross(transform.up, (destinationPoint - transform.position).normalized).normalized * movementSpeed;
					}
				}
			}
		}
	}
	public void FixedUpdate() {
		if(!isAtTarget) {
			//rigidbody.MovePosition(Vector3.MoveTowards (transform.position, destinationPoint, (float)(movementSpeed * Time.deltaTime)));
			//rigidbody.velocity = movementSpeed * rigidbody.velocity.normalized;
			if(!moveOverload)
				rigidbody.velocity = (destinationPoint - transform.position).normalized * movementSpeed; //Vector3.MoveTowards (transform.position, destinationPoint, (float)(movementSpeed * Time.deltaTime));
		}
	}
	public void MoveUnit(){
		var newRotation = Quaternion.LookRotation (destinationPoint - transform.position).eulerAngles;
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
		if(Vector3.Distance(transform.position, destinationPoint) < 0.2) {
			isAtTarget = true;
		}
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
			return this.isAtTarget;
		}
		set {
			isAtTarget = value;
		}
	}
	public Vector3 DestinationPoint {
		get {
			return this.destinationPoint;
		}
		set {
			destinationPoint = value;
		}
	}
}

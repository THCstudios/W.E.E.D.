using UnityEngine;
using System.Collections;

public class GameUnit :  TopLevelUnits{

	[SerializeField]
	private float movementSpeed;
	[SerializeField]
	private bool isSelected;
	[SerializeField]
	private bool isAtTarget;
	[SerializeField]
	private Vector3 destinationPoint;

	// Use this for initialization
	void Start () {
		isSelected = false;
		isAtTarget = true;
		movementSpeed = 2.0f;
	}
	
	// Update is called once per frame
	void Update () {
		if(!isAtTarget) {
			MoveUnit();
		}
		RaycastHit hit = new RaycastHit ();
		if(Physics.Raycast(transform.position,-Vector3.up,out hit)) {
			Vector3 forwd = Vector3.Cross(transform.right,hit.normal);
			transform.rotation = Quaternion.Slerp(transform.rotation,Quaternion.LookRotation(forwd, hit.normal), 3*Time.deltaTime);
		}
	}
	void OnMouseDown(){
		if (!isSelected) {
			isSelected = true;
			renderer.material.color = Color.blue;
		} else {
			isSelected = false;
			renderer.material.color = Color.red;
		}
	}
	void OnCollisionEnter(){

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

		this.rigidbody.MovePosition(Vector3.MoveTowards(transform.position, destinationPoint, (float) (movementSpeed * Time.deltaTime)));
		if(Vector3.Distance(transform.position, destinationPoint) < 0.5) {
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

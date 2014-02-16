using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour {

	public float cameraSpeed = 30;
	public float ZoomSpeed = 200;
	public float boundary = 20;

	private float xRotation, yRotation;

	// Use this for initialization
	void Start () {
		// boundary = 5;
		// cameraSpeed = 20;
		xRotation = transform.eulerAngles.x;
		yRotation = transform.eulerAngles.y;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyUp (KeyCode.LeftAlt)) {
			Screen.lockCursor = false;
		}
		if (Input.GetKeyDown (KeyCode.LeftAlt)) {
			Screen.lockCursor = true;
		}

		if (Input.GetKey (KeyCode.LeftAlt)) {
			xRotation -= Input.GetAxis ("Mouse Y") * cameraSpeed * Time.deltaTime;
			yRotation += Input.GetAxis ("Mouse X") * cameraSpeed * Time.deltaTime;
			transform.eulerAngles = new Vector3 (xRotation, yRotation, 0);
		} else {
			if(Input.mousePosition.x < boundary) {
				transform.Translate(Vector3.left * cameraSpeed * Time.deltaTime);
			}
			if(Input.mousePosition.x > (Screen.width - boundary)) {
				transform.Translate(Vector3.right * cameraSpeed * Time.deltaTime);
			}
			if(Input.mousePosition.y < boundary) {
				Vector3 normal = Vector3.Cross (Vector3.left, transform.worldToLocalMatrix * Vector3.up) * cameraSpeed;
				transform.Translate(normal * Time.deltaTime);
			}
			if(Input.mousePosition.y > (Screen.height - boundary)) {
				Vector3 normal = Vector3.Cross (transform.worldToLocalMatrix * Vector3.up, Vector3.left) * cameraSpeed;
				transform.Translate(normal * Time.deltaTime);
			}
		}
		transform.Translate (Vector3.forward * ZoomSpeed * Time.deltaTime * Input.GetAxis ("Mouse ScrollWheel"));
	}
}

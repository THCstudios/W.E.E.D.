using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour {

	public float cameraSpeed;
	public float boundary;

	// Use this for initialization
	void Start () {
		boundary = 10;
		cameraSpeed = 20;
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.mousePosition.x < boundary) {
			transform.Translate(Vector3.left*cameraSpeed*Time.deltaTime, Space.World);
		}
		if(Input.mousePosition.x > (Screen.width - boundary)) {
			transform.Translate(Vector3.right*cameraSpeed*Time.deltaTime, Space.World);
		}
		if(Input.mousePosition.y < boundary) {
			transform.Translate(Vector3.back*cameraSpeed*Time.deltaTime, Space.World);
		}
		if(Input.mousePosition.y > (Screen.height - boundary)) {
			transform.Translate(Vector3.forward*cameraSpeed*Time.deltaTime,Space.World);
		}
	}
}

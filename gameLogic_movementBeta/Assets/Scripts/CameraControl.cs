using UnityEngine;
using System.Collections;

/*
 * Changes: 
 * 	-	removed boundary and cameraSpeed initialization from Start() to allow changes per editor
 * 	+	added ZoomSpeed field for Camera zoom
 *	+	Implemented zoom per mouse wheel
 */
public class CameraControl : MonoBehaviour {

	public float cameraSpeed = 30;
	public float ZoomSpeed = 200;
	public float boundary = 20;

	// Use this for initialization
	void Start () {
		// boundary = 5;
		// cameraSpeed = 20;
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
		transform.Translate (Vector3.forward * ZoomSpeed * Time.deltaTime * Input.GetAxis ("Mouse ScrollWheel"));
	}
}

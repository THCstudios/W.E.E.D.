using UnityEngine;
using System.Collections;

public class CircleProjector : MonoBehaviour {

	void Update () {
		transform.localPosition = Vector3.zero;
		transform.rotation = Quaternion.Euler (new Vector3(90, 0));
	}
}

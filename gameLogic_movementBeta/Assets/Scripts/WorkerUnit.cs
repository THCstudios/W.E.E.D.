using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class WorkerUnit : GameUnit {

	[SerializeField]
	private float maxResourceLoad;
	private float resourceLoad;
	private float gatheringSpeed;
	private ResourceType carriedType;
	private GameObject resourceNode;
	private GameObject homeNode;
	private bool onWayBack;
	private List<Vector3> otherTarget;

	public float ResourceLoad {
		get { return resourceLoad; }
		set { resourceLoad = value; }
	}

	public override void Awake() {
		base.BuildTimeTemp = BuildTimeAbsolute;
	}
	// Use this for initialization
	public override void Start () {
		base.Start();
		resourceNode = null;
		homeNode = null;
		gatheringSpeed = 3;
		resourceLoad = 0;
		maxResourceLoad = 10;
	}
	public override void Update() {
		base.Update();
		if (IsAtTarget) {
			if (!onWayBack && resourceNode != null) {
				Debug.Log("Is at resource");
				if (resourceLoad >= maxResourceLoad) {
					Debug.Log(resourceLoad + ", " + maxResourceLoad);
					resourceLoad -= (resourceLoad - maxResourceLoad);
					onWayBack = true;
					Path = Optimize(Controlling.GetComponent<PathFinder>().FindPath(transform.position, homeNode.transform.position), homeNode.transform.position);
				} else {
					if (resourceLoad == 0) {
						carriedType = resourceNode.GetComponent<Resource>().resourceType;
						resourceLoad += gatheringSpeed * Time.deltaTime;
						resourceNode.GetComponent<Resource>().CapacityTemp -= gatheringSpeed * Time.deltaTime;
					} else {
						if (carriedType != resourceNode.GetComponent<Resource>().resourceType) {
							carriedType = resourceNode.GetComponent<Resource>().resourceType;
							resourceLoad = gatheringSpeed * Time.deltaTime;
							resourceNode.GetComponent<Resource>().CapacityTemp -= gatheringSpeed * Time.deltaTime;
						} else {
							resourceLoad += gatheringSpeed * Time.deltaTime;
							resourceNode.GetComponent<Resource>().CapacityTemp -= gatheringSpeed * Time.deltaTime;
						}
					}
				}
			} else if (onWayBack) {
				Debug.Log("Is at building");
				if (resourceLoad > 0 && carriedType != null) {
					foreach (Player p in Controlling.GetComponent<Controller>().players) {
						if (p.isMe) {
							p.resources[carriedType] += (int)resourceLoad;
							resourceLoad = 0;
						}
					}
				}
				if (resourceNode != null) {
					Path = Optimize(Controlling.GetComponent<PathFinder>().FindPath(transform.position, resourceNode.transform.position), resourceNode.transform.position);
					onWayBack = false;
				}
			}
		}
	} 
	
	public override Vector3 DestinationPoint {
		get {
			return Path[0];
		}
		set {

			Debug.Log("blub");
			Ray ray = Camera.main.ScreenPointToRay(value);
			RaycastHit hit;

			if (Physics.Raycast(ray, out hit, Mathf.Infinity)) {
				if (hit.collider.gameObject.GetComponent<Resource>() != null) {
					Debug.Log("I move towards a resource!");
					resourceNode = hit.collider.gameObject;
					resourceNode.renderer.material.color = Color.green;
					GameObject[] buildings = GameObject.FindGameObjectsWithTag("Building");
					GameObject nearestBuilding = buildings[0];
					for (int i = 1; i < buildings.Length; i++) {
						if ((transform.position - nearestBuilding.transform.position).magnitude > (transform.position - buildings[i].transform.position).magnitude) {
							nearestBuilding = buildings[i];
						}
					}
					homeNode = nearestBuilding;
					homeNode.renderer.material.color = Color.blue;
					onWayBack = false;

				} else if (hit.collider.gameObject.GetComponent<Building>() != null) {
					Debug.Log("I move towards a building!");
					resourceNode = null;
					homeNode = hit.collider.gameObject;
					onWayBack = true;
				} else {
					resourceNode = null;
					homeNode = null;
					onWayBack = false;
				}
				RaycastHit hit2;
				if (Physics.Raycast(ray, out hit2, Mathf.Infinity, 1 << 8)) {
					Path = Optimize(Controlling.GetComponent<PathFinder>().FindPath(transform.position, hit2.point), hit2.point);
				}
			}
		}
	}
}

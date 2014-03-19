using UnityEngine;
using System.Collections;
using Assets.Scripts;

public class WorkerUnit : GameUnit {

	[SerializeField]
	private float resourceLoad;
	private bool assignedToGather;

	public bool AssignedToGather {
		get { return assignedToGather; }
		set { assignedToGather = value; }
	}

	public float ResourceLoad {
		get { return resourceLoad; }
		set { resourceLoad = value; }
	}

	void Awake() {
		base.BuildTimeTemp = BuildTimeAbsolute;
	}
	// Use this for initialization
	void Start () {
		base.IsSelected = false;
		base.MovementSpeed = 3f;
		base.MoveOverload = false;
		assignedToGather = false;

		Collider collider = GetComponent<Collider>();
		base.HalfBounds = new Vector3(collider.bounds.size.x / 2, 0, collider.bounds.size.z / 2);
		base.Controlling = GameObject.FindGameObjectWithTag("GameController");
	}
	
	// Update is called once per frame
	/*void Update () {
		base.Update();
	}
	void OnMouseDown() {
		base.OnMouseDown();
	}
	void FixedUpdate() {
		base.FixedUpdate();
	}
	void MoveUnit() {
		base.MoveUnit();
	}*/
	public override Vector3 DestinationPoint {
		get {
			return base.DestinationPoint;
		}
		set {
			Path = Optimize(Controlling.GetComponent<PathFinder>().FindPath(transform.position, value), value);
			foreach (GameObject go in GameObject.FindGameObjectsWithTag("Resource")) {
				if (go.GetComponent<Resource>().Tiles.Contains(GameObject.FindGameObjectWithTag("GameController").GetComponent<Level>().tiles[(int)value.x, (int)value.z])) {
					assignedToGather = true;
				}
			}
		}
	}
}

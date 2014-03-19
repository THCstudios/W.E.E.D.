using UnityEngine;
using System.Collections.Generic;

/*
 * Changes:
 * 	#	Changed property IsSelected: now automatically adapts color on set
 */
public class GameUnit :  TopLevelUnits
{
	[SerializeField]
	private float
		movementSpeed;
	[SerializeField]
	private bool
		isSelected;
	[SerializeField]
	private double
		collisionTime;
	private bool moveOverload;
	public GameObject Controlling;
	public ProcessingPath Path;
	public ProcessingPath BackupPath;
	public List<Vector3> Waypoints;
	public List<Vector3> BackupWaypoints;
	Vector3 halfBounds;
	public UnitStats Stats;
	public int Health;
	public GUIStyle BarStyle;
	public Texture2D healthbar_healthy;
	public Texture2D healthbar_hurt;
	public Texture2D healthbar_neardeath;
	public Texture2D healthbar_background;
	public GameUnit Target;
	private Tile lastKnownTargetPos;
	public float MaximumDistance = 0f;
	public float BuildTimeAbsolute;
	private float buildTimeTemp;
	public Texture2D Icon;
	private int progress;

	public float BuildTimeTemp { 
		get { return buildTimeTemp; } 
		set { buildTimeTemp = value; } 
	}

	void Awake ()
	{
		buildTimeTemp = BuildTimeAbsolute;
		Debug.Log (buildTimeTemp);
	}

	// Use this for initialization
	void Start ()
	{
		Stats = new UnitStats (null);//new FeMoObject ());
		Health = Stats.CachedMaxHealth;
		isSelected = false;
		movementSpeed = 2.0f;
		moveOverload = false;

		halfBounds = new Vector3 (collider.bounds.size.x / 2, 0, collider.bounds.size.z / 2);
		Controlling = GameObject.FindGameObjectWithTag ("GameController");

		if (healthbar_background == null) {
			healthbar_background = Resources.Load<Texture2D> ("healthbar_background");
		}
		if (healthbar_healthy == null) {
			healthbar_healthy = Resources.Load<Texture2D> ("healthbar_healthy");
		}
		if (healthbar_hurt == null) {
			healthbar_hurt = Resources.Load<Texture2D> ("healthbar_hurt");
		}
		if (healthbar_neardeath == null) {
			healthbar_neardeath = Resources.Load<Texture2D> ("healthbar_neardeath");
		}

		InitUnit ();
	}

	public virtual void InitUnit ()
	{
		
	}

	// Update is called once per frame
	public void Update ()
	{
		if (Path != null) {
			Waypoints = Optimize (Path, Waypoints [Waypoints.Count - 1]);
			BackupWaypoints = new List<Vector3> (Waypoints);
			BackupWaypoints.Insert (0, transform.position);
			if (Path.finished || Path.cancelled) {
				Path = null;
			}
			progress = 0;
		}
		if (Health <= 0) {
			Destroy (gameObject);
		}
		if (Target != null && lastKnownTargetPos != Target.Tile) {
			Attack (Target);
		}
		if (BackupWaypoints != null && BackupWaypoints.Count > 1) {
			int progress = this.progress == BackupWaypoints.Count - 1 ? BackupWaypoints.Count - 2 : this.progress;
			Vector3 newRotation = Quaternion.LookRotation (BackupWaypoints [progress + 1] - BackupWaypoints [progress]).eulerAngles;
			Vector3 oldRotation = transform.rotation.eulerAngles;
			if (newRotation != oldRotation) {
				RotateUnit (newRotation);
			}
		}

		// USE?
		RaycastHit hit;
		if (Physics.Raycast (transform.position, -Vector3.up, out hit)) {
			Vector3 forwd = Vector3.Cross (transform.right, hit.normal);
			transform.rotation = Quaternion.Slerp (transform.rotation, Quaternion.LookRotation (forwd, hit.normal), 3 * Time.deltaTime);
		}
	}

	public void Attack (GameUnit unit)
	{
		Debug.Log ("Attacking " + unit);
		DestinationPoint = unit.transform.position;
		Target = unit;
		lastKnownTargetPos = Target.Tile;
	}
	
	public void Goto (Vector3 position)
	{
		Target = null;
		lastKnownTargetPos = null;
		DestinationPoint = position;
	}

	public void OnMouseDown ()
	{
		IsSelected = !IsSelected;
	}

	public void OnCollisionEnter (Collision collision)
	{
		if (collision.gameObject.tag != "Unit" && collision.gameObject.tag != "Terrain") {
			Debug.Log ("collided");
			collisionTime = 0.2;
		}
	}

	public void OnCollisionStay (Collision collision)
	{
		if (collision.gameObject.tag != "Unit" && collision.gameObject.tag != "Terrain") {
			if (collisionTime > 0) {
				//Debug.Log(collisionTime);
				collisionTime -= Time.deltaTime;
			} else {
				if (!IsAtTarget) {
					//Debug.DrawRay(transform.position,(DestinationPoint - transform.position).normalized, Color.red);
					if (!Physics.Raycast (transform.position, (DestinationPoint - transform.position).normalized, 2f)) {
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

	public void FixedUpdate ()
	{
		Animation animation = GetComponentInChildren<Animation> ();
		if (!IsAtTarget) {
			if (animation.clip == null || animation.clip.name != "Walk") {
				animation.Play ("Walk");
			}
			for (int i = 0; BackupPath != null && i < BackupPath.path.Count - 1; i++) {
				Debug.DrawLine (new Vector3 (BackupPath.path [i].pos.x + 0.5f, 0.5f, BackupPath.path [i].pos.y + 0.5f), new Vector3 (BackupPath.path [i + 1].pos.x + 0.5f, 0.5f, BackupPath.path [i + 1].pos.y + 0.5f), Color.cyan);
			}
			for (int i = 0; BackupWaypoints != null && i < BackupWaypoints.Count - 1; i++) {
				Debug.DrawLine (BackupWaypoints [i], BackupWaypoints [i + 1], Color.blue);
				//Debug.DrawRay (BackupWaypoints[i] + halfBounds * 1.1f, dir, Color.green, distance);
			}
			//rigidbody.MovePosition(Vector3.MoveTowards (transform.position, destinationPoint, (float)(movementSpeed * Time.deltaTime)));
			//rigidbody.velocity = movementSpeed * rigidbody.velocity.normalized;
			if (!moveOverload) {
				Vector3 dir = (DestinationPoint - transform.position).normalized * movementSpeed;
				if (movementSpeed * Time.fixedDeltaTime > (DestinationPoint - transform.position).magnitude) {
					dir = (DestinationPoint - transform.position) / Time.fixedDeltaTime;
				}

				dir.y = rigidbody.velocity.y;
				rigidbody.velocity = dir; //Vector3.MoveTowards (transform.position, destinationPoint, (float)(movementSpeed * Time.deltaTime));
			}			
			if (Vector3.Distance (transform.position, DestinationPoint) <= MaximumDistance) {
				Waypoints.RemoveAt (0);
				progress++;
			}

		} else {
			rigidbody.velocity = new Vector3 (0, rigidbody.velocity.y);
		}
		if (animation != null && (animation.clip == null || !animation.isPlaying)) {
			animation.Play ("Idle");
		}
	}

	public void RotateUnit (Vector3 newRotation)
	{
		newRotation.x = transform.rotation.eulerAngles.x;
		float tempY;
		if (Mathf.Abs (transform.rotation.eulerAngles.y - newRotation.y) > 180) {
			if (transform.rotation.eulerAngles.y > newRotation.y) {
				tempY = Mathf.Lerp (transform.rotation.eulerAngles.y, newRotation.y + 360f, Time.deltaTime * 3f);
			} else {
				tempY = Mathf.Lerp (transform.rotation.eulerAngles.y + 360f, newRotation.y, Time.deltaTime * 3f);
			}
		} else {
			tempY = Mathf.Lerp (transform.rotation.eulerAngles.y, newRotation.y, Time.deltaTime * 3f);
		}
		newRotation.y = (tempY > 360f) ? tempY - 360f : tempY;
		newRotation.z = transform.rotation.eulerAngles.z;

		transform.rotation = Quaternion.Euler (newRotation);
	}

	public void OnGUI ()
	{
		if (BarStyle.name != "HealthBar") {
			BarStyle = new GUIStyle ("Box");
			BarStyle.name = "HealthBar";
			BarStyle.normal.background = null;
			BarStyle.fontStyle = FontStyle.Bold;
		}
		Vector3 bar = transform.position;
		bar.y += .5f;
		bar = Camera.main.WorldToScreenPoint (bar);
		bar.y = Screen.height - bar.y - 15;
		bar.z = 0;
		GUI.DrawTexture (new Rect (bar.x - 20, bar.y - 2.5f, 40, 5), healthbar_background);
		float relHP = Health / (float) Stats.CachedMaxHealth;
		Texture2D healthbar;
		if (relHP > .5f) {
			healthbar = healthbar_healthy;
		} else if (relHP > .2f) {
			healthbar = healthbar_hurt;
		} else {
			healthbar = healthbar_neardeath;
		}
		GUI.DrawTextureWithTexCoords (new Rect (bar.x - 19, bar.y - 1.5f, 38 * Health / Stats.CachedMaxHealth, 3), healthbar, new Rect (0, 0, Health / Stats.CachedMaxHealth, 1));
		GUI.Box (new Rect (bar.x - 50, bar.y - 10, 100, 20), Health.ToString (), BarStyle);
	}

	public List<Vector3> Optimize (ProcessingPath path, Vector3 finalDest)
	{
		if (path == null) {
			return null;
		}
		List<Vector3> opped = new List<Vector3> ();
		opped.Add (transform.position);
		foreach (Tile tile in path.path) {
			opped.Add (new Vector3 (tile.pos.x + 0.5f, 0.5f, tile.pos.y + 0.5f));
		}
		opped.Add (finalDest);
		for (int i = 0; i < opped.Count - 2; i++) {
			for (int j = i + 2; j < opped.Count; j++) {
				Vector3 dir = opped [j] - opped [i];
				float distance = dir.magnitude;
				RaycastHit[] hits;
				hits = Physics.RaycastAll (opped [i], dir, distance);
				foreach (RaycastHit hit in hits) {
					if (hit.collider != collider && hit.collider.gameObject.tag != "Terrain") {
						goto NotDirect;
					}
				}
				hits = Physics.RaycastAll (opped [i] - halfBounds * 1.1f, dir, distance);
				foreach (RaycastHit hit in hits) {
					if (hit.collider != collider && hit.collider.gameObject.tag != "Terrain") {
						goto NotDirect;
					}
				}
				hits = Physics.RaycastAll (opped [i] + halfBounds * 1.1f, dir, distance);
				foreach (RaycastHit hit in hits) {
					if (hit.collider != collider && hit.collider.gameObject.tag != "Terrain") {
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
		opped.Remove (transform.position);
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
			GetComponentInChildren<Projector> ().enabled = isSelected = value;
		}
	}

	public bool IsAtTarget {
		get {
			return (Waypoints == null || Waypoints.Count == 0);
		}
	}

	public Vector3 DestinationPoint {
		get {
			return Waypoints [0];
		}
		set {
			if (Path != null) {
				Path.cancelled = true;
			}
			Path = Controlling.GetComponent<PathFinder> ().FindPath (transform.position, value);
			BackupPath = Path;
			Waypoints = Optimize (Path, value + new Vector3 (0, .5f, 0));
			BackupWaypoints = new List<Vector3> (Waypoints);
			BackupWaypoints.Insert (0, transform.position);
		}
	}
	
	public Tile Tile {
		get {
			return Controlling.GetComponent<Level> ().tiles
				[(int) transform.position.x, (int) transform.position.z];
		}
	}
}

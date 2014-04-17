using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player {

	public Dictionary<ResourceType, float> resources;
	public Color color;
	public string name;
	public bool isMe;

	// Use this for initialization
	public Player() {
		resources = new Dictionary<ResourceType, float>();
		resources.Add(ResourceType.Wood, 20);
		resources.Add(ResourceType.Food, 20);
		resources.Add(ResourceType.Gold, 20);
	}
}

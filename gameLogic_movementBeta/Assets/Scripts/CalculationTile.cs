using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


public class CalculationTile {

	private bool isOccupied;

	public bool IsOccupied {
		get { return isOccupied; }
		set { isOccupied = value; }
	}
	private Vector2 pos;

	public Vector2 Pos {
		get { return pos; }
		set { pos = value; }
	}

	public CalculationTile(bool isOccupied, float posX, float posY) {
		this.isOccupied = isOccupied;
		pos = new Vector2(posX, posY);
	}
}

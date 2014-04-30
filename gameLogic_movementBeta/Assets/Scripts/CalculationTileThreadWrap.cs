using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class CalculationTileThreadWrap {
	private CalculationTileMap map;

	internal CalculationTileMap Map {
		get { return map; }
		set { map = value; }
	}
	private CalculationTile dst;

	internal CalculationTile Dst {
		get { return dst; }
		set { dst = value; }
	}
	private CalculationTile src;

	internal CalculationTile Src {
		get { return src; }
		set { src = value; }
	}


	public CalculationTileThreadWrap(CalculationTileMap tiles, Vector3 src, Vector3 dst) {
		map = tiles;
		this.dst = map.Tiles[(int)dst.x, (int)dst.z];
		this.src = map.Tiles[(int)src.x, (int)src.z];
	}
}

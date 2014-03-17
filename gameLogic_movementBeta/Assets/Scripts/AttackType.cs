using System.Collections.Generic;

public class AttackType
{
	public static readonly List<AttackType> AllTypes = new List<AttackType>();

	public static readonly AttackType Melee = new AttackType("Melee");
	public static readonly AttackType Ranged = new AttackType("Ranged");
	public static readonly AttackType Sword = new AttackType("Sword", Melee);
	public static readonly AttackType LongSword = new AttackType("Long Sword", Sword, Melee);
	public static readonly AttackType Bow = new AttackType("Bow", Ranged);
	public static readonly AttackType CrossBow = new AttackType("Crossbow", Bow, Ranged);

	public string Name;
	public List<AttackType> Parents = new List<AttackType>();

	public AttackType(string name) {
		Name = name;
		AllTypes.Add (this);
	}

	public AttackType(string name, params AttackType[] parents) : this(name) {
		Parents.AddRange (parents);
	}
}


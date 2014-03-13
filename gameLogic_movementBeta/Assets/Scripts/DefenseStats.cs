using System.Collections.Generic;

public class DefenseStats
{
	public Dictionary<AttackType, float> DefenseEffectivity = new Dictionary<AttackType, float>();

	public DefenseStats() {
		foreach (AttackType type in AttackType.AllTypes) {
			DefenseEffectivity.Add (type, 1);
		}
	}

	public float GetEffectivity(GameUnit unit) {
		float factor;
		float val;
		DefenseEffectivity.TryGetValue (unit.Stats.CachedAttackType, out val);
		factor = val;
		foreach (AttackType parent in unit.Stats.CachedAttackType.Parents) {
			DefenseEffectivity.TryGetValue (parent, out val);
			factor *= val;
		}
		return factor;
	}

	public DefenseStats AddEffectivity (AttackType type, float effectivity) {
		if (DefenseEffectivity.ContainsKey (type)) {
			DefenseEffectivity.Remove (type);
		}
		DefenseEffectivity.Add (type, effectivity);
		return this;
	}
}
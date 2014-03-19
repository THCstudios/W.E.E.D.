//using SharedMemory;

public class UnitStats 
{
	//private FeMoObject backing;
	
	public AttackType CachedAttackType;
	public DefenseStats CachedDefenseStats = new DefenseStats ();
	public float CachedAttackDamage;
	public int CachedAttackRange;
	public int CachedMaxHealth = 100;

	public UnitStats(object o) {

	}

	/*public UnitStats (FeMoObject backing) {
		this.backing = backing;
		backing.AddDecimal ("AttackDamage", CachedAttackDamage);
		backing.AddInt ("AttackRange", CachedAttackRange);
		backing.AddInt ("MaxHealth", CachedMaxHealth);
	}*/

	/*public void RenewCache() {
		CachedAttackDamage = AttackDamage;
		CachedAttackRange = AttackRange;
		CachedMaxHealth = MaxHealth;
	}

	public float AttackDamage {
		get {
			return (float)backing.GetDecimal ("AttackDamage");
		}

		set {
			backing.SetDecimal ("AttackDamage", value);
		}
	}

	public int AttackRange {
		get {
			return backing.GetInt ("AttackRange");
		}

		set {
			backing.SetInt ("AttackRange", value);
		}
	}

	public int MaxHealth {
		get {
			return backing.GetInt ("MaxHealth");
		}
		
		set {
			backing.SetInt ("MaxHealth", value);
		}
	}*/
}



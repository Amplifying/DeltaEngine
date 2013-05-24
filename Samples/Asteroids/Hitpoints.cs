namespace Asteroids
{
	/// <summary>
	/// HP of an entity
	/// </summary>
	public class Hitpoints
	{
		public Hitpoints(int maximumHp)
		{
			HP = maximumHp;
		}

		public int HP;

		public bool HitByDmgAndCheckSurvival(int damageHp)
		{
			HP -= damageHp;
			return (HP >= 0);
		}
	}
}
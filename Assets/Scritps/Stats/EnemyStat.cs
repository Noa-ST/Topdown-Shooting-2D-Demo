using UnityEngine;

[CreateAssetMenu(fileName = "Enemy Stats", menuName = "Nora/TDS/Create Enemy Stats")]
public class EnemyStat : ActionStats
{
    [Header("Xp Bonus:")]
    public float minXpBonus;
    public float maxXpBonus;

    [Header("Level Up:")]
    public float hpUp;
    public float damageUp;
}

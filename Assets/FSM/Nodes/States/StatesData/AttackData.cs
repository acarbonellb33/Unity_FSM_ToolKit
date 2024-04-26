public class AttackData: StateScriptData
{
    public float attackDamage = 10f;
    public float attackFrequency = 1f;
    
    public AttackData()
    {
        SetStateName("Attack");
    }
}

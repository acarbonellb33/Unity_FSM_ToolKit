public class HealthData : StateScriptData
{
    public FSMOperands operand;
    public float health = 10f;
    
    public HealthData()
    {
        SetStateName("Health");
    }
}

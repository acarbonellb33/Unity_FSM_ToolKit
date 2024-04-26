public class HearingData : StateScriptData
{
    public FSMOperands operand;
    public float hearingRange = 10f;
    
    public HearingData()
    {
        SetStateName("Hearing");
    }
}

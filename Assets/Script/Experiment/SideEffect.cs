using UnityEngine;

public enum SideEffectSeverity
{
    Minor,
    Moderate,
    Severe
}

[CreateAssetMenu(fileName = "SideEffect", menuName = "Scriptable Objects/SideEffect")]
public class SideEffect : ScriptableObject
{
    public string effectName;
    public SideEffectSeverity severity;
    public int effectID;
}

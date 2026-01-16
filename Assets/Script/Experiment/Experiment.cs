using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Experiment", menuName = "Scriptable Objects/Experiment")]
public class Experiment : ScriptableObject
{
    public String name;
    public String Des;
    public int experimentID; // 어떤 능력인지 구분하기 위한 ID
    //이미지 추가 예정 
}

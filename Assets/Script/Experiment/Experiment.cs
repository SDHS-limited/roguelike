using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Experiment", menuName = "Scriptable Objects/Experiment")]
public class Experiment : ScriptableObject
{
    public String name;
    public String Buff;
    public String Nerf;
    public int experimentID; // 어떤 능력인지 구분하기 위한 ID
    public int requiredSideEffects; // 이 실험을 해금하기 위해 필요한 최소 후유증 개수
// public Sprite image;
    //이미지 추가 예정 
}

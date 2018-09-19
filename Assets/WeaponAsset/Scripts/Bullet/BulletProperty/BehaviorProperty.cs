using UnityEngine;
using UnityEditor;

[System.Serializable]
public abstract class BehaviorProperty : BulletProperty
{
    public abstract BehaviorProperty Clone();
    public abstract void Behavior();
}
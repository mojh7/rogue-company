using UnityEngine;

[System.Serializable]
public abstract class BehaviorProperty : BulletProperty
{
    public abstract BehaviorProperty Clone();
    public abstract void Behavior();
}
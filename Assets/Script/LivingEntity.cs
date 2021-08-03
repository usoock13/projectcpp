using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LivingEntity : MonoBehaviour, IDamageable
{
    [field: SerializeField]
    public float currentHP { get; protected set; }
    [field: SerializeField]
    public float maxHp { get; protected set; }
    protected bool isDead { get; private set; } = false;

    protected void Start(){
        currentHP = maxHp;
    }
    public virtual void OnDamage(float amount, Vector3 originDirection) {
        if(isDead) return;

        currentHP -= amount;
        if(currentHP <= 0) OnDie();
    }
    public virtual void OnDie() {
        isDead = true;
    }
}

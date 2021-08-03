using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable {
    void OnDamage(float amount, Vector3 originDirection);
    void OnDie();
}

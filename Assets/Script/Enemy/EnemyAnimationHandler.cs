using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimationHandler : MonoBehaviour
{
    [SerializeField]
    Enemy EnemyInstance;

    void BasicAttackStart(){
        EnemyInstance.BasicAttackStart();
    }
    void BasicAttackHit(){
        EnemyInstance.BasicAttackHit();
    }
    void BasicAttackEnd(){
        EnemyInstance.BasicAttackEnd();
    }
    void HitEnd() {
        EnemyInstance.HitEnd();
    }
}

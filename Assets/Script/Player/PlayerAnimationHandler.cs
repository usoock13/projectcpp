using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationHandler : MonoBehaviour
{
    [SerializeField]
    private Player playerInstance;

    void EarlyInputStart() {
        playerInstance.EarlyInputStart();
    }
    void EarlyInputEnd() {
        playerInstance.EarlyInputEnd();
    }
    void BasicAttackHit(int attackOrder) {
        playerInstance.BasicAttackHit(attackOrder);
    }
    void BasicAttackStart() {
        playerInstance.BasicAttackStart();
    }
    void DodgeEnd() {
        playerInstance.DodgeEnd();
    }
    void HitEnd() {
        playerInstance.HitEnd();
    }
    void DodgeAttackHit() {
        playerInstance.DodgeAttackHit();
    }
    void FootR() {
        
    }
    void FootL() {
        
    }
}
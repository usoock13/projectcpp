using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    [SerializeField] // Player Instance
    private Player playerInstance;

    void FixedUpdate(){
        WatchMove();
    }
    void Update() {
        if(Input.GetButtonDown("Fire1")) InputBasicAttack(); // 일반 공격
        if(Input.GetButton("Fire2")) InputSpecialAttack(); // 특수 공격
        if(Input.GetButtonDown("Dodge")) InputDodge(); // 회피
    }

    void WatchMove() {
        Vector3 direction = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        playerInstance.PlayerMove(direction.normalized);
    }

    void InputBasicAttack() {
        RaycastHit hit;
        if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, 1 << 8)) {
            switch(playerInstance.playerState) {
                case PlayerState.Dodge :
                case PlayerState.SecondDodge :
                    playerInstance.DodgeAttack(hit.point);
                    break;
                case PlayerState.Idle :
                case PlayerState.Move :
                case PlayerState.BasicAttack :
                    playerInstance.BasicAttack(hit.point);
                    break;
            }
        }
    }
    void InputSpecialAttack() {

    }
    void InputDodge() {
        playerInstance.Dodge();
    }
}

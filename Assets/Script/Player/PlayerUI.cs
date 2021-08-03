using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    public Player playerComponent;
    public GameObject hpBarUIObject;

    public void PlayerHPBarUpdate() {
        playerComponent ??= GetComponent<Player>();
        hpBarUIObject.GetComponent<Slider>().value = playerComponent.currentHP / playerComponent.maxHp;
    }
}

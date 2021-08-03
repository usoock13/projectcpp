using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Enemy : LivingEntity
{
    Player targetPlayer;
    EnemyState enemyState;
    protected GameObject enemyHpUI;

    public Canvas UICanvas;
    public GameObject enemyHpUiPrefab;

    RectTransform enemyUITransform;

    new public void Start() {
        base.Start();
        enemyHpUI = GameObject.Instantiate(enemyHpUiPrefab, UICanvas.gameObject.transform);
        enemyUITransform = enemyHpUI.GetComponent<RectTransform>();
    }
    public void Update() {
        if(!isDead) UIUpdate();
    }

    public virtual void BasicAttackStart(){
        
    }
    public virtual void BasicAttackHit(){
        
    }
    public virtual void BasicAttackEnd(){
        
    }
    public virtual void HitEnd(){
        
    }

    void UIUpdate() {
        Collider collider = GetComponent<Collider>();
        Vector3 hpBarPoint;

        if(collider) {
            hpBarPoint = new Vector3(transform.position.x, transform.position.y + collider.bounds.size.y + .5f, transform.position.z);
            enemyUITransform.position = Camera.main.WorldToScreenPoint(hpBarPoint);
            
            enemyUITransform.sizeDelta = new Vector2((collider.bounds.size.x * 100f) + 50f, enemyUITransform.sizeDelta.y);
        } else {
            Debug.LogError("UIUpdate Method :: MissingComponentException :: 충돌체(Collider)가 없습니다. 크기가 유동적인 UI는 해당 객체의 충돌체를 기준으로 크기가 변환됩니다. 충돌체를 찾을 수 없어 해당 기능을 활성화 할 수 없습니다.");
            print(isDead);
        }
    }
    public override void OnDie() {
        base.OnDie();
        enemyHpUI.SetActive(false);
    }
}

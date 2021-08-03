using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public enum EnemyState {
    Idle, Chase, Attack, SpecialAttack
}
public class EnemyNormal : Enemy
{
    EnemyState enemyState = EnemyState.Idle;
    [SerializeField]
    float attackPower = 15f;
    float basicAttackDelay = .3f;
    bool canAttack = true;

    [SerializeField]
    NavMeshAgent navMeshAgent;
    IEnumerator enemyCoroutine;
    List<GameObject> targets = new List<GameObject>();
    GameObject mainTarget;

    // 범위 체크용 Colliders
    public ColliderEvent checkedInAroundEvent; // 플레이어 추적 범위
    public ColliderEvent playerIsAttackRangeEvent; // 공격 실행 범위
    public Collider hitCheckCollider;

    [SerializeField]
    Animator enemyAnimator;

    public void GetTargetHandler(Collider other) {
        if(other.tag == "Player" && !isDead) {
            targets.Add(other.gameObject);
        }
    }
    public void LostTargetHandler(Collider other) {
        if(other.tag == "Player" && !isDead) targets.Remove(other.gameObject);
    }
    public void TargetIsNearEvent(Collider other) {
        if(
            other.tag == "Player" 
            && enemyState != EnemyState.Attack 
            && enemyState != EnemyState.SpecialAttack
            && !isDead
        ) {
            transform.LookAt(mainTarget.transform.position);
            if(canAttack) {
                navMeshAgent.isStopped = true;
                enemyState = EnemyState.Attack;
                enemyAnimator.SetTrigger("Basic Attack");
            }
        }
    }
    new void Start() {
        base.Start();

        checkedInAroundEvent.triggerEnterEvent += GetTargetHandler;
        checkedInAroundEvent.triggerExitEvent += LostTargetHandler;

        playerIsAttackRangeEvent.triggerStayEvent += TargetIsNearEvent;
        StartCoroutine("ChaseTarget");

        enemyHpUI.GetComponent<Slider>().value = currentHP / maxHp;
    }
    IEnumerator ChaseTarget() {
        while(!isDead) {
            if(targets.Count > 0) {
                mainTarget = targets[0];
                targets.ForEach(target => {
                    if( Vector3.Distance(transform.position, target.transform.position) < Vector3.Distance(transform.position, mainTarget.transform.position) )
                        mainTarget = target;
                });
                navMeshAgent.SetDestination(mainTarget.transform.position);

                if(enemyState == EnemyState.Idle || enemyState == EnemyState.Chase) {
                    enemyAnimator.SetBool("Chase Target", true);
                    navMeshAgent.isStopped = false;
                }
            } else {
                enemyAnimator.SetBool("Chase Target", false);
                navMeshAgent.isStopped = true;
            }
            yield return new WaitForSeconds(.5f);
        }
    }

    public override void BasicAttackEnd() {
        canAttack = false;
        Invoke("BasicAttackCooldown", basicAttackDelay);
    }
    public void BasicAttackCooldown() {
        if(targets.Count > 0) {
            enemyAnimator.SetBool("Chase Target", true);
            if(!isDead) navMeshAgent.isStopped = false;
        } else {
            enemyAnimator.SetBool("Chase Target", false);
            navMeshAgent.isStopped = true;
        }
        enemyAnimator.SetTrigger("Attack Delay Exit");
        enemyState = EnemyState.Idle;
        canAttack = true;
    }
    public override void BasicAttackHit() {
        Collider[] colliders = Physics.OverlapBox(hitCheckCollider.bounds.center, hitCheckCollider.bounds.size, hitCheckCollider.transform.rotation , LayerMask.GetMask("Player"));
        for(int i=0; i<colliders.Length; i++) {
            Player player = colliders[i].GetComponent<Player>();
            player.OnDamage(attackPower, transform.position);
        }
    }
    public override void OnDamage(float amount, Vector3 originDirection)
    {
        if(!isDead) {
            base.OnDamage(amount, originDirection);
            enemyAnimator.SetTrigger("Hit");
            enemyHpUI.GetComponent<Slider>().value = currentHP / maxHp;
        }
    } 
    // 애니메이션 이벤트 핸들러 메소드
    public override void HitEnd() {
        enemyState = EnemyState.Idle;
    }

    public override void OnDie()
    {
        base.OnDie();
        enemyAnimator.SetTrigger("Die");
        navMeshAgent.isStopped = true;
        Destroy(GetComponent<Collider>());
        Destroy(GetComponent<NavMeshAgent>());
    }
}

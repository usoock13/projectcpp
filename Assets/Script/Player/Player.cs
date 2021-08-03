using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerState {
    Idle, Move, BasicAttack, Dodge, SecondDodge, GetHit, DodgeAttack
}
public class Player : LivingEntity
{
    [SerializeField]
    private GameObject playerModel; // Player 캐릭터 모델
    [SerializeField]
    private Animator playerAnimator;
    [SerializeField]
    private PlayerSoundManager playerSoundManager;
    [SerializeField]
    private PlayerUI playerUI;
    [SerializeField]
    public PlayerState playerState { get; private set; } = PlayerState.Idle; // Player 현재 상태
    private Rigidbody playerRigidbody;
    private bool canMove = true; // Player는 현재 이동이 가능한 상태인가?
    private bool canAttack = true;
    private bool canDodge = true;

    private bool canInputEarlyBasicAttack = false;

    private float moveSpeed = 5f; // Player 이동속도
    private float dodgeCoefficient = 2.5f;
    private float dodgeCooldownTime = .3f;
    private float attackPower = 35f;

    private float dodgeAttackDistanceCoef = .9f;

    Vector3 attackPoint; // Player 공격 방향 (최근 마우스 클릭 좌표)
    Vector3 inputDirection; // Player 누르고 있는 키보드 방향

    [SerializeField]
    Transform particlePointLeft;
    [SerializeField]
    Transform particlePointRight;
    
    Queue<GameObject> basicAttackParticlePool = new Queue<GameObject>();
    [SerializeField]
    GameObject basicAttackParticle;
    
    new void Start() {
        base.Start();
        playerRigidbody = GetComponent<Rigidbody>();
        playerSoundManager = GetComponent<PlayerSoundManager>();
        playerUI = GetComponent<PlayerUI>();

        ParticleInitialize(10, basicAttackParticle);

        playerUI.PlayerHPBarUpdate(); // 플레이어 체력바 UI 초기화 
    }

    public Vector3 MoveVector(Vector3 direction) {
        RaycastHit hit;
        if(Physics.BoxCast(
            (transform.position + new Vector3(0, .5f, 0)) - (direction.normalized * .7f), // BoxCast 시작 지점
            new Vector3(.25f, .1f, .25f), // BoxCast 박스 크기
            direction.normalized, // BoxCast 박스 진행 방향
            out hit, // BoxCast RaycastHit 정보
            playerModel.transform.rotation, // BoxCast 박스 방향
            .7f, // BoxCast 박스 진행 거리
            LayerMask.GetMask("Block Move"))) // BoxCast 확인할 레이어
        {
            Vector3 normal = new Vector3(hit.normal.x, 0, hit.normal.z);
            float angle = Mathf.Round(Vector3.Angle(direction, Quaternion.AngleAxis(90, Vector3.up) * normal) - 90);
            float coef = (180 - Vector3.Angle(direction, normal))/90;
            Vector3 resultVector = Quaternion.AngleAxis(angle, Vector3.up) * direction * coef;
            return resultVector;
        } else {
            return direction;
        }
    }
    public void PlayerMove(Vector3 direction) {
        inputDirection = Quaternion.AngleAxis(45f, Vector3.up) * direction;
        if(playerState == PlayerState.Idle || playerState == PlayerState.Move) canMove = true; // Player의 현재 상태가 Idel, Move 중 하나라면 이동 가능한 상태로 복귀
        if(canMove) {
            if(inputDirection == Vector3.zero) playerAnimator.SetBool("Move", false); // 입력받는 이동 값이 없어, direction이 (0, 0, 0)으로 들어올 경우, Player Animator의 Move 애니메이션 비활성화
            else {
                playerModel.transform.localRotation = Quaternion.LookRotation(direction);
                playerRigidbody.MovePosition(transform.position + MoveVector(inputDirection) * moveSpeed * Time.deltaTime);
                playerAnimator.SetBool("Move", true); // Player Animator의 Move 애니메이션 재생
                playerState = PlayerState.Move; // Player 현재 상태를 이동으로 변경
            }
        } else {
            return; // Player가 이동 가능한 상태인지 체크
        }
    }
    // 회피 관련 메소드 >>
    IEnumerator dodgeCoroutine; // 회피는 두 번 까지 연속으로 사용 가능 / 회피의 이동 처리는 코루틴으로 처리
                                // 두 번째 회피가 첫 번째 회피를 중단시키고 작동하기 위해 이를 위한 코루틴 변수를 선언 (StopCoroutine 사용)
    public void Dodge() {
        if(!canDodge) return; // 닷지 가능 여부 확인
        playerState = playerState==PlayerState.Dodge ? PlayerState.SecondDodge : PlayerState.Dodge;
        playerAnimator.SetBool("Move", false); // Dodge 애니메이션 종료 후 방향키 입력 여부와 무관하게 Move 애니메이션이 실행되는 것을 막음
        playerAnimator.SetTrigger("Dodge"); // 애니메이션 시작
        playerSoundManager.PlaySound(playerSoundManager.dodgeSound); //Dodge 사운드 출력

        canMove = false; // 일반 이동 불가능
        canAttack = false; // 공격 불가능
        if(playerState == PlayerState.SecondDodge) canDodge = false; // 회피 불가능

        if(dodgeCoroutine != null) StopCoroutine(dodgeCoroutine);
        dodgeCoroutine = DodgeMove(playerState);
        StartCoroutine(dodgeCoroutine);
    }
    IEnumerator DodgeMove(PlayerState dodgeState) {
        Vector3 direction = inputDirection==Vector3.zero ? Quaternion.AngleAxis(-45, Vector3.up) * playerModel.transform.forward : inputDirection.normalized;
        Vector3 dodgeDistance = direction;
        Quaternion lookDirection = Quaternion.LookRotation(Quaternion.AngleAxis(-45, Vector3.up) * dodgeDistance);

        playerRigidbody.velocity = Vector3.zero;
        while(playerState == dodgeState) {
            dodgeDistance = MoveVector(direction) * (moveSpeed);
            playerModel.transform.localRotation = lookDirection;
            playerRigidbody.MovePosition(transform.position + dodgeDistance * dodgeCoefficient * Time.fixedDeltaTime);
            yield return new WaitForFixedUpdate();
        }
    }
    public void DodgeEnd() { // 애니메이션 이벤트 핸들러 (회피 종료 시점)
        playerState = PlayerState.Idle;
        canMove = true;
        canAttack = true;
        canDodge = false; // 회피 쿨다운 타임을 위한 비활성화 (회피 한 번의 경우를 상정)
        Invoke("DodgeCooldown", dodgeCooldownTime); // 회피 쿨다운 타임 적용
    }
    public void DodgeCooldown() { // 회피 가능 상태로 전환
        canDodge = true;
    }
    // << 회피 관련 메소드

    IEnumerator attackMoveCoroutine;
    public void BasicAttack(Vector3 point) {
        if(playerState == PlayerState.Dodge || playerState == PlayerState.SecondDodge) {
            DodgeAttack(point);
            return;
        }
        if(!canAttack) return; // 공격 불가능 상태일 경우 반환(탈출)
        attackPoint = new Vector3(point.x, 0, point.z); // 공격 목적지 (마우스로 클릭한 지점)

        if(playerState != PlayerState.BasicAttack) {
            playerState = PlayerState.BasicAttack; // Player 현재 상태를 공격으로 변경
            canMove = false;
            playerAnimator.SetTrigger("Basic Attack"); // Player Animator의 Basic Attack 애니메이션 재생
        } else {
            if(canInputEarlyBasicAttack) {
                playerAnimator.SetBool("Input Early Basic Attack", true); // Player 공격 애니메이션 선입력 유무를 true로 설정
            }
        }
    }
    IEnumerator AttackMove(Vector3 moveSpeed, float duration) {
        float startTime = Time.time;
        playerRigidbody.velocity = Vector3.zero;
        while(startTime + duration > Time.time && playerState == PlayerState.BasicAttack) {
            playerRigidbody.MovePosition(transform.position + MoveVector(moveSpeed) * Time.fixedDeltaTime);
            yield return new WaitForFixedUpdate();
        }
    }
    public void EarlyInputStart() {
        playerAnimator.SetBool("Input Early Basic Attack", false); // Player 공격 애니메이션 선입력 유무를 false로 설정
        canInputEarlyBasicAttack = true; // 선입력 가능 타이밍 시작
    }
    public void EarlyInputEnd() { // 선입력을 받지 못함 > 공격 애니메이션 종료
        playerState = PlayerState.Idle;
        canInputEarlyBasicAttack = false; // 선입력 가능 타이밍 종료
    }
    public void BasicAttackHit(int attackOrder) {
        Collider[] targets = Overlapshape.OverlapFanshape(transform.position, 40f, attackPoint, 1.2f, 1 << LayerMask.NameToLayer("Damagable"), .25f);
        foreach(Collider target in targets) {
            target.gameObject.GetComponent<IDamageable>().OnDamage(attackPower, transform.position);
            playerSoundManager.PlaySound(playerSoundManager.basicAttackSounds[attackOrder]);

            Transform particlePoint = attackOrder%2==0 ? particlePointRight : particlePointLeft;
            Vector3 particlePosition = target.GetComponent<Collider>() ? target.GetComponent<Collider>().ClosestPoint(particlePoint.position) : particlePoint.position;
            GameObject attackParticle = 
                basicAttackParticlePool.Count>0 
                ? basicAttackParticlePool.Dequeue() 
                : GameObject.Instantiate(basicAttackParticle,  particlePosition, particlePoint.rotation);
            StartCoroutine(ActiveParticle(attackParticle, 2.5f, particlePoint));
        }
    }
    public void BasicAttackStart() {
        StartCoroutine(AttackMove(inputDirection.normalized * 2f, .35f));
        playerModel.transform.LookAt(attackPoint); // 공격 방향을 바라보기 (Player Model)
    } 

    public override void OnDamage(float amount, Vector3 originDirection) {
        base.OnDamage(amount, originDirection);
        Vector3 lookDirection = originDirection;
        lookDirection.y = playerModel.transform.position.y;
        playerModel.transform.LookAt(lookDirection);

        playerUI.PlayerHPBarUpdate();

        playerState = PlayerState.GetHit;
        canMove = false;
        canDodge = false;
        canAttack = false;
        playerAnimator.SetTrigger("Get Hit");
    }
    public void HitEnd() { // 피격 애니메이션 종료
        playerState = PlayerState.Idle;
        canMove = true;
        canDodge = true;
        canAttack = true;
    }

    void ParticleInitialize(int count, GameObject particle) {
        for(int i=0; i<count; i++) {
            GameObject instantiatedObj = GameObject.Instantiate(particle);
            instantiatedObj.SetActive(false);
            instantiatedObj.transform.SetParent(transform.Find("Player Particle Parent"));
            basicAttackParticlePool.Enqueue(instantiatedObj);
        }
    }
    IEnumerator ActiveParticle(GameObject particle, float returnTime, Transform particlePoint) {
        GameObject attackParticle = particle;
        attackParticle.transform.SetParent(null);
        attackParticle.transform.position = particlePoint.position;
        attackParticle.transform.rotation = particlePoint.rotation;
        attackParticle.SetActive(true);

        yield return new WaitForSeconds(returnTime);

        attackParticle.SetActive(false);
        attackParticle.transform.SetParent(transform.Find("Player Particle Parent"));
        basicAttackParticlePool.Enqueue(attackParticle);
    }
    IEnumerator ActiveParticle(GameObject particle, float returnTime, Vector3 particlePoint, Quaternion particleRotation) {
        GameObject attackParticle = particle;
        attackParticle.transform.SetParent(null);
        attackParticle.transform.position = particlePoint;
        attackParticle.transform.rotation = particleRotation;
        attackParticle.SetActive(true);

        yield return new WaitForSeconds(returnTime);

        attackParticle.SetActive(false);
        attackParticle.transform.SetParent(transform.Find("Player Particle Parent"));
        basicAttackParticlePool.Enqueue(attackParticle);
    }
    void DodgeAttack(Vector3 point) {
        canDodge = false;
        StopCoroutine(dodgeCoroutine); // 회피를 진행하던 코루틴 중단
        
        attackPoint = new Vector3(point.x, 0, point.z);
        playerState = PlayerState.DodgeAttack;
        playerModel.transform.LookAt(point);
        StartCoroutine(DodgeAttackMove(point));
        
        playerAnimator.SetTrigger("Dodge Attack");
        playerAnimator.SetBool("Dodge", false);
        playerSoundManager.PlaySound(playerSoundManager.dodgeAttackSound);
        
    }
    IEnumerator DodgeAttackMove(Vector3 point) {
        Vector3 direction = (point - transform.position).normalized;
        float count = 0;
        while(count < .25f) {
            count += .02f;
            playerRigidbody.MovePosition(transform.position + MoveVector(direction * dodgeAttackDistanceCoef * moveSpeed * Time.fixedDeltaTime));
            yield return new WaitForFixedUpdate();
            // yield return null;
        }
    }
    public void DodgeAttackHit() {
        Collider[] targets = Physics.OverlapBox(transform.position + playerModel.transform.forward * 2f, new Vector3(.4f, 1f, 2f), playerModel.transform.rotation, 1 << LayerMask.NameToLayer("Damagable"));
        playerAnimator.SetBool("Dodge Attack", false);
        
        if(targets.Length > 0) playerSoundManager.PlaySound(playerSoundManager.dodgeAttackHitSound);
        foreach(Collider target in targets) {
            target.GetComponent<IDamageable>().OnDamage(attackPower * 1.3f, transform.position);
            GameObject attackParticle = 
                basicAttackParticlePool.Count>0 
                ? basicAttackParticlePool.Dequeue() 
                : GameObject.Instantiate(basicAttackParticle, target.transform.position, particlePointRight.rotation);
            StartCoroutine(ActiveParticle(attackParticle, 2.5f, target.transform.position, particlePointRight.rotation));
        }
    }
}
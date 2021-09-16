using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleDirector : MonoBehaviour
{
    Dictionary<string, GameObject> particlePoolGameObjects;
    Dictionary<string, Queue<GameObject>> particlePools;

    public ParticleDirector() {
        particlePoolGameObjects = new Dictionary<string, GameObject>();
        particlePools = new Dictionary<string, Queue<GameObject>>();
    }
    
    // 파티클 풀 생성하여 파티클 추가
    // (이름, 추가할 파티클, 추가할 개수, 생성된 파티클이 부모로 가질 오브젝트(트랜스폼)(없으면 최상위에 생성))
    public Queue<GameObject> InitializeParticle(string name, GameObject particle, int count, Transform particleParent = null) {
        GameObject newParticlePoolGameObject = new GameObject(name);
        if(particleParent != null) newParticlePoolGameObject.transform.parent = particleParent.transform;

        Queue<GameObject> newParticlePool = new Queue<GameObject>();
        particlePoolGameObjects.Add(name, Instantiate(newParticlePoolGameObject, Vector3.zero, Quaternion.identity));

        for(int i=0; i<count; i++) {
            GameObject newParticle = Instantiate(particle, Vector3.zero, Quaternion.identity);
            newParticle.SetActive(false);
            newParticle.transform.parent = newParticlePoolGameObject.transform;
            newParticlePool.Enqueue(newParticle);
        }
        particlePools.Add(name, newParticlePool);

        return newParticlePool;
    }

    public GameObject ActiveParticle(string particleName, float inectiveDelay, Transform parentGameObject = null) {
        GameObject instantParticle;
        try { instantParticle = particlePools[particleName].Dequeue(); } 
        catch { throw new System.Exception("파티클 풀 목록에서 '" + particleName + "'에 해당하는 파티클 풀을 찾을 수 없습니다."); }
        
        if(particlePools[particleName].Count<=0) {
            GameObject additionalParticle = Instantiate(instantParticle, instantParticle.transform);
            particlePools[particleName].Enqueue(additionalParticle);
        }

        instantParticle.SetActive(true);
        if(parentGameObject != null) instantParticle.transform.parent = parentGameObject.transform;

        StartCoroutine(InectiveParticle(instantParticle, inectiveDelay, particleName));

        return instantParticle;
    }
    public GameObject ActiveParticle(string particleName, float inectiveDelay, Transform transformToInstantiate, Transform parentGameObject = null) {
        GameObject instantParticle;
        try { instantParticle = particlePools[particleName].Dequeue(); } 
        catch { throw new System.Exception("파티클 풀 목록에서 '" + particleName + "'에 해당하는 파티클 풀을 찾을 수 없습니다."); }

        if(particlePools[particleName].Count<=0) {
            GameObject additionalParticle = Instantiate(instantParticle, instantParticle.transform);
            particlePools[particleName].Enqueue(additionalParticle);
        }

        instantParticle.transform.position = transformToInstantiate.position; // 위치값 조정
        instantParticle.transform.rotation = transformToInstantiate.rotation; // 회전값 조정

        instantParticle.SetActive(true);
        if(parentGameObject != null) instantParticle.transform.parent = parentGameObject.transform;

        StartCoroutine(InectiveParticle(instantParticle, inectiveDelay, particleName));
        
        return instantParticle;
    }
    public GameObject ActiveParticle(string particleName, float inectiveDelay, Vector3 positionToInstantiate, Quaternion rotationToInstantiate, Transform parentGameObject = null) {
        GameObject instantParticle;
        try { instantParticle = particlePools[particleName].Dequeue(); } 
        catch { throw new System.Exception("파티클 풀 목록에서 '" + particleName + "'에 해당하는 파티클 풀을 찾을 수 없습니다."); }
        
        if(particlePools[particleName].Count<=0) {
            GameObject additionalParticle = Instantiate(instantParticle, instantParticle.transform);
            particlePools[particleName].Enqueue(additionalParticle);
        }

        instantParticle.transform.position = positionToInstantiate; // 위치값 조정
        instantParticle.transform.rotation = rotationToInstantiate; // 회전값 조정

        instantParticle.SetActive(true);
        if(parentGameObject != null) instantParticle.transform.parent = parentGameObject.transform;

        StartCoroutine(InectiveParticle(instantParticle, inectiveDelay, particleName));
        
        return instantParticle;
    }
    IEnumerator InectiveParticle(GameObject targetParticle, float inectiveDelay, string particleName) {

        yield return new WaitForSeconds(inectiveDelay);

        targetParticle.SetActive(false);

        if(particlePools.ContainsKey(particleName)) { // 비활성화 할 때 파티클 풀이 제거된 상태라면 본인도 제거
            particlePools[particleName].Enqueue(targetParticle);
            targetParticle.transform.parent = particlePoolGameObjects[particleName].transform;
        } else {
            Destroy(targetParticle);
        }
    }
    public void RemoveParticle(string name, float delayTime = 0) {
        while(particlePools[name].Count > 0) {
            if(delayTime==0) Destroy(particlePools[name].Dequeue());
            else Destroy(particlePools[name].Dequeue(), delayTime);
        }
        particlePools.Remove(name);
    }
}

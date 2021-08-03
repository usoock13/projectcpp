using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ColliderEvent : MonoBehaviour {
    [SerializeField]

    public delegate void CollisionEvent(Collision collision);
    public delegate void TriggerEvent(Collider other);

    public CollisionEvent collisionEnterEvent;
    public TriggerEvent triggerEnterEvent;
    
    public CollisionEvent collisionStayEvent;
    public TriggerEvent triggerStayEvent;
    
    public CollisionEvent collisionExitEvent;
    public TriggerEvent triggerExitEvent;

    void OnCollisionEnter(Collision collisionInfo) {
        if(collisionEnterEvent != null)
            collisionEnterEvent.Invoke(collisionInfo);
    }
    void OnTriggerEnter(Collider other) {
        if(triggerEnterEvent != null)
            triggerEnterEvent.Invoke(other);
    }
    void OnCollisionStay(Collision collisionInfo) {
        if(collisionStayEvent != null)
            collisionStayEvent.Invoke(collisionInfo);
    }
    void OnTriggerStay(Collider other) {
        if(triggerStayEvent != null)
            triggerStayEvent.Invoke(other);
    }
    void OnCollisionExit(Collision collisionInfo) {
        if(collisionExitEvent != null)
            collisionExitEvent.Invoke(collisionInfo);
    }
    void OnTriggerExit(Collider other) {
        if(triggerExitEvent != null)
            triggerExitEvent.Invoke(other);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Overlapshape : MonoBehaviour {

    static public Collider[] OverlapFanshape(Vector3 center, float angle, Vector3 destination, float distance) {
        Collider[] overlapedTarget = Physics.OverlapSphere(center, distance);
        
        List<Collider> targetsInAngle = new List<Collider>();
        foreach(Collider collider in overlapedTarget) {
            if(angle >= Vector3.Angle(destination - center, collider.transform.position - center)) {
                targetsInAngle.Add(collider);
            }
        };
        Collider[] resultColliders = targetsInAngle.ToArray();

        return resultColliders;
    }
    static public Collider[] OverlapFanshape(Vector3 center, float angle, Vector3 destination, float distance, LayerMask layerMask) {
        Collider[] overlapedTarget = Physics.OverlapSphere(center, distance, layerMask);
        
        List<Collider> targetsInAngle = new List<Collider>();
        foreach(Collider collider in overlapedTarget) {
            if(angle >= Vector3.Angle(destination - center, collider.transform.position - center)) {
                targetsInAngle.Add(collider);
            }
        };
        Collider[] resultColliders = targetsInAngle.ToArray();

        return null;
    }
    static public Collider[] OverlapFanshape(Vector3 center, float angle, Vector3 destination, float distance, float t) {

        return null;
    }
    static public Collider[] OverlapFanshape(Vector3 center, float angle, Vector3 destination, float distance, LayerMask layerMask, float t) {
        Vector3 lerpedCenter = center + (center - Vector3.Lerp(center, center + (destination-center).normalized * distance, t));
        Collider[] overlapedTarget = Physics.OverlapSphere(lerpedCenter, distance + (distance * t), layerMask);
        List<Collider> targetsInAngle = new List<Collider>();
        foreach(Collider collider in overlapedTarget) {
            float targetAngle = Vector3.Angle(destination - lerpedCenter, collider.transform.position - lerpedCenter);
            float distanceToCenter = Vector3.Distance(center, lerpedCenter);
            float distanceToTarget = Vector3.Distance(collider.transform.position, lerpedCenter);
            if(targetAngle <= angle && distanceToTarget > distanceToCenter) {
                targetsInAngle.Add(collider);
            }
        };

        Collider[] resultColliders = targetsInAngle.ToArray();

        return resultColliders;
    }
}
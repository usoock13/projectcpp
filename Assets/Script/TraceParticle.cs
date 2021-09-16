using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TraceParticle : MonoBehaviour {

    public Transform centerTransform;
    ParticleSystem[] targetParticleSystems;
    public bool interpolation = false;
    public float interpolationCoefficient = .1f;

    void Start() {
        targetParticleSystems = GetComponentsInChildren<ParticleSystem>();
    }

    void Update() {
        if(!centerTransform) return;
        if(!interpolation) 
            FixedRotate();
        else
            InterpolatedRotate();
    }

    void FixedRotate() {
        float centerRotate = centerTransform.rotation.eulerAngles.y * Mathf.Deg2Rad;
        foreach(ParticleSystem particle in targetParticleSystems) {
            ParticleSystem.MainModule main = particle.main;
            main.startRotationY = -centerRotate;
        }
    }
    void InterpolatedRotate() {
        float centerRotate = centerTransform.rotation.eulerAngles.y * Mathf.Deg2Rad;
        foreach(ParticleSystem particle in targetParticleSystems) {
            ParticleSystem.MainModule main = particle.main;
            main.startRotationY = Mathf.Lerp(main.startRotationY.constant, -centerRotate, interpolationCoefficient);
        }
    }
}

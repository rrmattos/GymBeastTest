using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagBoyController : MonoBehaviour
{
        [SerializeField] private Transform visuals;
        public Transform GetVisuals() => visuals;
        [SerializeField] private Transform hips;
        public Transform GetHips() => hips;
        [SerializeField] private Transform spine;
        public Transform GetSpine() => spine;
        private Rigidbody spineRb;
        private AnimationBehaviour animationBehaviour;
        private Animator animator;
        
        void Awake()
        {
            if (visuals == null) visuals = transform.Find("Visuals");

            if (animator == null) animator = visuals.GetComponent<Animator>();

            if (spine == null)
                Debug.LogWarning($"{this.name}: A propriedade do tipo Transform 'spine' não foi referenciada!");
            else
                spineRb = spine.GetComponent<Rigidbody>();

            if (animationBehaviour == null)
                animationBehaviour = visuals.GetComponent<AnimationBehaviour>();
        }

        private void Start()
        {
            animationBehaviour.UpdateAnimation(AnimationStates.VICTORY);
        }

        private void OnTriggerEnter(Collider _other)
        {
            if (_other.CompareTag("Player"))
            {
                if (!animator.enabled) return;
                
                animator.enabled = false;
                GetComponent<Collider>().enabled = false;
            }
        }

        public void TakeKnockBack(Vector3 _direction)
        {
            spineRb.AddForce(_direction, ForceMode.Impulse);
        }
}

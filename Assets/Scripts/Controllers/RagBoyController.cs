using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public AnimationBehaviour GetAnimationBehaviour() => animationBehaviour;

    private Transform player;

    private bool isStacking = false;

    public bool IsKnockedOut { get; set; }= false;


    private void OnEnable()
    {
        PlayerController.OnSetPlayerReference += GetPlayerReference;
    }

    void Awake()
    {
        if (visuals == null) visuals = transform.Find("Visuals");
        
        if (animationBehaviour == null)
            animationBehaviour = visuals.GetComponent<AnimationBehaviour>();

        if (spine == null)
            Debug.LogWarning($"{this.name}: A propriedade do tipo Transform 'spine' não foi referenciada!");
        else
            spineRb = spine.GetComponent<Rigidbody>();
    }

    private void Start()
    {
        animationBehaviour.UpdateAnimation(AnimationStates.VICTORY);
    }

    private void OnTriggerEnter(Collider _other)
    {
        if (_other.CompareTag("Player"))
        {
            if (!animationBehaviour.GetAnimator().enabled) return;

            animationBehaviour.GetAnimator().enabled = false;
            GetComponent<Collider>().enabled = false;
            IsKnockedOut = true;
            
            StartCoroutine(TimerActivateHipsCollider());
        }
    }
    
    public void TakeKnockBack(Vector3 _direction)
    {
        spineRb.AddForce(_direction, ForceMode.Impulse);
    }

    public void CallTimeToStack()
    {
        if (isStacking) return;

        isStacking = true;
        StartCoroutine(TimeToStack());
    }
    
    private IEnumerator TimeToStack()
    {
        LayerChanger.ChangeLayerRecursively(gameObject, 3);
        
        hips.SetParent(null);

        List<Rigidbody> rigidBodies = new()
        {
            hips.GetComponent<Rigidbody>(),
            spineRb
        };

        foreach (Rigidbody rb in rigidBodies)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }
        
        transform.position = hips.position;
        hips.SetParent(visuals);
        
        player.GetComponent<PlayerController>().GetStackController().AddToStack(transform);

        yield return new WaitForSeconds(1);
        
        isStacking = false;
    }

    private IEnumerator TimerActivateHipsCollider()
    {
        yield return new WaitForSeconds(2);
        
        hips.GetComponent<CapsuleCollider>().enabled = true;
    }

    private void GetPlayerReference(Transform _player)
    {
        player = _player;
        if (player != null) PlayerController.OnSetPlayerReference -= GetPlayerReference;
    }
}

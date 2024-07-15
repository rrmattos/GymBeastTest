using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static event Action<Transform> OnSetPlayerReference;
    
    [SerializeField] private Transform visuals;
    public Transform GetVisuals() => visuals;
    private AnimationBehaviour animationBehaviour;
    public AnimationBehaviour GetAnimationBehaviour() => animationBehaviour;
    private TouchController touchController;
    private StackController stackController;
    
    private PlayerController(){}
    
    void Awake()
    {
        if (visuals == null)
            visuals = transform.Find("Visuals");
        
        if (animationBehaviour == null)
            animationBehaviour = visuals.GetComponent<AnimationBehaviour>();
        
        if (touchController == null)
            touchController = GetComponent<TouchController>();
        
        if (stackController == null)
            stackController = GetComponent<StackController>();
    }

    private void Update()
    {
        OnSetPlayerReference?.Invoke(transform);

        if (animationBehaviour != null && touchController != null)
        {
            if (touchController.isMoveBlocked) return;
            
            Enum currentState = animationBehaviour.CurrentState;
            if (currentState != (Enum)touchController.MoveState) ;
                animationBehaviour.UpdateAnimation(touchController.MoveState);
                
            if (currentState != null && currentState.HasFlag(AnimationStates.RUN))
            {
                visuals.localRotation = touchController.VisualRotation;
            }
        }
    }

    private void OnTriggerEnter(Collider _other)
    {
        if (_other.CompareTag("RagBoy") && !touchController.isMoveBlocked)
        {
            touchController.IsMoveBlocked(true);
            Punch(_other);
        }
    }

    private async void Punch(Collider _other)
    {
        await Task.Run(() =>
        {
            UnityMainThreadDispatcher.Enqueue(() =>
            {
                animationBehaviour.UpdateAnimationFreeze(AnimationStates.PUNCH);
            });
        });
        
        _other.transform.GetComponent<RagBoyController>().TakeKnockBack(transform.forward * 100);

        StartCoroutine(TimeToStack(_other));
    }

    private IEnumerator TimeToStack(Collider _other)
    {
        AnimatorStateInfo stateInfo = animationBehaviour.GetAnimator().GetCurrentAnimatorStateInfo(0);
        yield return new WaitForSeconds(stateInfo.length * (stateInfo.speed * 0.1f));

        //Transform ragBoySpine = _other.GetComponent<RagBoyController>().GetSpine();
        Transform ragBoyHips = _other.GetComponent<RagBoyController>().GetHips();
        ragBoyHips.SetParent(null);
        
        List<Rigidbody> rigidBodies = _other.GetComponentsInChildren<Rigidbody>().Where(rb =>
        {
            string[] names = { "Hips", "Spine" };
            return names.Any(name => rb.name.Contains(name));
        }).ToList();

        foreach (Rigidbody rb in rigidBodies)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        ragBoyHips.GetComponent<Rigidbody>().isKinematic = true;
        _other.transform.position = ragBoyHips.position;
        Debug.Log($"{_other.transform.position} - {ragBoyHips.position}");
        ragBoyHips.SetParent(_other.transform);
        
        stackController.AddToStack(_other.transform);
        LayerChanger.ChangeLayerRecursively(_other.gameObject, 3);
        Destroy(_other.GetComponent<Collider>());;

        touchController.IsMoveBlocked(false);
    }
}

public static class LayerChanger
{
    public static void ChangeLayerRecursively(GameObject _obj, int _newLayer)
    {
        if (_obj == null) return;

        _obj.layer = _newLayer;

        foreach (Transform child in _obj.transform)
        {
            ChangeLayerRecursively(child.gameObject, _newLayer);
        }
    }
}
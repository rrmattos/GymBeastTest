using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimationBehaviour : MonoBehaviour
{
    [SerializeField] private Animator animator;
    public Animator GetAnimator() => animator;
    public Enum CurrentState { get; set; }
    public bool isFreezingAnimationUpdate { get; private set; } = false;
    private string animationName;

    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
    }

    public void UpdateAnimation(Enum _state, bool _freezeAnimationUpdate = false)
    {
        if (isFreezingAnimationUpdate) return;

        if (_freezeAnimationUpdate)
        {
            StartCoroutine(TimerFreezingChanges());
            return;
        }

        if (CurrentState == _state) return;

        CurrentState = _state;

        if (_state.GetType() == typeof(AnimationStates)) SetAnimationName((AnimationStates)_state);

        animator.Play(animationName);
    }

    public async void UpdateAnimationFreeze(Enum _state)
    {
        isFreezingAnimationUpdate = true;

        CurrentState = _state;

        if (_state.GetType() == typeof(AnimationStates)) SetAnimationName((AnimationStates)_state);

        animator.Play(animationName);

        await Task.Run(() =>
        {
            UnityMainThreadDispatcher.Enqueue(() =>
            {
                StartCoroutine(TimerFreezingChanges());
            });
        });

        await Task.Yield();
    }

    private void SetAnimationName(AnimationStates _state)
    {
        switch (_state)
        {
            case AnimationStates.IDLE:
                animationName = "Idle";
                break;

            case AnimationStates.WALK:
                animationName = "Walk";
                break;

            case AnimationStates.RUN:
                animationName = "SlowRun";
                animator.applyRootMotion = false;
                break;

            case AnimationStates.PUNCH:
                animationName = "HookPunch";
                animator.applyRootMotion = true;
                animator.Rebind();
                break;

            case AnimationStates.DEATH:
                animationName = "Death";
                break;

            case AnimationStates.VICTORY:
                animationName = "Victory";
                break;

            case AnimationStates.BLOW_KISS:
                animationName = "BlowKiss";
                break;

            case AnimationStates.SHAKE_HANDS:
                animationName = "ShakingHands";
                break;
        }
    }

    private IEnumerator TimerFreezingChanges()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        yield return new WaitForSeconds(stateInfo.length * (stateInfo.speed * 0.1f));

        isFreezingAnimationUpdate = false;
    }
}

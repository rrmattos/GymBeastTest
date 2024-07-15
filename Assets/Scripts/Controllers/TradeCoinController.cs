using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class TradeCoinController : MonoBehaviour
{
    [SerializeField] private Animator doorSecurityAnimator;
    [SerializeField] private Animator tradePointAnimator;
    [SerializeField] private Transform tradeAnchorPoint;
    [SerializeField] private Transform outsideAnchor;
    [SerializeField] private float playerAdjustTime = 0.5f;
    [SerializeField] private float bodySliderTime = 2f;
    [SerializeField] private AudioClip coinAudio;
    private AudioSource audioSource;
    private TextMeshProUGUI textCoin;
    private Transform wallsParent;

    private StackController stackController;
    private Transform stackAnchor;
    private bool isPlayerGettinPosition = false;
    private int totalCoins = 0;

    private void OnEnable()
    {
        UIController.OnSetTextCoins += GetTextCoinReference;
        SFXAudioObserver.OnPlaySFX += GetSFXAudioReference;
        WallsObserver.OnSetWalls += GetWallsReference;
    }

    private void OnTriggerEnter(Collider _other)
    {
        if (_other.transform.TryGetComponent(out PlayerController playerController))
        {
            if (stackController == null) stackController = _other.GetComponent<StackController>();
            if (stackAnchor == null) stackAnchor = stackController.GetStackAnchor();
            
            wallsParent.GetComponent<Animator>().Play("FadeIn");
            
            if (isPlayerGettinPosition) return;
            if (stackAnchor != null && stackAnchor.childCount == 0) return;
            Debug.Log(stackAnchor.childCount);
            
            isPlayerGettinPosition = true;

            StartCoroutine(TimerAdjustPlayerPosition(_other.transform, playerController));
        }
    }

    private void OnTriggerExit(Collider _other)
    {
        if (_other.CompareTag("Player"))
        {
            wallsParent.GetComponent<Animator>().Play("FadeOut");
        }
    }

    private IEnumerator TimerAdjustPlayerPosition(Transform _other, PlayerController _playerController)
    {
        TouchController touchController = _other.GetComponent<TouchController>();
        touchController.enabled = false;
        Transform playerVisuals = _playerController.GetVisuals();
        _playerController.enabled = false;
        
        #if UNITY_EDITOR
        Selection.activeObject = null;
        #endif

        tradePointAnimator.enabled = false;
        
        Vector3 startPos = _other.position;
        Quaternion startRot = playerVisuals.rotation;
        Vector3 endPos = tradeAnchorPoint.position;
        Quaternion endRot = tradeAnchorPoint.rotation;

        float elapsedTime = 0f;

        while (elapsedTime < playerAdjustTime)
        {
            _other.position = Vector3.Lerp(startPos, endPos, elapsedTime / playerAdjustTime);
            playerVisuals.rotation = Quaternion.Slerp(startRot, endRot, elapsedTime / playerAdjustTime);

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        _other.position = endPos;
        playerVisuals.rotation = endRot;
        
        doorSecurityAnimator.Play("Clapping2");

        List<Transform> stackBodies = new();
        foreach (Transform body in stackAnchor)
        {
            stackBodies.Add(body);
        }
    
        foreach (Transform body in stackBodies)
        {
            elapsedTime = 0;
            startPos = body.position;
            endPos = outsideAnchor.position;
        
            stackController.RemoveFromStack(body);
            body.SetParent(null);
        
            while (elapsedTime < bodySliderTime)
            {
                body.position = Vector3.Lerp(startPos, endPos, elapsedTime / bodySliderTime);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        
            body.position = endPos; 
            LayerChanger.ChangeLayerRecursively(body.gameObject, 0);
            //Rigidbody rb = body.GetComponent<Rigidbody>();
            //rb.useGravity = true;
            //rb.isKinematic = false;

            audioSource.clip = coinAudio;
            audioSource.Play();
            totalCoins++;
            textCoin.text = $"x{totalCoins.ToString()}";
        }

        _playerController.GetAnimationBehaviour().GetAnimator().applyRootMotion = true;
        
        _playerController.GetAnimationBehaviour().UpdateAnimationFreeze(AnimationStates.SHAKE_HANDS);
        doorSecurityAnimator.Play("ShakingHands");
        yield return new WaitForSeconds(4);
        doorSecurityAnimator.Play("Idle");
        _playerController.GetAnimationBehaviour().GetAnimator().applyRootMotion = false;
        
        Debug.Log("cabou tempo");
        isPlayerGettinPosition = false;
        
        touchController.enabled = true;
        _playerController.enabled = true;
        tradePointAnimator.enabled = true;
    }

    private void GetTextCoinReference(TextMeshProUGUI _textCoin)
    {
        textCoin = _textCoin; 
        if(textCoin != null) UIController.OnSetTextCoins -= GetTextCoinReference;
    }
    
    private void GetSFXAudioReference(AudioSource _audioSource)
    {
        audioSource = _audioSource;
        if(_audioSource != null) SFXAudioObserver.OnPlaySFX -= GetSFXAudioReference;
    }
    
    private void GetWallsReference(Transform _walls)
    {
        wallsParent = _walls;
        if (wallsParent != null) WallsObserver.OnSetWalls -= GetWallsReference;
    }
}

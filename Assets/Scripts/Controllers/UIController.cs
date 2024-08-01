using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public static event Action<TextMeshProUGUI> OnSetTextCoins;

    [SerializeField] private CanvasGroup storeGroup;
    [SerializeField] private TextMeshProUGUI textCoin;
    [SerializeField] private TextMeshProUGUI textBtnHead;
    [SerializeField] private TextMeshProUGUI textBtnFace;
    [SerializeField] private TextMeshProUGUI textBtnBody;
    [SerializeField] private TextMeshProUGUI textStackCapacity;
    [SerializeField] private List<SCPTB_Skin> headSkins = new();
    [SerializeField] private List<SCPTB_Skin> faceSkins = new();
    [SerializeField] private List<SCPTB_Skin> bodySkins = new();
    [SerializeField] private AudioClip coinSound;
    private Transform player;
    private Transform playerVisuals;
    private Transform playerHead;
    private Transform playerFace;
    private Transform playerBody;
    private int headCounter = 0;
    private int faceCounter = 0;
    private int bodyCounter = 0;
    private StackController stackController;
    private AudioSource audioSFX;

    private UIController() { }

    private void OnEnable()
    {
        PlayerController.OnSetPlayerReference += GetPlayerReference;
        SFXAudioObserver.OnPlaySFX += GetSFXAudioSource;
    }

    void Update()
    {
        OnSetTextCoins?.Invoke(textCoin);
    }

    public void OpenCloseStore()
    {
        if (storeGroup.alpha == 0)
        {
            storeGroup.alpha = 1;
            storeGroup.interactable = true;
            storeGroup.blocksRaycasts = true;

            textStackCapacity.text = $"{stackController.StackCapacity}";

            UpdateAllSkinsState();
        }
        else
        {
            storeGroup.alpha = 0;
            storeGroup.interactable = false;
            storeGroup.blocksRaycasts = false;

            ClearChildrensFromList(playerHead);
            ClearChildrensFromList(playerFace);

            #region --- Equipa as skins marcadas como "EQUIPED" ---

            UpdateEquipedSkins(headSkins, playerHead);
            UpdateEquipedSkins(faceSkins, playerFace);
            UpdateEquipedSkins(bodySkins, playerBody);

            #endregion
            //
            // headCounter = 0;
            // faceCounter = 0;
            // bodyCounter = 0;
        }
    }

    public void ChangeHeadSkin(int _direction)
    {
        if (headCounter + _direction < 0 || headCounter + _direction > headSkins.Count - 1) return;

        headCounter += _direction;

        if (playerHead.childCount > 0) ClearChildrensFromList(playerHead);

        if (headSkins[headCounter] != null && headSkins[headCounter].SkinPrefab != null)
        {
            GameObject newSkin = Instantiate(headSkins[headCounter].SkinPrefab, playerHead);
            newSkin.layer = 6;
        }

        UpdateSkinState(headSkins, headCounter, textBtnHead, 3);
    }

    public void ChangeFaceSkin(int _direction)
    {
        if (faceCounter + _direction < 0 || faceCounter + _direction > faceSkins.Count - 1) return;

        faceCounter += _direction;

        if (playerFace.childCount > 0) ClearChildrensFromList(playerFace);

        if (faceSkins[faceCounter] != null && faceSkins[faceCounter].SkinPrefab != null)
        {
            GameObject newSkin = Instantiate(faceSkins[faceCounter].SkinPrefab, playerFace);
            newSkin.layer = 6;
        }

        UpdateSkinState(faceSkins, faceCounter, textBtnFace, 2);
    }

    public void ChangeBodySkin(int _direction)
    {
        if (bodyCounter + _direction < 0 || bodyCounter + _direction > bodySkins.Count - 1) return;

        bodyCounter += _direction;

        if (bodySkins[bodyCounter] != null && bodySkins[bodyCounter].SkinMaterial != null)
        {
            SkinnedMeshRenderer skinnedMeshRenderer = playerBody.GetComponent<SkinnedMeshRenderer>();
            skinnedMeshRenderer.material = bodySkins[bodyCounter].SkinMaterial;
        }

        UpdateSkinState(bodySkins, bodyCounter, textBtnBody, 5);
    }

    public void IncreaseStackCapacity()
    {
        int currentCoins = Int32.Parse(textCoin.text.Replace("x", ""));

        if (currentCoins < 5) return;

        stackController.StackCapacity++;
        textStackCapacity.text = stackController.StackCapacity.ToString();
        textCoin.text = $"x{currentCoins - 5}";

        audioSFX.clip = coinSound;
        audioSFX.Play();
    }

    public void BuyEquipSkin(int _skinType)
    {
        switch (_skinType)
        {
            case 0: //--- Head ---
                UpdateSkinState(headSkins, headCounter, textBtnHead, 3, true);
                break;

            case 1: //--- Face ---
                UpdateSkinState(faceSkins, faceCounter, textBtnFace, 2, true);
                break;

            case 2: //--- Body ---
                UpdateSkinState(bodySkins, bodyCounter, textBtnBody, 5, true);
                break;
        }
    }

    private void UpdateSkinState(List<SCPTB_Skin> _skinList, int _counter, TextMeshProUGUI _textMesh, int _price, bool _isBuyingOrEquip = false)
    {
        Transform imageCoin = _textMesh.transform.parent.Find("ImageCoin");

        switch (_skinList[_counter].storeState)
        {
            case StoreStates.NONE:
                if (!_isBuyingOrEquip)
                {
                    if (!imageCoin.gameObject.activeSelf) imageCoin.gameObject.SetActive(true);

                    _textMesh.text = $"x{_price}";
                    return;
                }

                int currentCoins = Int32.Parse(textCoin.text.Replace("x", ""));

                if (currentCoins < _price) return;

                textCoin.text = $"x{currentCoins - _price}";
                _skinList[_counter].storeState = StoreStates.PURCHASED;
                _textMesh.text = "Equip";
                imageCoin.gameObject.SetActive(false);
                audioSFX.clip = coinSound;
                audioSFX.Play();
                break;

            case StoreStates.PURCHASED:
                if (!_isBuyingOrEquip)
                {
                    if (imageCoin.gameObject.activeSelf) imageCoin.gameObject.SetActive(false);
                    _textMesh.text = "Equip";
                    return;
                }

                foreach (SCPTB_Skin skin in _skinList)
                {
                    if (skin == null) continue;
                    if (skin.storeState == StoreStates.EQUIPED) skin.storeState = StoreStates.PURCHASED;
                }
                _skinList[_counter].storeState = StoreStates.EQUIPED;
                _textMesh.text = "Unequip";
                break;

            case StoreStates.EQUIPED:
                if (!_isBuyingOrEquip)
                {
                    if (imageCoin.gameObject.activeSelf) imageCoin.gameObject.SetActive(false);
                    _textMesh.text = "Unequip";
                    return;
                }

                if (_skinList[_counter].name.Contains("None") || _skinList[_counter].name.Contains("SkinBody1")) return;

                _skinList[_counter].storeState = StoreStates.PURCHASED;
                _textMesh.text = "Equip";
                break;
        }
    }

    private void UpdateAllSkinsState(bool _isBuyingOrEquip = false)
    {
        UpdateSkinState(headSkins, headCounter, textBtnHead, 3, _isBuyingOrEquip);

        UpdateSkinState(faceSkins, faceCounter, textBtnFace, 2, _isBuyingOrEquip);

        UpdateSkinState(bodySkins, bodyCounter, textBtnBody, 5, _isBuyingOrEquip);
    }

    private void UpdateEquipedSkins(List<SCPTB_Skin> _skinList, Transform _anchor)
    {
        foreach (SCPTB_Skin skin in _skinList)
        {
            if (skin == null || skin.name.Contains("None")) continue;

            if (skin.storeState != StoreStates.EQUIPED) continue;

            if (skin.SkinMaterial != null)
            {
                SkinnedMeshRenderer skinnedMeshRenderer = playerBody.GetComponent<SkinnedMeshRenderer>();
                skinnedMeshRenderer.material = skin.SkinMaterial;
            }
            else
            {
                GameObject newSkin = Instantiate(skin.SkinPrefab, _anchor);
                newSkin.layer = 6;
            }
        }
    }

    private void ClearChildrensFromList(Transform _target)
    {
        foreach (Transform child in _target)
        {
            child.gameObject.SetActive(false);
            Destroy(child.gameObject);
        }
    }

    private void GetSFXAudioSource(AudioSource _audio)
    {
        audioSFX = _audio;
        if (audioSFX != null) SFXAudioObserver.OnPlaySFX -= GetSFXAudioSource;
    }

    private void GetPlayerReference(Transform _player)
    {
        player = _player;
        if (player != null) PlayerController.OnSetPlayerReference -= GetPlayerReference;
        SetPlayerReferences();
    }

    private void SetPlayerReferences()
    {
        playerVisuals = player.GetComponent<PlayerController>().GetVisuals();
        playerHead = playerVisuals.GetComponentsInChildren<Transform>().FirstOrDefault(child =>
            child.CompareTag("PlayerHead")
        );
        playerFace = playerVisuals.GetComponentsInChildren<Transform>().FirstOrDefault(child =>
            child.CompareTag("PlayerFace")
        );
        playerBody = playerVisuals.GetComponentsInChildren<Transform>().FirstOrDefault(child =>
            child.CompareTag("PlayerBody")
        );
        stackController = player.GetComponent<StackController>();
    }
}

public enum StoreStates
{
    NONE,
    PURCHASED,
    EQUIPED,
}
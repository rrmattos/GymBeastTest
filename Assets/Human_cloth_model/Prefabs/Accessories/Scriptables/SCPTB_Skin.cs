using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/New Skin", fileName = "NewSkin"/*, order = 1*/)]
public class SCPTB_Skin : ScriptableObject
{
    public GameObject SkinPrefab;
    public Material SkinMaterial;
    public StoreStates storeState = StoreStates.NONE;

    private void OnEnable()
    {
        if(name.Contains("None") || name == "BodySkin1")
            storeState = StoreStates.EQUIPED;
        else
            storeState = StoreStates.NONE;
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BodyRandomizerObserver : MonoBehaviour
{
    public static event Action<BodyRandomizerObserver> OnSetReference;
    
    [SerializeField] private List<Material> bodySkins = new();
    [SerializeField] private List<GameObject> headSkins = new();
    [SerializeField] private List<GameObject> faceSkins = new();
    [SerializeField] private List<GameObject> chestSkins = new();
    [SerializeField] private List<Material> colorPalets = new();
    private int random;

    private void Update()
    {
        OnSetReference?.Invoke(this);
    }

    public void RandomizeBody(Transform _body, Transform _head, Transform _chest, Transform _shirt, Transform _pants)
    {
        _body.GetComponent<SkinnedMeshRenderer>().material = bodySkins[Random.Range(0, bodySkins.Count)];

        random = Random.Range(0, headSkins.Count);
        if(random > 0) Instantiate(headSkins[random], _head);
        
        random = Random.Range(0, faceSkins.Count);
        if(random > 0) Instantiate(faceSkins[random], _head);
        
        random = Random.Range(0, chestSkins.Count);
        if(random > 0) Instantiate(chestSkins[random], _chest);

        if (_shirt != null)
        {
            random = Random.Range(0, colorPalets.Count);
            _shirt.GetComponent<SkinnedMeshRenderer>().material = colorPalets[Random.Range(0, colorPalets.Count)];
        }
        
        if (_pants != null)
        {
            random = Random.Range(0, colorPalets.Count);
            _pants.GetComponent<SkinnedMeshRenderer>().material = colorPalets[Random.Range(0, colorPalets.Count)];
        }
    }
}

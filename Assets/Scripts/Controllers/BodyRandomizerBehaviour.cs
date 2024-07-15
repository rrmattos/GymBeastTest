using UnityEngine;
public class BodyRandomizerBehaviour : MonoBehaviour
{
    [SerializeField] private Transform body;
    [SerializeField] private Transform head;
    [SerializeField] private Transform chest;
    [SerializeField] private Transform shirt = null;
    [SerializeField] private Transform pants = null;
    private BodyRandomizerObserver bodyRandomizer;
    private bool isRandomized = false;

    private void OnEnable()
    {
        BodyRandomizerObserver.OnSetReference += GetBodyRandomizerObserver;
    }

    private void Update()
    {
        if (!isRandomized) return;
        
        Destroy(GetComponent<BodyRandomizerBehaviour>());
    }

    private void GetBodyRandomizerObserver(BodyRandomizerObserver _observer)
    {
        bodyRandomizer = _observer;

        if (bodyRandomizer != null)
        {
            BodyRandomizerObserver.OnSetReference -= GetBodyRandomizerObserver;
            bodyRandomizer.RandomizeBody(body, head, chest, shirt, pants);
            isRandomized = true;
        }
    }
}

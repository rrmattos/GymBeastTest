using UnityEngine;

public class RagBoyHipsController : MonoBehaviour
{
    [SerializeField] private RagBoyController ragBoyController;
    private StackController stackController;
    private bool isStacked = false;

    private void Awake()
    {
        if (ragBoyController == null) ragBoyController = transform.GetComponentInParent<RagBoyController>();
    }

    private void OnTriggerEnter(Collider _other)
    {
        if (isStacked) return;
        
        if (!ragBoyController.IsKnockedOut) return;
        
        if (_other.CompareTag("Player") )
        {
            if (stackController == null) stackController = _other.GetComponent<PlayerController>().GetStackController();

            if (stackController.StackCapacity == stackController.GetStackAnchor().childCount) return;
            
            isStacked = true;
            ragBoyController.CallTimeToStack();
        }
    }
}

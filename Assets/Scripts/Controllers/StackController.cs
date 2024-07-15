using System.Collections.Generic;
using UnityEngine;

public class StackController : MonoBehaviour
{
    [SerializeField] private Transform stackAnchor;
    public Transform GetStackAnchor() => stackAnchor;
    [SerializeField] private float stackSpacing = 1f;
    [SerializeField] private float stackVelocity = 5f;
    [SerializeField] private float inertiaFactor = 0.1f;
    public int StackCapacity { get; set; } = 3;

    private List<Transform> stack = new();
    
    private StackController(){}

    private void Update()
    {
        if (stack.Count > 0)
        {
            UpdateStackPositions();
        }
    }

    private void UpdateStackPositions()
    {
        Vector3 previousPosition = stackAnchor.position;
        Quaternion previousRotation = stackAnchor.rotation;

        for (int i = 0; i < stack.Count; i++)
        {
            Transform ragboy = stack[i];
            Vector3 targetPosition = previousPosition + stackAnchor.up * stackSpacing;
            ragboy.position = Vector3.Lerp(ragboy.position, targetPosition, stackVelocity);
            ragboy.rotation = Quaternion.Slerp(ragboy.rotation, previousRotation, inertiaFactor);

            previousPosition = ragboy.position;
            previousRotation = ragboy.rotation;
        }
    }

    public void AddToStack(Transform _ragboy)
    {
        _ragboy.SetParent(null);
        stack.Add(_ragboy);
        _ragboy.SetParent(stackAnchor);
    }

    public void RemoveFromStack(Transform _ragboy)
    {
        stack.Remove(_ragboy);
    }
}
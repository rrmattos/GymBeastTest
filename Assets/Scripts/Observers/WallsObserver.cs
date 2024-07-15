using System;
using UnityEngine;

public class WallsObserver : MonoBehaviour
{
    public static event Action<Transform> OnSetWalls;

    private WallsObserver(){}

    private void Update()
    {
        OnSetWalls?.Invoke(transform);
    }
}

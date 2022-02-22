using UnityEngine;

public class TransformManager : MonoBehaviour, ITransformManager {
    [SerializeField]
    private Transform goTransform;
    public Transform GoTransform => goTransform;
}
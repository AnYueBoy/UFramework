using UnityEngine;

namespace UFramework
{
    public interface INodeManager
    {
        RectTransform LowerRoot { get; }
        RectTransform TopRoot { get; }
        RectTransform TipRoot { get; }
    }
}
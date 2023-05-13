using UnityEngine;

namespace UFramework.GameCommon
{
    public interface INodeManager
    {
        RectTransform LowerRoot { get;  }
        RectTransform TopRoot { get; }
        RectTransform TipRoot { get;  }
    }
}
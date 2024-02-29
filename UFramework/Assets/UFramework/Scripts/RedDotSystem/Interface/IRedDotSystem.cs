using System;
using System.Text;

namespace UFramework
{
    public interface IRedDotSystem
    {
        void Init();

        ITreeNode Root { get; }

        char SplitChar { get; }

        Action NodeNumChangeCallback { get; }

        Action<ITreeNode, int> NodeValueChangeCallback { get; }

        StringBuilder CacheStringBuilder { get; }

        ITreeNode AddListener(string path, Action<int> callback);

        void RemoveListener(string path, Action<int> callback);

        void RemoveAllListener(string path);

        void ChangeValue(string path, int newValue);

        int GetValue(string path);

        ITreeNode GetTreeNode(string path);

        bool RemoveTreeNode(string path);

        void RemoveAllTreeNode();

        void LocalUpdate();

        void MakeDirtyNode(ITreeNode node);

        void RegisterTrigger(params IRedDotTrigger[] triggerConditions);
    }
}
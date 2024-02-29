using System;
using System.Collections.Generic;

namespace UFramework
{
    public interface ITreeNode
    {
        ITreeNode Parent { get; }

        string Name { get; }

        string FullPath { get; }

        int Value { get; }

        Dictionary<RangeString, ITreeNode>.ValueCollection Children { get; }

        public int ChildrenCount { get; }

        void AddListener(Action<int> callback);

        void RemoveListener(Action<int> callback);

        void RemoveAllListener();

        void ChangedValue(int newValue);

        void ChangedValue();

        ITreeNode AddChild(RangeString key);

        bool RemoveChild(RangeString key);

        void RemoveAllChild();
        
        ITreeNode GetChild(RangeString key);

        ITreeNode GetOrAddChild(RangeString key);
    }
}
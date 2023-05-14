using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UFramework.GameCommon
{
    public class RuntimeComponent : MonoBehaviour
    {
        private RectTransform _rectTrans;

        public RectTransform RectTrans => _rectTrans;

        public void Init()
        {
            _rectTrans = GetComponent<RectTransform>();
        }

#if UNITY_EDITOR
        [SerializeField] private string[] _alias;
#endif

        [SerializeField] private Object[] _objects;
        private int _index;

        private void Reset()
        {
            _index = 0;
        }

        public object CurrentObject()
        {
            if (_index + 1 <= _objects.Length)
            {
                return _objects[_index++];
            }

            return null;
        }

        public Object[] Objects => _objects;

        private ItemInfo NewItemInfo(Object ob, ItemInfo info = null)
        {
            if (info == null)
            {
                info = new ItemInfo();
            }

            if (ob != null)
            {
                var list = new List<string>();
                var types = new List<Type>();
                if (ob is GameObject)
                {
                    var go = ob as GameObject;
                    info.index = AddComponentsTypes(go, null, list, types);
                    info.gameObject = go;
                }
                else if (ob is Component)
                {
                    var co = ob as Component;
                    info.index = AddComponentsTypes(co.gameObject, co, list, types);
                    info.gameObject = co.gameObject;
                }
                else
                {
                    var t = ob.GetType();
                    list.Add(t.Name);
                    types.Add(t);
                }

                info.types = list.ToArray();
                info.components = types.ToArray();
            }
            else
            {
                info.types = new[] {"none"};
                info.components = new Type[] {null};
            }

            return info;
        }

        private int AddComponentsTypes(GameObject go, Component co, List<string> list, List<Type> types, int index = 0)
        {
            //GameObject引用比较特殊，需要进行特殊处理
            list.Add("GameObject");
            types.Add(typeof(GameObject));
            var componentList = go.GetComponents(typeof(Component));
            foreach (var component in componentList)
            {
                list.Add(component.GetType().Name);
                types.Add(component.GetType());
            }

            if (co != null)
            {
                var t = co.GetType().Name;
                return list.IndexOf(t);
            }

            return index;
        }
    }

    public class ItemInfo
    {
        public string[] types = { };
        public Type[] components = { };
        public int index = 0;
        public GameObject gameObject;

        public Object GetValue()
        {
            var t = components[index];
            if (t == typeof(GameObject))
            {
                return gameObject;
            }

            return gameObject.GetComponent(t);
        }
    }
}
/*
 * @Author: l hy 
 * @Date: 2020-03-07 16:37:25 
 * @Description: 界面基类 
 * @Last Modified by: l hy
 * @Last Modified time: 2020-12-21 16:42:57
 */
namespace UFramework.GameCommon {
    using UnityEngine;

    public class BaseUI : MonoBehaviour {

        public virtual void OnShow (params object[] args) { }
    }
}
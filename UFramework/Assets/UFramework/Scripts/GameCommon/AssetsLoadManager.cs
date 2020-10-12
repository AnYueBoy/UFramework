/*
 * @Author: l hy 
 * @Date: 2020-10-10 06:54:10 
 * @Description: 资源加载管理 
 * @Last Modified by: l hy
 * @Last Modified time: 2020-10-10 07:12:02
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetsLoadManager {

    public static T loadAssets<T> (string assetsUrl) where T : Object {
        //  load json prefab or audioclip 
        return Resources.Load<T> (assetsUrl);
    }
}
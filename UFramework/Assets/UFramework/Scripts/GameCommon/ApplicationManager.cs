/*
 * @Author: l hy 
 * @Date: 2021-01-25 15:34:12 
 * @Description: 程序入口
 */
namespace UFrameWork.GameCommon {
    using UnityEngine;

    public delegate void applicationCallback ();
    public class ApplicationManager : MonoBehaviour {

        #region 程序生命周期
        public static applicationCallback applicationQuit = null;

        #endregion
    }
}
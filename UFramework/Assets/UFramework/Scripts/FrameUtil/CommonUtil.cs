/*
 * @Author: l hy 
 * @Date: 2019-12-16 23:05:55 
 * @Description: 工具类
 * @Last Modified by: l hy
 * @Last Modified time: 2020-12-21 16:44:12
 */

namespace UFramework.FrameUtil {

    using System.Collections.Generic;
    using UnityEngine;

    public static class CommonUtil {

        private static float getAspect () {
            float aspect = (float) Screen.width / Screen.height;
            return aspect;
        }

        /// <summary>
        /// 是否横屏
        /// </summary>
        /// <returns></returns>
        public static bool isLandscape () {
            float aspect = getAspect ();
            if (aspect > 1) {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 是否是IPad分辨率
        /// </summary>
        /// <returns></returns>
        public static bool isPadResoluation () {
            return targetValue (4 / 3.0f);
        }

        /// <summary>
        /// 是否是IPhone分辨率
        /// </summary>
        /// <returns></returns>
        public static bool isPhone () {
            return targetValue (16 / 9.0f);
        }

        private static bool targetValue (float value) {
            float offset = 0.05f;
            float aspect = getAspect ();
            if (aspect > value - offset && aspect < value + offset) {
                return true;
            }
            return false;
        }

        #region 归并排序

        /// <summary>
        /// 归并排序
        /// </summary>
        /// <param name="targetArray">目标数组</param>
        public static void mergeSort (this List<double> targetArray) {
            mSort (targetArray, 0, targetArray.Count - 1);
        }

        private static void mSort (List<double> array, int l, int r) {
            if (l >= r) {
                return;
            }

            // right move calculate
            int mid = (l + r) >> 1;
            mSort (array, l, mid);
            mSort (array, mid + 1, r);
            merge (array, l, mid, r);
        }

        private static void merge (List<double> array, int l, int mid, int r) {
            // List<T> 避免ArrayList 频繁的装箱拆箱操作
            // List 需要有元素时才能使用下标进行访问
            // List<double> temp = new List<double> (r - l + 1);
            double[] temp = new double[r - l + 1];
            int leftPointer = l;
            int rightPointer = mid + 1;
            int index = 0;
            while (leftPointer <= mid && rightPointer <= r) {
                temp[index++] = array[leftPointer] <= array[rightPointer] ? array[leftPointer++] : array[rightPointer++];
            }

            while (leftPointer <= mid) {
                temp[index++] = array[leftPointer++];
            }

            while (rightPointer <= r) {
                temp[index++] = array[rightPointer++];
            }

            for (int i = 0; i < temp.Length; i++) {
                array[l + i] = temp[i];
            }
        }

        #endregion

        #region 快速排序
        private static void qSort (List<double> array, int low, int high) {
            if (low >= high) {
                return;
            }

            int i = low, j = high;
            double key = array[i];

            while (i < j) {
                while (i < j && array[j] >= key) j--;
                array[i] = array[j];
                while (i < j && array[i] <= key) i++;
                array[j] = array[i];
            }
            array[i] = key;
            // 需要注意的是：一趟快排之后左侧基准值已经达到正确位置，以一趟快排后的i为界，二分排序
            qSort (array, low, i - 1);
            qSort (array, i + 1, high);
        }

        /// <summary>
        /// 快速排序
        /// </summary>
        /// <param name="array">目标数组</param>
        public static void quickSort (this List<double> array) {
            qSort (array, 0, array.Count - 1);
        }
        #endregion

        #region 二叉堆

        /// <summary>
        /// 上浮(子节点,用于插入数据，插入数据放入数组尾端)
        /// </summary>
        private static void upFloat (List<int> targetList) {
            int childIndex = targetList.Count - 1;
            int parentIndex = childIndex >> 1;
            int tempValue = targetList[childIndex];

            while (childIndex > 0 && tempValue > targetList[parentIndex]) {
                targetList[childIndex] = targetList[parentIndex];
                childIndex = parentIndex;
                parentIndex >>= 1;
            }

            targetList[childIndex] = tempValue;
        }

        /// <summary>
        /// 下沉(用于删除节点，亦用于重构整个二叉堆)
        /// </summary>
        /// <param name="targetIndex">删除的节点索引</param>
        /// <param name="targetList"></param>
        private static void downSink (int targetIndex, List<int> targetList) {
            int parentIndex = targetIndex;
            // 左子节点
            int childIndex = (parentIndex << 1) + 1;
            int tempValue = targetList[parentIndex];

            while (childIndex <= targetList.Count - 1) {
                int rightChildIndex = childIndex + 1;
                // 若有右子节点，且右子节点比左子节点大，则将 childrenIndex 记录为右子节点的下标
                if (rightChildIndex <= targetList.Count - 1 && targetList[rightChildIndex] > targetList[childIndex]) {
                    childIndex++;
                }

                // 如果父节点大于子节点，则无需下沉，直接返回
                if (tempValue >= targetList[childIndex]) {
                    break;
                }

                targetList[parentIndex] = targetList[childIndex];

                parentIndex = childIndex;
                childIndex = (parentIndex << 1) + 1;
            }

            targetList[parentIndex] = tempValue;
        }

        /// <summary>
        /// 构建二叉堆
        /// </summary>
        /// <param name="targetList"></param>
        public static void buildBinaryHeap (List<int> targetList) {
            for (int i = targetList.Count / 2 - 1; i >= 0; i--) {
                downSink (i, targetList);
            }
        }

        #endregion

        /// <summary>
        /// 字符串是否存在
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static bool isVialid (this string target) {
            if (target == null || target == "") {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 获取区间内的随机数值[min,max]
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static int getRandomValue (int min, int max) {
            min = min > max?max : min;
            max = max < min?min : max;
            int result = Random.Range (min, max + 1);
            return result;
        }

        /// <summary>
        /// 混乱数组元素
        /// </summary>
        /// <param name="sourceList"></param>
        /// <typeparam name="T"></typeparam>
        public static void confusionElement<T> (List<T> sourceList) {
            for (int i = sourceList.Count - 1; i >= 0; i--) {
                int randomValue = getRandomValue (0, i);
                T tempValue = sourceList[i];
                sourceList[i] = sourceList[randomValue];
                sourceList[randomValue] = tempValue;
            }
        }

        /// <summary>
        /// 有序数组去重
        /// </summary>
        /// <param name="sourceList">原数组</param>
        public static void removeDuplicate (List<int> sourceList) {
            int first = 0;

            int second;

            int sourceListCount = sourceList.Count;
            if (sourceListCount <= 1) {
                return;
            }

            for (second = 1; second < sourceListCount; second++) {
                if (sourceList[first] != sourceList[second]) {
                    sourceList[++first] = sourceList[second];
                }
            }

            int newListLength = first + 1;
            sourceList = sourceList.GetRange (0, newListLength);
        }

        public static bool isOddNumber (int value) {
            // 奇数的二进制最后一位为1
            return (value & 1) == 1;
        }

        //取一定范围内的一个奇数
        public static int getOddNumber (int min, int max) {
            while (true) {
                int temp = Random.Range (min, max);
                if (!isOddNumber (temp)) {
                    continue;
                }
                return temp;
            }
        }

        /// <summary> 
        /// 简单2d射线检测（只返回是否检测到目标）       
        /// </summary> 
        /// <param name="startPos">开始位置</param> 
        /// <param name="direcition">方向</param> 
        /// <param name="distance">距离</param> 
        /// <param name="layerMask">检测层</param> 
        /// <returns></returns> 
        public static bool ray2DCheck (Vector2 startPos, Vector2 direcition, float distance, int layerMask) {
            RaycastHit2D raycastInfo = Physics2D.Raycast (startPos, direcition, distance, layerMask);
            if (raycastInfo) {
                return true;
            }

            return false;
        }

        /// <summary> 
        /// 绘制线段 
        /// </summary> 
        /// <param name="startPos">开始位置</param> 
        /// <param name="direcition">方向</param> 
        /// <param name="distance">距离</param> 
        /// <param name="color">颜色</param> 
        public static void drawLine (Vector3 startPos, Vector3 direcition, float distance, Color color) {
            Vector3 endPos = startPos + direcition * distance;
            Debug.DrawLine (startPos, endPos, color);
        }

        public static string getCurPlatformName () {
            RuntimePlatform runtimePlatform = Application.platform;
            switch (runtimePlatform) {
                case RuntimePlatform.WindowsPlayer:
                    return "Windows";

                case RuntimePlatform.IPhonePlayer:
                    return "IOS";

                case RuntimePlatform.Android:
                    return "Android";

                case RuntimePlatform.LinuxPlayer:
                    return "Linux";

                case RuntimePlatform.WindowsEditor:
                    Debug.LogWarning ("curPlatform WindosEditor default use windows");
                    return "Windows";

                default:
                    // FIXME: other platfrom not support
                    return null;
            }
        }

        public static string getBundleUrl () {
            string platformName = getCurPlatformName ();
            string bundleUrl = Application.dataPath + "/AssetsBundles/" + platformName + "/";
            return bundleUrl;
        }
    }
}
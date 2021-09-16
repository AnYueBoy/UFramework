/*
 * @Author: l hy 
 * @Date: 2021-01-25 18:42:53 
 * @Description: 帧率计算器
 */

namespace UFramework.Develop {
    using UnityEngine;
    public class FPSCounter {

        private readonly float calculateRate = 0.5f;

        private int frameCount = 0;

        private float rateTimer = 0;

        private int fps = 0;

        public void init () {
            this.frameCount = 0;
            this.rateTimer = 0;
            this.fps = 0;
        }

        public void localUpdate () {
            ++this.frameCount;
            this.rateTimer += Time.deltaTime;
            if (rateTimer > this.calculateRate) {
                this.fps = (int) (this.frameCount / this.rateTimer);
                this.frameCount = 0;
                this.rateTimer = 0;
            }
        }

        public void drawGUI () {
            GUI.color = Color.black;
            GUI.Box (new Rect (80, 20, 120, 25), "fps:" + this.fps.ToString ());
        }
    }
}
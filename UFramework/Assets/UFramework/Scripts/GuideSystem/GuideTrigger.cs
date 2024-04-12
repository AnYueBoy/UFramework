using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UFramework
{
    public class GuideTrigger : MonoBehaviour
    {
        private RectTransform rectTrans;
        private Vector2 parentSize;

        public event Action<PointerEventData> OnPointerClickEvent;
        public event Action<PointerEventData> OnPointerDownEvent;
        public event Action<PointerEventData> OnPointerUpEvent;

        public event Action<PointerEventData> OnBeginDragEvent;
        public event Action<PointerEventData> OnDragEvent;
        public event Action<PointerEventData> OnEndDragEvent;

        private void Awake()
        {
            rectTrans = GetComponent<RectTransform>();
            parentSize = rectTrans.parent.GetComponent<RectTransform>().sizeDelta;

            var eventTriggerComp = GetComponent<EventTrigger>();

            // 添加点击entry
            var pointerClickEntry = new EventTrigger.Entry();
            pointerClickEntry.eventID = EventTriggerType.PointerClick;
            pointerClickEntry.callback.AddListener(PointerClick);
            eventTriggerComp.triggers.Add(pointerClickEntry);

            // 添加指针按下事件
            var pointerDownEntry = new EventTrigger.Entry();
            pointerDownEntry.eventID = EventTriggerType.PointerDown;
            pointerDownEntry.callback.AddListener(PointerDown);
            eventTriggerComp.triggers.Add(pointerDownEntry);

            // 添加指针抬起事件
            var pointerUpEntry = new EventTrigger.Entry();
            pointerUpEntry.eventID = EventTriggerType.PointerUp;
            pointerUpEntry.callback.AddListener(PointerUp);
            eventTriggerComp.triggers.Add(pointerUpEntry);

            // 添加拖动开始事件
            var dragStartEntry = new EventTrigger.Entry();
            dragStartEntry.eventID = EventTriggerType.BeginDrag;
            dragStartEntry.callback.AddListener(BeginDrag);
            eventTriggerComp.triggers.Add(dragStartEntry);

            // 添加拖动中事件
            var dragEntry = new EventTrigger.Entry();
            dragEntry.eventID = EventTriggerType.Drag;
            dragEntry.callback.AddListener(Drag);
            eventTriggerComp.triggers.Add(dragEntry);

            // 添加拖动结束事件
            var endDragEntry = new EventTrigger.Entry();
            endDragEntry.eventID = EventTriggerType.EndDrag;
            endDragEntry.callback.AddListener(EndDrag);
            eventTriggerComp.triggers.Add(endDragEntry);
        }

        private void PointerClick(BaseEventData data)
        {
            PointerEventData eventData = data as PointerEventData;
            OnPointerClickEvent?.Invoke(eventData);
            OnPointerClickEvent = null;
        }

        private void PointerDown(BaseEventData data)
        {
            PointerEventData eventData = data as PointerEventData;
            OnPointerDownEvent?.Invoke(eventData);
        }

        private void PointerUp(BaseEventData data)
        {
            PointerEventData eventData = data as PointerEventData;
            OnPointerUpEvent?.Invoke(eventData);
        }

        private void BeginDrag(BaseEventData data)
        {
            PointerEventData eventData = data as PointerEventData;
            OnBeginDragEvent?.Invoke(eventData);
        }

        private void Drag(BaseEventData data)
        {
            PointerEventData eventData = data as PointerEventData;
            OnDragEvent?.Invoke(eventData);
        }

        private void EndDrag(BaseEventData data)
        {
            PointerEventData eventData = data as PointerEventData;
            OnEndDragEvent?.Invoke(eventData);
        }

        #region 对外接口

        public void Show(Vector2 pos, Vector2 size)
        {
            Hide();
            gameObject.SetActive(true);
            rectTrans.localPosition = pos;
            rectTrans.sizeDelta = size;
        }

        public void ShowFullScreen()
        {
            Show(Vector2.zero, parentSize);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
            OnPointerDownEvent = OnPointerUpEvent = null;
            OnBeginDragEvent = OnDragEvent = OnEndDragEvent = null;
            OnPointerClickEvent = null;
        }

        #endregion
    }
}
using UnityEngine;
using UnityEngine.EventSystems;

namespace ProcessScheduling
{
    [RequireComponent(typeof(CanvasGroup))]
    public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public Transform ParentToReturnTo
        {
            get;
            set;
        }

        public Vector3 OriginalLocalPosition
        {
            get;
            set;
        }

        public int OriginalSiblingIndex
        {
            get;
            set;
        }

        public bool ShouldReturnToOriginalParent
        {
            get;
            set;
        }

        private Vector3 dragPositionOffset;

        private CanvasGroup canvasGroup;

        public void OnBeginDrag(PointerEventData eventData)
        {
            Vector3 cursorPosition = eventData.position;
            dragPositionOffset = this.transform.position - cursorPosition;

            if (canvasGroup != null)
            {
                canvasGroup.blocksRaycasts = false;
            }

            ParentToReturnTo = this.transform.parent;
            OriginalLocalPosition = this.transform.localPosition;
            OriginalSiblingIndex = this.transform.GetSiblingIndex();
            ShouldReturnToOriginalParent = true;

            // TODO: Temporary fix for process going inside clipping bounds of scroll rect
            Canvas parentCanvas = GetComponentInParent<Canvas>();
            if (parentCanvas != null)
            {
                this.transform.SetParent(parentCanvas.transform);
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            Vector3 cursorPosition = eventData.position;
            this.transform.position = cursorPosition + dragPositionOffset;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            dragPositionOffset = Vector3.zero;

            if (ShouldReturnToOriginalParent)
            {
                this.transform.SetParent(ParentToReturnTo);
                this.transform.SetSiblingIndex(OriginalSiblingIndex);
                this.transform.localPosition = OriginalLocalPosition;
            }

            if (canvasGroup != null)
            {
                canvasGroup.blocksRaycasts = true;
            }
        }

        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }
    }
}

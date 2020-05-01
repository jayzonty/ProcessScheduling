using UnityEngine;
using UnityEngine.EventSystems;

namespace ProcessScheduling
{
    public class IOQueueBehavior : MonoBehaviour, IDropHandler
    {
        public Transform dropDestinationTransform;

        public void OnDrop(PointerEventData eventData)
        {
            GameObject draggedObject = eventData.pointerDrag;
            if (draggedObject != null)
            {
                ProcessBehavior process = draggedObject.GetComponent<ProcessBehavior>();
                if (process != null)
                {
                    if (process.CurrentState == ProcessBehavior.State.IOWait)
                    {
                        draggedObject.transform.SetParent(dropDestinationTransform);
                        draggedObject.transform.localPosition = Vector3.zero;

                        Draggable draggable = draggedObject.GetComponent<Draggable>();
                        if (draggable != null)
                        {
                            draggable.ShouldReturnToOriginalParent = false;
                            CPUBehavior cpuBehavior = draggable.ParentToReturnTo.GetComponentInParent<CPUBehavior>();
                            if (cpuBehavior != null)
                            {
                                cpuBehavior.CurrentProcess = null;
                            }
                        }
                    }
                }
            }
        }
    }
}

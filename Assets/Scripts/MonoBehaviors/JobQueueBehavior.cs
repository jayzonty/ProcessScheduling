using UnityEngine;
using UnityEngine.EventSystems;

namespace ProcessScheduling
{
    public class JobQueueBehavior : MonoBehaviour, IDropHandler
    {
        public Transform contentTransform;

        public void AddProcess(ProcessBehavior process)
        {
            if (contentTransform == null)
            {
                return;
            }

            process.transform.SetParent(contentTransform, false);
        }

        public void OnDrop(PointerEventData eventData)
        {
            if (contentTransform == null)
            {
                return;
            }

            GameObject draggedObject = eventData.pointerDrag;
            if (draggedObject == null)
            {
                return;
            }

            Draggable draggable = draggedObject.GetComponent<Draggable>();
            if (draggable == null)
            {
                return;
            }

            ProcessBehavior process = draggedObject.GetComponent<ProcessBehavior>();
            if (process != null)
            {
                bool acceptProcess = false;

                switch (process.CurrentState)
                {
                    case ProcessBehavior.State.New:
                        acceptProcess = true;
                        process.CurrentState = ProcessBehavior.State.Ready;

                        JobRequestContainerBehavior jobRequestContainer = draggable.ParentToReturnTo.GetComponentInParent<JobRequestContainerBehavior>();
                        if (jobRequestContainer != null)
                        {
                            jobRequestContainer.SetProcess(null);
                        }

                        break;

                    case ProcessBehavior.State.Ready:
                        acceptProcess = true;
                        break;

                    case ProcessBehavior.State.Running:
                        acceptProcess = true;
                        process.CurrentState = ProcessBehavior.State.Ready;
                        break;

                    case ProcessBehavior.State.IOWait:
                        break;

                    case ProcessBehavior.State.Finished:
                        break;
                }

                draggable.ShouldReturnToOriginalParent = !acceptProcess;

                if (acceptProcess)
                {
                    AddProcess(process);
                }
            }
        }
    }
}

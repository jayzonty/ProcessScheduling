using UnityEngine;
using UnityEngine.EventSystems;

namespace ProcessScheduling
{
    public class CPUBehavior : MonoBehaviour, IDropHandler
    {
        public Transform dropDestinationTransform;

        public ProcessBehavior CurrentProcess
        {
            get;
            set;
        }

        private TimeManager timeManager;
        
        public void OnDrop(PointerEventData eventData)
        {
            if (dropDestinationTransform == null)
            {
                return;
            }

            GameObject draggedObject = eventData.pointerDrag;
            if (draggedObject != null)
            {
                ProcessBehavior process = draggedObject.GetComponent<ProcessBehavior>();
                if (process != null)
                {
                    draggedObject.transform.SetParent(dropDestinationTransform);
                    draggedObject.transform.localPosition = Vector3.zero;

                    CurrentProcess = process;
                }

                Draggable draggable = draggedObject.GetComponent<Draggable>();
                if (draggable != null)
                {
                    draggable.ShouldReturnToOriginalParent = false;
                }
            }
        }

        private void Awake()
        {
            CurrentProcess = null;

            timeManager = GameObject.FindObjectOfType<TimeManager>();
        }

        private void Update()
        {
            if (CurrentProcess != null)
            {
                if (timeManager != null)
                {
                    if (CurrentProcess.CurrentState == ProcessBehavior.State.Ready || CurrentProcess.CurrentState == ProcessBehavior.State.Running || CurrentProcess.CurrentState == ProcessBehavior.State.IOWait)
                    {
                        CurrentProcess.Execute(Time.deltaTime * timeManager.TimeMultiplier);
                    }
                    else if (CurrentProcess.CurrentState == ProcessBehavior.State.Finished)
                    {
                        Destroy(CurrentProcess.gameObject);
                        CurrentProcess = null;
                    }
                }
            }
        }
    }
}

using System.Collections.Generic;

using UnityEngine;
using UnityEngine.EventSystems;

namespace ProcessScheduling
{
    public class IOQueueBehavior : MonoBehaviour, IDropHandler
    {
        public Transform dropDestinationTransform;

        /// <summary>
        /// Reference to the time manager script
        /// </summary>
        private TimeManager timeManager = null;

        /// <summary>
        /// List of processes in the I/O queue
        /// </summary>
        private List<ProcessBehavior> processList;

        /// <summary>
        /// Reference to the game manager script
        /// </summary>
        private GameManager gameManager;

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
                                cpuBehavior.ChangeState(CPUBehavior.State.Idle);
                            }
                        }

                        processList.Add(process);
                    }
                }
            }
        }

        /// <summary>
        /// Unity callback function called when the script
        /// is created.
        /// </summary>
        private void Awake()
        {
            gameManager = GameObject.FindObjectOfType<GameManager>();
            timeManager = GameObject.FindObjectOfType<TimeManager>();

            processList = new List<ProcessBehavior>();
        }

        /// <summary>
        /// Unity callback function called when the script
        /// is enabled.
        /// </summary>
        private void OnEnable()
        {
            if (timeManager != null)
            {
                timeManager.TimerTick += TimeManager_TimerTick;
            }
        }

        /// <summary>
        /// Unity callback function called when the script
        /// is disabled.
        /// </summary>
        private void OnDisable()
        {
            if (timeManager != null)
            {
                timeManager.TimerTick -= TimeManager_TimerTick;
            }
        }

        /// <summary>
        /// Callback function per in-game time tick
        /// </summary>
        /// <param name="ticksSinceStart">Number of ticks since the in-game timer started</param>
        private void TimeManager_TimerTick(int ticksSinceStart)
        {
            if (processList.Count > 0)
            {
                // Get the first process in the queue
                ProcessBehavior process = processList[0];

                switch (process.CurrentState)
                {
                    case ProcessBehavior.State.IOWait:
                        process.ExecuteIO(1);
                        break;

                    case ProcessBehavior.State.Ready:
                        if (gameManager != null)
                        {
                            gameManager.AddProcessToJobQueue(process);
                        }

                        // Remove the first element from the queue
                        processList.RemoveAt(0);
                        break;

                    default:
                        break;
                }
            }
        }
    }
}

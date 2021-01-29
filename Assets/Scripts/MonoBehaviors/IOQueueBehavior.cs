using System.Collections.Generic;

using UnityEngine;
using UnityEngine.EventSystems;

namespace ProcessScheduling
{
    public class IOQueueBehavior : MonoBehaviour, IDropHandler
    {
        public Transform dropDestinationTransform;

        /// <summary>
        /// Minimum amount of time processes have to wait
        /// before I/O requests are processed.
        /// </summary>
        public int minIOAvailbleTime = 5;

        /// <summary>
        /// Maximum amount of time processes have to wait
        /// before I/O requests are processed.
        /// </summary>
        public int maxIOAvailableTime = 10;

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

        /// <summary>
        /// Timer until I/O processing is available
        /// </summary>
        private int timerUntilIOAvailable;

        /// <summary>
        /// Function called when an attempt to drop a game object
        /// to this game object was done
        /// </summary>
        /// <param name="eventData">Data related to the drop event</param>
        public void OnDrop(PointerEventData eventData)
        {
            if (dropDestinationTransform == null)
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
            if (process == null)
            {
                return;
            }

            bool acceptProcess = false;

            switch (process.CurrentState)
            {
                case ProcessBehavior.State.New:
                case ProcessBehavior.State.Ready:
                case ProcessBehavior.State.Running:
                    break;

                case ProcessBehavior.State.IOWait:
                    acceptProcess = true;

                    CPUBehavior cpuBehavior = draggable.ParentToReturnTo.GetComponentInParent<CPUBehavior>();
                    if (cpuBehavior != null)
                    {
                        cpuBehavior.CurrentProcess = null;
                        cpuBehavior.ChangeState(CPUBehavior.State.Idle);
                    }

                    break;

                case ProcessBehavior.State.Terminated:
                case ProcessBehavior.State.Finished:
                    break;
            }

            if (acceptProcess)
            {
                draggedObject.transform.SetParent(dropDestinationTransform);
                draggedObject.transform.localPosition = Vector3.zero;

                processList.Add(process);
            }

            draggable.ShouldReturnToOriginalParent = !acceptProcess;
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
        /// Unity callback function called before the first
        /// Update() call.
        /// </summary>
        private void Start()
        {
            timerUntilIOAvailable = Random.Range(minIOAvailbleTime, maxIOAvailableTime + 1);
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

                timerUntilIOAvailable = Mathf.Max(timerUntilIOAvailable - 1, 0);

                if (timerUntilIOAvailable <= 0)
                {
                    process.ChangeState(ProcessBehavior.State.Ready);
                    gameManager.AddProcessToJobQueue(process);

                    timerUntilIOAvailable = Random.Range(minIOAvailbleTime, maxIOAvailableTime + 1);

                    // Remove the first element from the queue
                    processList.RemoveAt(0);
                }
            }
        }
    }
}

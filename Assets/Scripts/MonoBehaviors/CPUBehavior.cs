using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ProcessScheduling
{
    public class CPUBehavior : MonoBehaviour, IDropHandler
    {
        public Transform dropDestinationTransform;

        public Text statusText;

        public int contextSwitchTime = 5;

        public ProcessBehavior CurrentProcess
        {
            get;
            set;
        }

        public enum State
        {
            Idle,
            Running,
            ContextSwitch
        }
        public State CurrentState
        {
            get;
            private set;
        } = State.Idle;

        private TimeManager timeManager;

        private GameManager gameManager;

        private int contextSwitchTimer = 0;

        private ProcessBehavior nextProcess = null;
        
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
            if (process != null)
            {
                bool acceptProcess = false;

                switch (process.CurrentState)
                {
                    case ProcessBehavior.State.New:
                        break;

                    case ProcessBehavior.State.Ready:
                        acceptProcess = true;
                        break;

                    case ProcessBehavior.State.Running:
                    case ProcessBehavior.State.IOWait:
                    case ProcessBehavior.State.Terminated:
                    case ProcessBehavior.State.Finished:
                        break;
                }

                if (acceptProcess)
                {
                    draggedObject.transform.SetParent(dropDestinationTransform);
                    draggedObject.transform.localPosition = Vector3.zero;

                    if (CurrentState == State.Idle)
                    {
                        CurrentState = State.Running;
                    }
                    else if (CurrentState == State.Running)
                    {
                        if (CurrentProcess.CurrentState == ProcessBehavior.State.Running)
                        {
                            CurrentProcess.ChangeState(ProcessBehavior.State.Ready);
                        }

                        gameManager.AddProcessToJobQueue(CurrentProcess);
                    }

                    CurrentProcess = process;
                }

                draggable.ShouldReturnToOriginalParent = !acceptProcess;
            }
        }

        /// <summary>
        /// Change CPU's state
        /// </summary>
        /// <param name="newState">New state</param>
        public void ChangeState(State newState)
        {
            CurrentState = newState;
        }

        private void Awake()
        {
            timeManager = GameObject.FindObjectOfType<TimeManager>();
            gameManager = GameObject.FindObjectOfType<GameManager>();
        }

        private void OnEnable()
        {
            if (timeManager != null)
            {
                timeManager.TimerTick += TimeManager_TimerTick;
            }
        }

        private void OnDisable()
        {
            if (timeManager != null)
            {
                timeManager.TimerTick -= TimeManager_TimerTick;
            }
        }

        private void TimeManager_TimerTick(int tick)
        {
            if (CurrentProcess != null)
            {
                if (timeManager != null)
                {
                    if (CurrentState == State.Running)
                    {
                        statusText.enabled = false;
                        if (CurrentProcess.CurrentState == ProcessBehavior.State.Ready || CurrentProcess.CurrentState == ProcessBehavior.State.Running || CurrentProcess.CurrentState == ProcessBehavior.State.IOWait)
                        {
                            CurrentProcess.Execute(1);
                        }
                        else if (CurrentProcess.CurrentState == ProcessBehavior.State.Finished)
                        {
                            Destroy(CurrentProcess.gameObject);
                            CurrentProcess = null;

                            CurrentState = State.Idle;
                            statusText.enabled = true;
                            statusText.text = "Idle";

                            ++gameManager.numFinishedProcesses;
                            --gameManager.NumProcessesInSystem;
                        }
                    }
                    else if (CurrentState == State.ContextSwitch)
                    {
                    }
                }
            }
        }
    }
}

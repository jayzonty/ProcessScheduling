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
            if (draggedObject != null)
            {
                ProcessBehavior process = draggedObject.GetComponent<ProcessBehavior>();
                if (process != null)
                {
                    if (CurrentState == State.Idle)
                    {
                        draggedObject.transform.SetParent(dropDestinationTransform);
                        draggedObject.transform.localPosition = Vector3.zero;

                        CurrentState = State.Running;
                    }
                    else if (CurrentState == State.Running)
                    {
                        nextProcess = process;

                        // Hide the process visual first because we have to display the context switch text
                        nextProcess.IsVisible = false;

                        CurrentProcess.CurrentState = ProcessBehavior.State.Ready;
                        gameManager.AddProcessToJobQueue(CurrentProcess);
                        CurrentProcess = null;

                        contextSwitchTimer = contextSwitchTime;
                        CurrentState = State.ContextSwitch;
                    }
                    
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
                        }
                    }
                    else if (CurrentState == State.ContextSwitch)
                    {
                        statusText.enabled = true;

                        contextSwitchTimer = Mathf.Max(0, contextSwitchTimer - 1);
                        statusText.text = "Context Switch\n(" + contextSwitchTimer.ToString("F2") + ")";
                        if (contextSwitchTimer == 0)
                        {
                            if (nextProcess != null)
                            {
                                nextProcess.transform.SetParent(dropDestinationTransform, false);
                                nextProcess.transform.localPosition = Vector3.zero;

                                CurrentProcess = nextProcess;

                                // Unhide the process visual after context switch is done
                                nextProcess.IsVisible = true;
                                nextProcess = null;

                                CurrentState = State.Running;
                            }
                        }
                    }
                }
            }
        }
    }
}

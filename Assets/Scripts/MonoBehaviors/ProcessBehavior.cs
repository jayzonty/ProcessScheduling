using UnityEngine;
using UnityEngine.UI;

namespace ProcessScheduling
{
    public class ProcessBehavior : MonoBehaviour
    {
        /// <summary>
        /// Color if the process is in a ready state
        /// </summary>
        public Color readyStateColor = Color.white;

        /// <summary>
        /// Color if the process is in a running state
        /// </summary>
        public Color runningStateColor = Color.green;

        /// <summary>
        /// Color if the process is in an I/O wait state
        /// </summary>
        public Color ioWaitStateColor = Color.yellow;

        public enum State
        {
            New,
            Ready,
            Running,
            IOWait,
            Finished,
            Terminated
        }

        public State CurrentState
        {
            get;
            private set;
        }

        public Text processNameText;
        public Text processInfoText;

        /// <summary>
        /// Process template
        /// </summary>
        public ProcessTemplate Template
        {
            get;
            set;
        } = null;

        /// <summary>
        /// Property for the process name
        /// </summary>
        public string Name
        {
            get
            {
                if (Template != null)
                {
                    return Template.name;
                }

                return string.Empty;
            }
        }

        /// <summary>
        /// Probability that the process will request for I/O
        /// whenever an I/O request check is made.
        /// </summary>
        public float IORequestProbability
        {
            get
            {
                if (Template != null)
                {
                    return Template.ioRequestProbability;
                }

                return 0.0f;
            }
        }

        /// <summary>
        /// Property for the remaining burst time
        /// </summary>
        public int RemainingBurstTime
        {
            get
            {
                return remainingBurstTime;
            }

            set
            {
                remainingBurstTime = value;
                UpdateInfoDisplay();
            }
        }

        public int ExecutionDeadline
        {
            get;
            set;
        }

        /// <summary>
        /// Property for the execution deadline timer
        /// </summary>
        public int ExecutionDeadlineTimer
        {
            get
            {
                return executionDeadlineTimer;
            }

            set
            {
                executionDeadlineTimer = value;
                UpdateInfoDisplay();
            }
        }

        public int TurnaroundTime
        {
            get;
            private set;
        }

        /// <summary>
        /// Property for the waiting time
        /// </summary>
        public int WaitingTime
        {
            get;
            private set;
        }

        /// <summary>
        /// Process name
        /// </summary>
        private string processName;

        /// <summary>
        /// Remaining burst time
        /// </summary>
        private int remainingBurstTime = 0;

        /// <summary>
        /// Execution deadline timer
        /// </summary>
        private int executionDeadlineTimer;

        /// <summary>
        /// Timer before the next I/O request check
        /// </summary>
        private int ioRequestCheckTimer = 0;

        private int startTime;

        private TimeManager timeManager = null;

        private GameManager gameManager = null;

        private CanvasGroup canvasGroup;

        /// <summary>
        /// Reference to the image component
        /// </summary>
        private Image imageComponent;

        public bool IsVisible
        {
            get
            {
                return canvasGroup.alpha == 1.0f;
            }
            
            set
            {
                canvasGroup.alpha = value ? 1.0f : 0.0f;
            }
        }

        public void ChangeState(State newState)
        {
            if (CurrentState == newState)
            {
                return;
            }

            switch (newState)
            {
                case State.New:
                    break;

                case State.Ready:
                    ExecutionDeadlineTimer = ExecutionDeadline;
                    break;

                case State.Running:
                    break;

                case State.IOWait:
                    break;

                case State.Terminated:
                    break;

                case State.Finished:
                    break;
            }

            CurrentState = newState;

            UpdateInfoDisplay();
        }

        public void Execute(int deltaTime)
        {
            if (CurrentState == State.Ready)
            {
                ChangeState(State.Running);
            }
            else if (CurrentState == State.Running)
            {
                RemainingBurstTime = Mathf.Max(0, RemainingBurstTime - deltaTime);
                ioRequestCheckTimer = Mathf.Max(0, ioRequestCheckTimer - deltaTime);

                if (RemainingBurstTime > 0)
                {
                    if (ioRequestCheckTimer == 0)
                    {
                        bool isIORequest = false;
                        if (Template != null)
                        {
                            if (Template.ioRequestProbability > 0.0f)
                            {
                                isIORequest = Random.Range(0.0f, 1.0f) <= Template.ioRequestProbability;
                            }
                        }
                        if (isIORequest)
                        {
                            ChangeState(State.IOWait);
                        }

                        ioRequestCheckTimer = 2;
                    }
                }
                else
                {
                    ChangeState(State.Finished);
                }
            }
        }

        private void Awake()
        {
            timeManager = GameObject.FindObjectOfType<TimeManager>();

            gameManager = GameObject.FindObjectOfType<GameManager>();
            
            canvasGroup = GetComponent<CanvasGroup>();

            imageComponent = GetComponent<Image>();

            CurrentState = State.New;
        }

        private void Start()
        {
            if (timeManager != null)
            {
                startTime = timeManager.CurrentGameTime;
            }
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
            switch (CurrentState)
            {
                case State.New:
                    break;

                case State.Ready:
                    if (ExecutionDeadline > 0)
                    {
                        --ExecutionDeadlineTimer;
                        if (ExecutionDeadlineTimer <= 0)
                        {
                            ++gameManager.numMissedProcesses;
                            --gameManager.NumProcessesInSystem;
                            Destroy(gameObject);
                        }
                    }

                    ++WaitingTime;

                    break;

                case State.Running:
                case State.IOWait:
                case State.Terminated:
                case State.Finished:
                    break;
            }

            TurnaroundTime = timeManager.CurrentGameTime - startTime;

            UpdateInfoDisplay();
        }

        private void UpdateInfoDisplay()
        {
            if (processNameText != null)
            {
                processNameText.text = Name;
            }

            if (processInfoText != null)
            {
                string info = "Burst: " + RemainingBurstTime.ToString() + "\n";
                info += "State: " + CurrentState.ToString() + "\n";
                info += "Time in system: " + TurnaroundTime.ToString();

                if (CurrentState == State.Ready)
                {
                    if (ExecutionDeadline > 0.0f)
                    {
                        info += "\nDeadline: " + ExecutionDeadlineTimer.ToString();
                    }
                }
                processInfoText.text = info;
            }

            // Change color based on the process state
            if (imageComponent != null)
            {
                switch (CurrentState)
                {
                    case State.Ready:
                        imageComponent.color = readyStateColor;
                        break;

                    case State.Running:
                        imageComponent.color = runningStateColor;
                        break;

                    case State.IOWait:
                        imageComponent.color = ioWaitStateColor;
                        break;

                    case State.Finished:
                        break;
                }
            }
        }
    }
}

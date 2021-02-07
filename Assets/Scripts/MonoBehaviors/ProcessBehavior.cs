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
        /// Property for the process name
        /// </summary>
        public string Name
        {
            get
            {
                return processName;
            }

            set
            {
                processName = value;
                UpdateInfoDisplay();
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

        public int MinTimeUntilIOWait
        {
            get;
            set;
        }

        public int MaxTimeUntilIOWait
        {
            get;
            set;
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

        private int timeUntilNextIOWait = 0;

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
                timeUntilNextIOWait = Random.Range(MinTimeUntilIOWait, MaxTimeUntilIOWait);

                CurrentState = State.Running;
            }
            else if (CurrentState == State.Running)
            {
                RemainingBurstTime = Mathf.Max(0, RemainingBurstTime - deltaTime);
                timeUntilNextIOWait = Mathf.Max(0, timeUntilNextIOWait - deltaTime);

                if (RemainingBurstTime > 0)
                {
                    if (timeUntilNextIOWait == 0)
                    {
                        CurrentState = State.IOWait;
                        timeUntilNextIOWait = Random.Range(MinTimeUntilIOWait, MaxTimeUntilIOWait + 1);
                    }
                }
                else
                {
                    CurrentState = State.Finished;
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

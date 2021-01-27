using UnityEngine;
using UnityEngine.UI;

namespace ProcessScheduling
{
    public class ProcessBehavior : MonoBehaviour
    {
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

        public string Name
        {
            get;
            set;
        }

        public int RemainingBurstTime
        {
            get;
            set;
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

        public int MinIOWaitDuration
        {
            get;
            set;
        }

        public int MaxIOWaitDuration
        {
            get;
            set;
        }

        public int ExecutionDeadline
        {
            get;
            set;
        }

        public int ExecutionDeadlineTimer
        {
            get;
            private set;
        }

        public int TurnaroundTime
        {
            get;
            private set;
        }

        private int timeUntilNextIOWait = 0;
        private int timeUntilIOReceive = 0;

        private int startTime;

        private TimeManager timeManager = null;

        private GameManager gameManager = null;

        private CanvasGroup canvasGroup;

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
                        timeUntilIOReceive = Random.Range(MinIOWaitDuration, MaxIOWaitDuration + 1);
                    }
                }
                else
                {
                    CurrentState = State.Finished;
                }
            }
        }

        public void ExecuteIO(int deltaTime)
        {
            if (CurrentState == State.IOWait)
            {
                float timeMultipler = 1.0f;
                if (timeManager != null)
                {
                    timeMultipler = timeManager.timeMultiplier;
                }

                timeUntilIOReceive = Mathf.Max(0, timeUntilIOReceive - 1);
                if (timeUntilIOReceive == 0.0f)
                {
                    CurrentState = State.Ready;
                }
            }
        }

        private void Awake()
        {
            timeManager = GameObject.FindObjectOfType<TimeManager>();

            gameManager = GameObject.FindObjectOfType<GameManager>();
            
            canvasGroup = GetComponent<CanvasGroup>();

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
                            Destroy(gameObject);
                        }
                    }

                    break;

                case State.Running:
                case State.IOWait:
                case State.Terminated:
                case State.Finished:
                    break;
            }

            if (processNameText != null)
            {
                processNameText.text = Name;
            }

            if (processInfoText != null)
            {
                string info = "Burst: " + RemainingBurstTime.ToString() + "\n";
                info += "State: " + CurrentState.ToString() + "\n";
                info += "Time in system: " + TurnaroundTime.ToString();
                if (ExecutionDeadline > 0.0f)
                {
                    info += "\nDeadline: " + ExecutionDeadlineTimer.ToString();
                }
                processInfoText.text = info;
            }

            TurnaroundTime = timeManager.CurrentGameTime - startTime;
        }
    }
}

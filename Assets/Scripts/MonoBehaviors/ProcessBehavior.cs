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
            set;
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

        public int Deadline
        {
            get;
            set;
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

        private void Awake()
        {
            timeManager = GameObject.FindObjectOfType<TimeManager>();

            canvasGroup = GetComponent<CanvasGroup>();

            CurrentState = State.Ready;
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
            if (processNameText != null)
            {
                processNameText.text = Name;
            }

            if (processInfoText != null)
            {
                string info = "Burst: " + RemainingBurstTime.ToString() + "\n";
                info += "State: " + CurrentState.ToString() + "\n";
                info += "Time in system: " + TurnaroundTime.ToString();
                if (Deadline > 0.0f)
                {
                    info += "\nDeadline: " + Deadline.ToString();
                }
                processInfoText.text = info;
            }

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

            TurnaroundTime = timeManager.CurrentGameTime - startTime;
        }
    }
}

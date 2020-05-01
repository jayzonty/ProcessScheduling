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

        public float RemainingBurstTime
        {
            get;
            set;
        }

        public float MinTimeUntilIOWait
        {
            get;
            set;
        }

        public float MaxTimeUntilIOWait
        {
            get;
            set;
        }

        public float MinIOWaitDuration
        {
            get;
            set;
        }

        public float MaxIOWaitDuration
        {
            get;
            set;
        }

        private float timeUntilNextIOWait = 0.0f;
        private float timeUntilIOReceive = 0.0f;

        private TimeManager timeManager = null;

        public void Execute(float time)
        {
            if (CurrentState == State.Ready)
            {
                timeUntilNextIOWait = Random.Range(MinTimeUntilIOWait, MaxTimeUntilIOWait);

                CurrentState = State.Running;
            }
            else if (CurrentState == State.Running)
            {
                RemainingBurstTime = Mathf.Max(0.0f, RemainingBurstTime - time);
                timeUntilNextIOWait = Mathf.Max(0.0f, timeUntilNextIOWait - time);

                if (RemainingBurstTime > 0.0f)
                {
                    if (timeUntilNextIOWait <= 0.0f)
                    {
                        CurrentState = State.IOWait;
                        timeUntilNextIOWait = Random.Range(MinTimeUntilIOWait, MaxTimeUntilIOWait);
                        timeUntilIOReceive = Random.Range(MinIOWaitDuration, MaxIOWaitDuration);
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

            CurrentState = State.Ready;
        }

        private void Update()
        {
            if (processNameText != null)
            {
                processNameText.text = Name;
            }

            if (processInfoText != null)
            {
                string info = "Burst: " + RemainingBurstTime.ToString("F1") + "\n";
                info += "State: " + CurrentState.ToString();
                processInfoText.text = info;
            }

            if (CurrentState == State.IOWait)
            {
                float timeMultipler = 1.0f;
                if (timeManager != null)
                {
                    timeMultipler = timeManager.TimeMultiplier;
                }

                timeUntilIOReceive = Mathf.Max(0.0f, timeUntilIOReceive - Time.deltaTime * timeMultipler);
                if (timeUntilIOReceive <= 0.0f)
                {
                    CurrentState = State.Ready;
                }
            }
        }
    }
}

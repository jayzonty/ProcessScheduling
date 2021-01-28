using UnityEngine;

namespace ProcessScheduling
{
    public class GameManager : MonoBehaviour
    {
        public ProcessListScriptableObject processList;

        public GameObject processPrefab;

        public CPUListBehavior cpuList;

        public int numFinishedProcesses = 0;

        public int numMissedProcesses = 0;

        public int maxMissableProcesses = 5;

        public int initialProcessorCount = 2;

        public int timeLimit = 180;

        public bool hasTimeLimit = true;

        public int NumProcessesInSystem
        {
            get;
            set;
        } = 0;

        public int score = 0;

        private float timer = 0.0f;

        private JobQueueBehavior jobQueueBehavior;

        private TimeManager timeManager;

        public int NumCPUs
        {
            get
            {
                if (cpuList != null)
                {
                    return cpuList.GetNumCPUs();
                }

                return 0;
            }
        }

        public void AddProcessToJobQueue(ProcessBehavior processBehavior)
        {
            if (jobQueueBehavior == null)
            {
                return;
            }

            if (processBehavior == null)
            {
                return;
            }

            jobQueueBehavior.AddProcess(processBehavior);
        }

        private void Awake()
        {
            jobQueueBehavior = GameObject.FindObjectOfType<JobQueueBehavior>();

            timeManager = GameObject.FindObjectOfType<TimeManager>();
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

        private void Start()
        {
            if (cpuList != null)
            {
                cpuList.SetNumCPUs(initialProcessorCount);
            }
        }

        private void Update()
        {
            timer -= Time.deltaTime;
            if (timer <= 0.0f)
            {
                timer = 5.0f;

                if (jobQueueBehavior != null)
                {
                    if (NumProcessesInSystem < 5)
                    {
                        Process processToSpawn = processList.processes[Random.Range(0, processList.processes.Count)];
                        ProcessBehavior processRequest = CreateProcessBehavior(processToSpawn);
                        processRequest.ChangeState(ProcessBehavior.State.Ready);
                        jobQueueBehavior.AddProcess(processRequest);

                        ++NumProcessesInSystem;
                    }
                }
            }
        }


        private ProcessBehavior CreateProcessBehavior(Process process)
        {
            if (processPrefab == null)
            {
                return null;
            }

            GameObject processGO = GameObject.Instantiate(processPrefab);

            ProcessBehavior processBehavior = processGO.GetComponent<ProcessBehavior>();
            if (processBehavior != null)
            {
                processBehavior.Name = process.name;
                processBehavior.RemainingBurstTime = Random.Range(process.minBurstTime, process.maxBurstTime);
                processBehavior.MinTimeUntilIOWait = process.minTimeUntilIOWait;
                processBehavior.MaxTimeUntilIOWait = process.maxTimeUntilIOWait;
                processBehavior.MinIOWaitDuration = process.minIOWaitDuration;
                processBehavior.MaxIOWaitDuration = process.maxIOWaitDuration;

                bool hasDeadline = Random.Range(0, 2) == 0;
                if (hasDeadline)
                {
                    processBehavior.ExecutionDeadline = 10;
                }
            }

            return processBehavior;
        }

        private void TimeManager_TimerTick(int tick)
        {
            if (hasTimeLimit)
            {
                timeLimit = Mathf.Max(timeLimit - 1, 0);

                if (CheckLevelStopConditions())
                {
                    if (CheckWinConditions())
                    {
                        Debug.Log("Game Over! Success!");
                    }
                    else
                    {
                        Debug.Log("Game Over! Failed!");
                    }
                }
            }
        }

        /// <summary>
        /// Check whether the conditions to stop the level have been met.
        /// </summary>
        /// <returns></returns>
        private bool CheckLevelStopConditions()
        {
            // For now, just see if the time limit has expired,
            // or if the number of missed processes have exceeded the maximum.
            // TODO: Have a list of conditions to check
            return (timeLimit == 0) || (numMissedProcesses >= maxMissableProcesses);
        }

        /// <summary>
        /// Check whether the win conditions have been met
        /// </summary>
        /// <returns>Returns true if the win conditions have been met. False otherwise.</returns>
        private bool CheckWinConditions()
        {
            // For now, just check if the number of missed processes do not exceed the max.
            // TODO: Have a list of conditions to check
            return numMissedProcesses < maxMissableProcesses;
        }
    }
}

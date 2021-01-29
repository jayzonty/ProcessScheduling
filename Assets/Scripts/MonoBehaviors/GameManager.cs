using System.Collections.Generic;

using UnityEngine;

namespace ProcessScheduling
{
    public class GameManager : MonoBehaviour
    {
        public ProcessListScriptableObject processList;

        public GameObject processPrefab;

        public CPUListBehavior cpuList;

        public LevelData levelData;

        public int numFinishedProcesses = 0;

        public int numMissedProcesses = 0;

        public int remainingTime = 180;

        /// <summary>
        /// Number of processes currently in the system
        /// </summary>
        public int NumProcessesInSystem
        {
            get;
            set;
        } = 0;

        /// <summary>
        /// Total waiting time
        /// </summary>
        public int TotalWaitingTime
        {
            get;
            set;
        } = 0;

        /// <summary>
        /// Total turnaround time
        /// </summary>
        public int TotalTurnaroundTime
        {
            get;
            set;
        } = 0;

        /// <summary>
        /// Time elapsed since the level started
        /// </summary>
        public int TimeElapsed
        {
            get;
            private set;
        }

        /// <summary>
        /// CPU Utilization (in percentage [0.0, 1.0])
        /// </summary>
        public float CPUUtilization
        {
            get
            {
                if (cpuList.CPUs != null)
                {
                    if (cpuList.GetNumCPUs() == 0)
                    {
                        return 0.0f;
                    }

                    int totalCPURunningTime = 0;

                    foreach (CPUBehavior cpuBehavior in cpuList.CPUs)
                    {
                        totalCPURunningTime += cpuBehavior.TotalTimeRunning;
                    }

                    float cpuUtilization = (totalCPURunningTime * 1.0f) / (TimeElapsed * cpuList.GetNumCPUs());
                    return cpuUtilization;
                }

                return 0.0f;
            }
        }

        public int score = 0;

        private float timer = 0.0f;

        private JobQueueBehavior jobQueueBehavior;

        private TimeManager timeManager;

        private GameOverPanelBehavior gameOverPanelBehavior;

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

            gameOverPanelBehavior = GameObject.FindObjectOfType<GameOverPanelBehavior>();

            // TODO: Move this somewhere
            levelData = new LevelData();
            levelData.numProcessors = 2;
            levelData.timeLimit = 180;

            // Set up level stop conditions (timeLimit <= 0 || numMissedProcesses >= levelData.maxMissableProcesses)
            levelData.levelStopConditions = new List<Condition>();
            levelData.levelStopConditions.Add(new Condition { targetAttributeName = "remainingTime", operation = Condition.ComparisonOperation.LessThanEqual, comparisonValue = 0 });
            levelData.levelStopConditions.Add(new Condition { targetAttributeName = "numMissedProcesses", operation = Condition.ComparisonOperation.GreaterThanEqual, comparisonValue = levelData.maxMissableProcesses });

            levelData.winConditions = new List<Condition>();
            levelData.winConditions.Add(new Condition { targetAttributeName = "numMissedProcesses", operation = Condition.ComparisonOperation.LessThan, comparisonValue = 5 });
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
            if (levelData == null)
            {
                Debug.LogError("Level data is null!");
            }

            if (cpuList != null)
            {
                cpuList.SetNumCPUs(levelData.numProcessors);
            }

            remainingTime = levelData.timeLimit;
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
            ++TimeElapsed;

            if (levelData.timeLimit > 0)
            {
                remainingTime = Mathf.Max(remainingTime - 1, 0);

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

                    timeManager.PauseTimer();

                    if (gameOverPanelBehavior != null)
                    {
                        gameOverPanelBehavior.SetVisible(true);
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
            return CheckOneConditionSatisfied(levelData.levelStopConditions);
        }

        /// <summary>
        /// Check whether the win conditions have been met
        /// </summary>
        /// <returns>Returns true if the win conditions have been met. False otherwise.</returns>
        private bool CheckWinConditions()
        {
            return CheckAllConditionsSatisfied(levelData.winConditions);
        }

        public bool CheckOneConditionSatisfied(List<Condition> conditions)
        {
            if (conditions == null)
            {
                return true;
            }

            foreach (Condition condition in conditions)
            {
                if (CheckCondition(condition))
                {
                    return true;
                }
            }

            return false;
        }

        public bool CheckAllConditionsSatisfied(List<Condition> conditions)
        {
            if (conditions == null)
            {
                return true;
            }

            foreach (Condition condition in conditions)
            {
                if (!CheckCondition(condition))
                {
                    return false;
                }
            }

            return true;
        }

        private bool CheckCondition(Condition condition)
        {
            int lhs = 0;
            switch (condition.targetAttributeName)
            {
                case "remainingTime":
                    lhs = remainingTime;
                    break;

                case "numMissedProcesses":
                    lhs = numMissedProcesses;
                    break;
            }

            bool retValue = false;
            switch (condition.operation)
            {
                case Condition.ComparisonOperation.Equal:
                    retValue = (lhs == condition.comparisonValue);
                    break;

                case Condition.ComparisonOperation.NotEqual:
                    retValue = (lhs != condition.comparisonValue);
                    break;

                case Condition.ComparisonOperation.LessThan:
                    retValue = (lhs < condition.comparisonValue);
                    break;

                case Condition.ComparisonOperation.LessThanEqual:
                    retValue = (lhs <= condition.comparisonValue);
                    break;

                case Condition.ComparisonOperation.GreaterThan:
                    retValue = (lhs > condition.comparisonValue);
                    break;

                case Condition.ComparisonOperation.GreaterThanEqual:
                    retValue = (lhs >= condition.comparisonValue);
                    break;
            }

            return retValue;
        }
    }
}

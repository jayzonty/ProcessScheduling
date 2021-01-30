﻿using System.Collections.Generic;

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
        /// Maximum number of processes in the system
        /// </summary>
        public int MaxProcessesInSystem
        {
            get;
            private set;
        } = 5;

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

        public bool IsSuccess
        {
            get;
            private set;
        } = true;

        public LevelDataScriptableObject LevelData
        {
            get;
            private set;
        }

        public int score = 0;

        private int processSpawnTimer = 0;

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

            GameStateBehavior gameState = GameObject.FindObjectOfType<GameStateBehavior>();
            if (gameState != null)
            {
                LevelData = gameState.LevelData;
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

        private void Start()
        {
            if (LevelData == null)
            {
                Debug.LogError("Level data is null!");
            }

            if (cpuList != null)
            {
                cpuList.SetNumCPUs(LevelData.initialProcessorCount);
            }

            remainingTime = LevelData.timeLimit;

            // 3-second delay before the first process spawn
            processSpawnTimer = 3;

            MaxProcessesInSystem = LevelData.maxProcessesInSystem;
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

            if (LevelData.timeLimit > 0)
            {
                remainingTime = Mathf.Max(remainingTime - 1, 0);
            }

            if (CheckLevelStopConditions())
            {
                if (CheckWinConditions())
                {
                    Debug.Log("Game Over! Success!");
                    IsSuccess = true;
                }
                else
                {
                    Debug.Log("Game Over! Failed!");
                    IsSuccess = false;
                }

                timeManager.PauseTimer();

                if (gameOverPanelBehavior != null)
                {
                    gameOverPanelBehavior.SetVisible(true);
                }
            }
            else
            {
                processSpawnTimer = Mathf.Max(processSpawnTimer - 1, 0);
                if (processSpawnTimer <= 0)
                {
                    if (NumProcessesInSystem < MaxProcessesInSystem)
                    {
                        if (jobQueueBehavior != null)
                        {
                            Process processToSpawn = processList.processes[Random.Range(0, processList.processes.Count)];
                            ProcessBehavior processRequest = CreateProcessBehavior(processToSpawn);
                            processRequest.ChangeState(ProcessBehavior.State.Ready);
                            jobQueueBehavior.AddProcess(processRequest);

                            ++NumProcessesInSystem;
                        }
                    }

                    processSpawnTimer = Random.Range(3, 6);
                }
            }
        }

        /// <summary>
        /// Check whether the conditions to stop the level have been met.
        /// </summary>
        /// <returns></returns>
        private bool CheckLevelStopConditions()
        {
            return CheckOneConditionSatisfied(LevelData.levelStopConditions);
        }

        /// <summary>
        /// Check whether the win conditions have been met
        /// </summary>
        /// <returns>Returns true if the win conditions have been met. False otherwise.</returns>
        private bool CheckWinConditions()
        {
            return CheckAllConditionsSatisfied(LevelData.winConditions);
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

                case "numFinishedProcesses":
                    lhs = numFinishedProcesses;
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

using System.Collections.Generic;

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
                    if (TimeElapsed <= 0)
                    {
                        cpuUtilization = 0.0f;
                    }

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

        public bool IsPaused
        {
            get;
            private set;
        } = false;

        public bool IsLevelOver
        {
            get;
            private set;
        } = false;

        public LevelDataScriptableObject LevelData
        {
            get;
            private set;
        }

        private int processSpawnTimer = 0;

        private JobQueueBehavior jobQueueBehavior;

        /// <summary>
        /// Reference to the IO queue script
        /// </summary>
        private IOQueueBehavior ioQueueBehavior;

        private TimeManager timeManager;

        /// <summary>
        /// Reference to the audio manager script
        /// </summary>
        private AudioManager audioManager;

        private GameOverPanelBehavior gameOverPanelBehavior;

        private PausePanelBehavior pausePanelBehavior;

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

        /// <summary>
        /// Reset level
        /// </summary>
        public void ResetLevel()
        {
            if (LevelData == null)
            {
                Debug.LogError("Level data is null!");
            }

            if (cpuList != null)
            {
                cpuList.SetNumCPUs(LevelData.initialProcessorCount);

                foreach (CPUBehavior cpu in cpuList.CPUs)
                {
                    cpu.ResetState();
                }
            }

            if (jobQueueBehavior != null)
            {
                jobQueueBehavior.ClearProcesses();
            }

            if (ioQueueBehavior != null)
            {
                ioQueueBehavior.ResetState();
            }

            remainingTime = LevelData.timeLimit;

            // 3-second delay before the first process spawn
            processSpawnTimer = 3;

            numFinishedProcesses = 0;
            numMissedProcesses = 0;

            NumProcessesInSystem = 0;
            MaxProcessesInSystem = LevelData.maxProcessesInSystem;

            TotalWaitingTime = 0;
            TotalTurnaroundTime = 0;
            TimeElapsed = 0;
            IsSuccess = true;
            IsLevelOver = false;

            if (gameOverPanelBehavior != null)
            {
                gameOverPanelBehavior.SetVisible(false);
            }

            if (pausePanelBehavior != null)
            {
                pausePanelBehavior.SetVisible(false);
            }

            if (timeManager != null)
            {
                timeManager.timeMultiplier = 1.0f;

                timeManager.ResetTimer();
                timeManager.StartTimer();
            }

            if (audioManager != null)
            {
                if (LevelData.backgroundMusicAudioClip != null)
                {
                    audioManager.PlayAudioClip(LevelData.backgroundMusicAudioClip, true);
                }
                else
                {
                    audioManager.PlayAudioClip(audioManager.defaultLevelAudioClip, true);
                }
            }
        }

        public void SetPaused(bool isPaused)
        {
            if (isPaused)
            {
                timeManager.PauseTimer();
                IsPaused = true;
            }
            else
            {
                if (timeManager != null)
                {
                    timeManager.ResumeTimer();
                    IsPaused = false;
                }
            }
        }

        /// <summary>
        /// End level
        /// </summary>
        public void EndLevel()
        {
            IsLevelOver = true;

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

            if (audioManager != null)
            {
                audioManager.StopCurrentAudioClip();
            }
        }

        private void Awake()
        {
            jobQueueBehavior = GameObject.FindObjectOfType<JobQueueBehavior>();

            ioQueueBehavior = GameObject.FindObjectOfType<IOQueueBehavior>();

            timeManager = GameObject.FindObjectOfType<TimeManager>();

            audioManager = GameObject.FindObjectOfType<AudioManager>();

            gameOverPanelBehavior = GameObject.FindObjectOfType<GameOverPanelBehavior>();

            pausePanelBehavior = GameObject.FindObjectOfType<PausePanelBehavior>();

            GameStateBehavior gameState = GameObject.FindObjectOfType<GameStateBehavior>();
            if (gameState != null)
            {
                LevelData = gameState.LevelData;
            }
        }

        /// <summary>
        /// Unity callback function that is called
        /// every frame.
        /// </summary>
        private void Update()
        {
            if (IsLevelOver)
            {
                return;
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (IsPaused)
                {
                    return;
                }

                if (pausePanelBehavior != null)
                {
                    pausePanelBehavior.SetVisible(true);
                    SetPaused(true);
                }
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
            ResetLevel();
        }

        private ProcessBehavior CreateProcessBehavior(ProcessTemplate process)
        {
            if (processPrefab == null)
            {
                return null;
            }

            GameObject processGO = GameObject.Instantiate(processPrefab);

            ProcessBehavior processBehavior = processGO.GetComponent<ProcessBehavior>();
            if (processBehavior != null)
            {
                processBehavior.Template = process;

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
                EndLevel();
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
                            ProcessTemplate processToSpawn = processList.processes[Random.Range(0, processList.processes.Count)];
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

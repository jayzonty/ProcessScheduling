using UnityEngine;

namespace ProcessScheduling
{
    public class GameManager : MonoBehaviour
    {
        public ProcessListScriptableObject processList;

        public GameObject processPrefab;

        public CPUListBehavior cpuList;

        public int numFinishedProcesses = 0;

        public int score = 0;

        private float timer = 0.0f;

        private JobQueueBehavior jobQueueBehavior;

        private JobRequestContainerBehavior jobRequestContainerBehavior;

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

            jobRequestContainerBehavior = GameObject.FindObjectOfType<JobRequestContainerBehavior>();
        }

        private void Update()
        {
            timer -= Time.deltaTime;
            if (timer <= 0.0f)
            {
                timer = 5.0f;

                if (jobRequestContainerBehavior != null)
                {
                    if (jobRequestContainerBehavior.CurrentProcess == null)
                    {
                        Process processToSpawn = processList.processes[Random.Range(0, processList.processes.Count)];
                        ProcessBehavior processRequest = CreateProcessBehavior(processToSpawn);
                        jobRequestContainerBehavior.SetProcess(processRequest);
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
                processBehavior.CurrentState = ProcessBehavior.State.New;
                processBehavior.RemainingBurstTime = Random.Range(process.minBurstTime, process.maxBurstTime);
                processBehavior.MinTimeUntilIOWait = process.minTimeUntilIOWait;
                processBehavior.MaxTimeUntilIOWait = process.maxTimeUntilIOWait;
                processBehavior.MinIOWaitDuration = process.minIOWaitDuration;
                processBehavior.MaxIOWaitDuration = process.maxIOWaitDuration;
            }

            return processBehavior;
        }
    }
}

using UnityEngine;

namespace ProcessScheduling
{
    public class GameManager : MonoBehaviour
    {
        public ProcessListScriptableObject processList;

        public JobQueueBehavior jobQueue;

        public CPUListBehavior cpuList;

        public int numFinishedProcesses = 0;

        public int score = 0;

        private float timer = 0.0f;

        public void AddProcessToJobQueue(ProcessBehavior process)
        {
            if (jobQueue != null)
            {
                jobQueue.AddProcess(process);
            }
        }

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

        private void Update()
        {
            timer -= Time.deltaTime;
            if (timer <= 0.0f)
            {
                timer = 5.0f;

                Process processToSpawn = processList.processes[Random.Range(0, processList.processes.Count)];
                if (jobQueue != null)
                {
                    jobQueue.AddProcess(processToSpawn);
                }
            }
        }
    }
}

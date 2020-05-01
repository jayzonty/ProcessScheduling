using UnityEngine;

namespace ProcessScheduling
{
    public class GameManager : MonoBehaviour
    {
        public ProcessListScriptableObject processList;

        public JobQueueBehavior jobQueue;

        private float timer = 0.0f;

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

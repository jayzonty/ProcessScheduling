using UnityEngine;

namespace ProcessScheduling
{
    public class JobQueueBehavior : MonoBehaviour
    {
        public GameObject processPrefab;

        public void AddProcess(Process process)
        {
            if (processPrefab == null)
            {
                return;
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

                processGO.transform.SetParent(this.transform, false);
            }
        }
    }
}

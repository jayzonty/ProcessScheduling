using UnityEngine;
using UnityEngine.EventSystems;

namespace ProcessScheduling
{
    public class JobQueueBehavior : MonoBehaviour
    {
        public Transform contentTransform;

        public int NumProcessesInQueue
        {
            get
            {
                if (contentTransform != null)
                {
                    return contentTransform.childCount;
                }

                return 0;
            }
        }

        public void AddProcess(ProcessBehavior process)
        {
            if (contentTransform == null)
            {
                return;
            }

            process.transform.SetParent(contentTransform, false);
        }

        /// <summary>
        /// Clear all the processes in the job queue
        /// </summary>
        public void ClearProcesses()
        {
            if (contentTransform == null)
            {
                return;
            }

            for (int i = 0; i < contentTransform.childCount; ++i)
            {
                Transform childTransform = contentTransform.GetChild(i);
                Destroy(childTransform.gameObject);
            }
        }
    }
}

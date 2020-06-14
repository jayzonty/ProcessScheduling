using UnityEngine;

namespace ProcessScheduling
{
    public class CPUListBehavior : MonoBehaviour
    {
        public int initialCPUCount = 2;

        private CPUBehavior[] cpus;
        
        public int GetNumCPUs()
        {
            int ret = 0;
            foreach (CPUBehavior cpu in cpus)
            {
                if (cpu.gameObject.activeInHierarchy)
                {
                    ++ret;
                }
            }

            return ret;
        }

        public void SetNumCPUs(int numCPUs)
        {
            int maxCPUs = cpus.Length;

            numCPUs = Mathf.Min(numCPUs, maxCPUs);
            
            for (int i = 0; i < numCPUs; ++i)
            {
                cpus[i].gameObject.SetActive(true);
            }

            for (int i = numCPUs; i < maxCPUs; ++i)
            {
                cpus[i].gameObject.SetActive(false);
            }
        }

        private void Awake()
        {
            cpus = GetComponentsInChildren<CPUBehavior>();
            SetNumCPUs(initialCPUCount);
        }
    }
}

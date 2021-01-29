using UnityEngine;

namespace ProcessScheduling
{
    public class CPUListBehavior : MonoBehaviour
    {
        public int initialCPUCount = 2;

        public CPUBehavior[] CPUs
        {
            get;
            private set;
        }
        
        public int GetNumCPUs()
        {
            int ret = 0;
            foreach (CPUBehavior cpu in CPUs)
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
            int maxCPUs = CPUs.Length;

            numCPUs = Mathf.Min(numCPUs, maxCPUs);
            
            for (int i = 0; i < numCPUs; ++i)
            {
                CPUs[i].gameObject.SetActive(true);
            }

            for (int i = numCPUs; i < maxCPUs; ++i)
            {
                CPUs[i].gameObject.SetActive(false);
            }
        }

        private void Awake()
        {
            CPUs = GetComponentsInChildren<CPUBehavior>();
            SetNumCPUs(initialCPUCount);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProcessScheduling
{
    [System.Serializable]
    public class Process
    {
        public string name;

        public int minBurstTime;
        public int maxBurstTime;

        public int minTimeUntilIOWait;
        public int maxTimeUntilIOWait;

        public int minIOWaitDuration;
        public int maxIOWaitDuration;
    }

    [CreateAssetMenu(fileName = "ProcessList", menuName = "Game/ProcessList")]
    public class ProcessListScriptableObject : ScriptableObject
    {
        public List<Process> processes;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProcessScheduling
{
    [System.Serializable]
    public class Process
    {
        public string name;

        public float minBurstTime;
        public float maxBurstTime;

        public float minTimeUntilIOWait;
        public float maxTimeUntilIOWait;

        public float minIOWaitDuration;
        public float maxIOWaitDuration;
    }

    [CreateAssetMenu(fileName = "ProcessList", menuName = "Game/ProcessList")]
    public class ProcessListScriptableObject : ScriptableObject
    {
        public List<Process> processes;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProcessScheduling
{
    [System.Serializable]
    public class ProcessTemplate
    {
        public string name;

        public int minBurstTime;
        public int maxBurstTime;

        /// <summary>
        /// Probability that the process will request for I/O
        /// whenever an I/O request check is made.
        /// </summary>
        public float ioRequestProbability;
    }

    [CreateAssetMenu(fileName = "ProcessList", menuName = "Game/ProcessList")]
    public class ProcessListScriptableObject : ScriptableObject
    {
        public List<ProcessTemplate> processes;
    }
}

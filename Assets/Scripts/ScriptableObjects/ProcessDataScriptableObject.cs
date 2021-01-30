using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProcessScheduling
{
    /// <summary>
    /// Class for a process data scriptable object
    /// </summary>
    [CreateAssetMenu(fileName = "Process", menuName = "Game/Process Data")]
    public class ProcessDataScriptableObject : ScriptableObject
    {
        /// <summary>
        /// Process name
        /// </summary>
        public string processName;

        /// <summary>
        /// Minimum burst time
        /// </summary>
        public int minBurstTime;

        /// <summary>
        /// Maximum burst time
        /// </summary>
        public int maxBurstTime;

        /// <summary>
        /// Minimum amount of time until the next IO wait
        /// </summary>
        public int minTimeUntilIOWait;

        /// <summary>
        /// Maximum amount of time until the next IO wait
        /// </summary>
        public int maxTimeUntilIOWait;
    }
}

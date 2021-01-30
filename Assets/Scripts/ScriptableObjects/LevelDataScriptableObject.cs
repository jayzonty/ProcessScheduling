using System.Collections.Generic;
using UnityEngine;

namespace ProcessScheduling
{
    /// <summary>
    /// Class for a level data scriptable object
    /// </summary>
    [CreateAssetMenu(fileName = "LevelData", menuName = "Game/Level Data")]
    public class LevelDataScriptableObject : ScriptableObject
    {
        /// <summary>
        /// Level name
        /// </summary>
        public string levelName;

        /// <summary>
        /// Level description
        /// </summary>
        [TextArea]
        public string levelDescription;

        /// <summary>
        /// Level time limit
        /// </summary>
        public int timeLimit = 180;

        /// <summary>
        /// Initial processor count
        /// </summary>
        public int initialProcessorCount = 2;

        /// <summary>
        /// Maximum number of missable processes
        /// </summary>
        public int maxMissableProcesses = 5;

        /// <summary>
        /// Maximum number of processes in the system
        /// </summary>
        public int maxProcessesInSystem = 5;

        /// <summary>
        /// Level stop conditions
        /// </summary>
        public List<Condition> levelStopConditions;

        /// <summary>
        /// Level win conditions
        /// </summary>
        public List<Condition> winConditions;

        /// <summary>
        /// List of process data templates
        /// </summary>
        public List<ProcessDataScriptableObject> processTemplates;
    }
}

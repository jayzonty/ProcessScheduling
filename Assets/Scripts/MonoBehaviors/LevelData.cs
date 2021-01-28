using System.Collections.Generic;

namespace ProcessScheduling
{
    /// <summary>
    /// Class containing data related to the level
    /// </summary>
    [System.Serializable]
    public class LevelData
    {
        /// <summary>
        /// Time limit
        /// </summary>
        public int timeLimit = 180;

        /// <summary>
        /// Number of processors
        /// </summary>
        public int numProcessors = 2;

        /// <summary>
        /// Max missable processes
        /// </summary>
        public int maxMissableProcesses = 5;

        /// <summary>
        /// Conditions to stop the level
        /// </summary>
        public List<Condition> levelStopConditions;

        /// <summary>
        /// Win conditions
        /// </summary>
        public List<Condition> winConditions;
    }
}

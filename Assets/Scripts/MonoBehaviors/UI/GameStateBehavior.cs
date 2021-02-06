using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProcessScheduling
{
    /// <summary>
    /// Script containing data and behavior related to
    /// the game state.
    /// </summary>
    public class GameStateBehavior : MonoBehaviour
    {
        /// <summary>
        /// Level data scriptable object
        /// </summary>
        public LevelDataScriptableObject LevelData
        {
            get;
            set;
        } = null;

        /// <summary>
        /// Unity callback function called before the first
        /// Update() call
        /// </summary>
        private void Start()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}

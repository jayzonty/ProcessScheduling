using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProcessScheduling
{
    /// <summary>
    /// Script containing data and behavior related to the
    /// level list panel.
    /// </summary>
    public class LevelListPanelBehavior : MonoBehaviour
    {
        /// <summary>
        /// Transform component of the game object that
        /// will contain the list of level data
        /// </summary>
        public Transform listContentTransform;

        /// <summary>
        /// Prefab for the level data panel
        /// </summary>
        public GameObject levelDataPanelPrefab;

        /// <summary>
        /// List of level data
        /// </summary>
        public List<LevelDataScriptableObject> levelDataList;

        /// <summary>
        /// Unity callback function called before the first
        /// Update() call.
        /// </summary>
        private void Start()
        {
            if (listContentTransform == null)
            {
                return;
            }

            if (levelDataPanelPrefab == null)
            {
                return;
            }

            foreach (LevelDataScriptableObject levelData in levelDataList)
            {
                GameObject levelDataPanelGameObject = GameObject.Instantiate(levelDataPanelPrefab, listContentTransform, false);

                LevelDataPanelBehavior levelDataPanelBehavior = levelDataPanelGameObject.GetComponent<LevelDataPanelBehavior>();
                if (levelDataPanelBehavior != null)
                {
                    levelDataPanelBehavior.levelData = levelData;
                }
            }
        }
    }
}

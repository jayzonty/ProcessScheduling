using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ProcessScheduling
{
    /// <summary>
    /// Script containing data and behavior for a level data panel
    /// </summary>
    public class LevelDataPanelBehavior : MonoBehaviour
    {
        /// <summary>
        /// Reference to the text displaying the level name
        /// </summary>
        public Text levelNameText;

        /// <summary>
        /// Reference to the text displaying the level description
        /// </summary>
        public Text levelDescriptionText;

        /// <summary>
        /// Reference to the play level button
        /// </summary>
        public Button playLevelButton;

        /// <summary>
        /// Level data scriptable object
        /// </summary>
        public LevelDataScriptableObject levelData;

        /// <summary>
        /// Unity callback function called when this script is enabled.
        /// </summary>
        private void OnEnable()
        {
            if (playLevelButton != null)
            {
                playLevelButton.onClick.AddListener(OnPlayButtonClicked);
            }
        }

        /// <summary>
        /// Unity callback function called when this script is disabled.
        /// </summary>
        private void OnDisable()
        {
            if (playLevelButton != null)
            {
                playLevelButton.onClick.RemoveListener(OnPlayButtonClicked);
            }
        }

        /// <summary>
        /// Unity callback function called before the first
        /// Update() call.
        /// </summary>
        private void Start()
        {
            if (levelData == null)
            {
                return;
            }

            if (levelNameText != null)
            {
                levelNameText.text = levelData.levelName;
            }

            if (levelDescriptionText != null)
            {
                levelDescriptionText.text = levelData.levelDescription;
            }
        }

        /// <summary>
        /// Handler function for when the play level button was clicked
        /// </summary>
        private void OnPlayButtonClicked()
        {
            GameStateBehavior gameStateBehavior = GameObject.FindObjectOfType<GameStateBehavior>();
            if (gameStateBehavior != null)
            {
                gameStateBehavior.LevelData = levelData;
            }

            SceneManager.LoadScene("SampleScene");
        }
    }
}

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ProcessScheduling
{
    /// <summary>
    /// Script containing data and behavior for the pause panel
    /// </summary>
    public class PausePanelBehavior : MonoBehaviour
    {
        /// <summary>
        /// Reference to the retry button
        /// </summary>
        public Button retryButton;

        /// <summary>
        /// Reference to the end level button
        /// </summary>
        public Button endLevelButton;

        /// <summary>
        /// Reference to the level select button
        /// </summary>
        public Button levelSelectButton;

        /// <summary>
        /// Reference to the cancel button
        /// </summary>
        public Button cancelButton;

        /// <summary>
        /// Reference to the game manager script
        /// </summary>
        private GameManager gameManager;

        /// <summary>
        /// Sets the visibility of this panel.
        /// </summary>
        /// <param name="isVisible">Visibility of this panel</param>
        public void SetVisible(bool isVisible)
        {
            if (isVisible)
            {
                gameObject.SetActive(true);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Unity callback function called when the script
        /// is created.
        /// </summary>
        private void Awake()
        {
            gameManager = GameObject.FindObjectOfType<GameManager>();
        }

        /// <summary>
        /// Unity callback function that is called
        /// every frame.
        /// </summary>
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                gameManager.SetPaused(false);
                SetVisible(false);
            }
        }

        /// <summary>
        /// Unity callback function called when the script
        /// is enabled.
        /// </summary>
        private void OnEnable()
        {
            if (retryButton != null)
            {
                retryButton.onClick.AddListener(OnRetryButtonClicked);
            }

            if (endLevelButton != null)
            {
                endLevelButton.onClick.AddListener(OnEndLevelButtonClicked);
            }

            if (levelSelectButton != null)
            {
                levelSelectButton.onClick.AddListener(OnLevelSelectButtonClicked);
            }

            if (cancelButton != null)
            {
                cancelButton.onClick.AddListener(OnCancelButtonClicked);
            }
        }

        /// <summary>
        /// Unity callback function called when the script
        /// is disabled.
        /// </summary>
        private void OnDisable()
        {
            if (retryButton != null)
            {
                retryButton.onClick.RemoveListener(OnRetryButtonClicked);
            }

            if (endLevelButton != null)
            {
                endLevelButton.onClick.RemoveListener(OnEndLevelButtonClicked);
            }

            if (levelSelectButton != null)
            {
                levelSelectButton.onClick.RemoveListener(OnLevelSelectButtonClicked);
            }

            if (cancelButton != null)
            {
                cancelButton.onClick.RemoveListener(OnCancelButtonClicked);
            }
        }

        /// <summary>
        /// Handler function for when the retry button was clicked.
        /// </summary>
        private void OnRetryButtonClicked()
        {
            if (gameManager != null)
            {
                gameManager.SetPaused(false);
                gameManager.ResetLevel();
                SetVisible(false);
            }
        }

        /// <summary>
        /// Handler function for when the end level button was clicked.
        /// </summary>
        private void OnEndLevelButtonClicked()
        {
            if (gameManager != null)
            {
                gameManager.SetPaused(false);
                gameManager.EndLevel();
                SetVisible(false);
            }
        }

        /// <summary>
        /// Handler function for when the level select button was clicked.
        /// </summary>
        private void OnLevelSelectButtonClicked()
        {
            SceneManager.LoadScene("LevelSelectScene");
        }

        /// <summary>
        /// Handler function for when the cancel button was clicked.
        /// </summary>
        private void OnCancelButtonClicked()
        {
            gameManager.SetPaused(false);
            SetVisible(false);
        }
    }
}

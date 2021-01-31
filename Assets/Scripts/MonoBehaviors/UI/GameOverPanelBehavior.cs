using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ProcessScheduling
{
    /// <summary>
    /// Script containing data and behavior for the game over panel
    /// </summary>
    public class GameOverPanelBehavior : MonoBehaviour
    {
        /// <summary>
        /// Reference to the text displaying the title of the panel
        /// </summary>
        public Text titleText;

        /// <summary>
        /// Reference to the text displaying the time elapsed
        /// </summary>
        public Text timeElapsedText;

        /// <summary>
        /// Reference to the text displaying the number of finished processes
        /// </summary>
        public Text finishedProcessesText;

        /// <summary>
        /// Reference to the text displaying the number of missed processes
        /// </summary>
        public Text missedProcessesText;

        /// <summary>
        /// Reference to the text displaying the CPU utilization
        /// </summary>
        public Text cpuUtilizationText;

        /// <summary>
        /// Reference to the text displaying the throughput
        /// </summary>
        public Text throughputText;

        /// <summary>
        /// Reference to the text displaying the average waiting time
        /// </summary>
        public Text averageWaitingTimeText;

        /// <summary>
        /// Reference to the text displaying the average turnaround time
        /// </summary>
        public Text averageTurnaroundTimeText;

        /// <summary>
        /// Reference to the retry button
        /// </summary>
        public Button retryButton;

        /// <summary>
        /// Reference to the level select button
        /// </summary>
        public Button levelSelectButton;

        /// <summary>
        /// Reference to the canvas group component
        /// </summary>
        private CanvasGroup canvasGroup;

        /// <summary>
        /// Reference to the game manager script
        /// </summary>
        private GameManager gameManager;

        /// <summary>
        /// Specifies whether the panel is visible or not.
        /// </summary>
        /// <param name="isVisible">Flag indicating whether the panel is visible or not.</param>
        public void SetVisible(bool isVisible)
        {
            if (canvasGroup != null)
            {
                if (isVisible)
                {
                    OnShow();
                    gameObject.SetActive(true);
                }
                else
                {
                    OnHide();
                    gameObject.SetActive(false);
                }
            }
        }

        /// <summary>
        /// Function called when this panel is about to be shown
        /// </summary>
        private void OnShow()
        {
            if (gameManager == null)
            {
                return;
            }

            if (titleText != null)
            {
                if (gameManager.IsSuccess)
                {
                    titleText.text = "Level Success!";
                }
                else
                {
                    titleText.text = "Level Failed!";
                }
            }

            if (timeElapsedText != null)
            {
                timeElapsedText.text = "Time Elapsed: " + gameManager.TimeElapsed;
            }

            if (finishedProcessesText != null)
            {
                finishedProcessesText.text = "Finished processes: " + gameManager.numFinishedProcesses;
            }

            if (missedProcessesText != null)
            {
                missedProcessesText.text = "Missed processes: " + gameManager.numMissedProcesses;
                if (gameManager.LevelData.maxMissableProcesses > 0)
                {
                    missedProcessesText.text += "/" + gameManager.LevelData.maxMissableProcesses;
                }
            }

            if (cpuUtilizationText != null)
            {
                float cpuUtilization = gameManager.CPUUtilization;
                cpuUtilizationText.text = "CPU Utilization: " + Mathf.FloorToInt(cpuUtilization * 100.0f) + "%";
                cpuUtilizationText.text += "\n(Percentage of the time the CPUs are not idle)";
            }

            if (throughputText != null)
            {
                float throughput = (gameManager.numFinishedProcesses * 1.0f) / gameManager.TimeElapsed;
                throughputText.text = "Throughput: " + throughput.ToString("F2") + " processes per second";
                throughputText.text += "\n(Number of processes finished per second)";
            }

            if (averageWaitingTimeText != null)
            {
                float averageWaitingTime = 0.0f;
                if (gameManager.numFinishedProcesses > 0)
                {
                    averageWaitingTime = (gameManager.TotalWaitingTime * 1.0f) / gameManager.numFinishedProcesses;
                }

                averageWaitingTimeText.text = "Average waiting time: " + averageWaitingTime.ToString("F2") + " seconds";
                averageWaitingTimeText.text += "\n(Average amount of time processes spend in the job queue)";
            }

            if (averageTurnaroundTimeText != null)
            {
                float averageTurnaroundTime = 0.0f;
                if (gameManager.numFinishedProcesses > 0)
                {
                    averageTurnaroundTime = (gameManager.TotalTurnaroundTime * 1.0f) / gameManager.numFinishedProcesses;
                }

                averageTurnaroundTimeText.text = "Average turnaround time: " + averageTurnaroundTime.ToString("F2") + " seconds";
                averageTurnaroundTimeText.text += "\n(Average amount of time processes take to finish execution)";
            }
        }

        /// <summary>
        /// Function called when this panel is about to be hidden
        /// </summary>
        private void OnHide()
        {

        }

        /// <summary>
        /// Unity callback function called when the script
        /// is created.
        /// </summary>
        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();

            gameManager = GameObject.FindObjectOfType<GameManager>();
        }

        /// <summary>
        /// Unity callback function called before the first
        /// Update() call.
        /// </summary>
        private void Start()
        {
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

            if (levelSelectButton != null)
            {
                levelSelectButton.onClick.AddListener(OnLevelSelectButtonClicked);
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

            if (levelSelectButton != null)
            {
                levelSelectButton.onClick.RemoveListener(OnLevelSelectButtonClicked);
            }
        }

        /// <summary>
        /// Handler function for when the retry button was clicked
        /// </summary>
        private void OnRetryButtonClicked()
        {
            if (gameManager != null)
            {
                gameManager.ResetLevel();
            }
        }

        /// <summary>
        /// Handler function for when the level select button was clicked
        /// </summary>
        private void OnLevelSelectButtonClicked()
        {
            SceneManager.LoadScene("LevelSelectScene");
        }
    }
}

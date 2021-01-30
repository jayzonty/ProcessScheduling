using UnityEngine;
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
                    canvasGroup.alpha = 1.0f;
                }
                else
                {
                    OnHide();
                    canvasGroup.alpha = 0.0f;
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
            }

            if (throughputText != null)
            {
                float throughput = (gameManager.numFinishedProcesses * 1.0f) / gameManager.TimeElapsed;
                throughputText.text = throughput.ToString("F2") + " processes per second";
            }

            if (averageWaitingTimeText != null)
            {
                float averageWaitingTime = 0.0f;
                if (gameManager.numFinishedProcesses > 0)
                {
                    averageWaitingTime = (gameManager.TotalWaitingTime * 1.0f) / gameManager.numFinishedProcesses;
                }

                averageWaitingTimeText.text = "Average waiting time: " + averageWaitingTime.ToString("F2") + " seconds";
            }

            if (averageTurnaroundTimeText != null)
            {
                float averageTurnaroundTime = 0.0f;
                if (gameManager.numFinishedProcesses > 0)
                {
                    averageTurnaroundTime = (gameManager.TotalTurnaroundTime * 1.0f) / gameManager.numFinishedProcesses;
                }

                averageTurnaroundTimeText.text = "Average turnaround time: " + averageTurnaroundTime.ToString("F2") + " seconds";
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
            SetVisible(false);
        }
    }
}

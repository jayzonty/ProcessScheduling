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
        public Text averageTurnaroundTime;

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
                    canvasGroup.alpha = 1.0f;
                }
                else
                {
                    canvasGroup.alpha = 0.0f;
                }
            }
        }

        /// <summary>
        /// Function called when this panel is about to be shown
        /// </summary>
        private void OnShow()
        {
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

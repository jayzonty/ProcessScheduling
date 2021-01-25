using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

namespace ProcessScheduling
{
    /// <summary>
    /// Script containing behavor of the new job request container
    /// </summary>
    public class JobRequestContainerBehavior : MonoBehaviour
    {
        /// <summary>
        /// Amount of time before the new job request expires
        /// </summary>
        public int newRequestExpiryInTicks = 10;

        /// <summary>
        /// Parent transform of the game object for the
        /// new process request
        /// </summary>
        public Transform processParent;

        /// <summary>
        /// Reference to the text component for displaying the
        /// job request expiry text
        /// </summary>
        public Text requestExpiryText;

        /// <summary>
        /// Current process
        /// </summary>
        public ProcessBehavior CurrentProcess
        {
            get;
            private set;
        }

        /// <summary>
        /// Reference to the time manager script
        /// </summary>
        private TimeManager timeManager;

        /// <summary>
        /// Timer for the job request expiry
        /// </summary>
        private int requestExpiryTimer;

        /// <summary>
        /// Sets the process currently requesting to run
        /// </summary>
        /// <param name="processBehavior">Process requesting to run</param>
        public void SetProcess(ProcessBehavior processBehavior)
        {
            CurrentProcess = processBehavior;

            if (CurrentProcess == null)
            {
                requestExpiryText?.gameObject.SetActive(false);
            }
            else
            {
                requestExpiryTimer = newRequestExpiryInTicks;
                requestExpiryText?.gameObject.SetActive(true);
                UpdateRequestExpiryText("Request Expiry: " + requestExpiryTimer);

                CurrentProcess.transform.SetParent(processParent, false);
                CurrentProcess.transform.localPosition = Vector3.zero;
            }
        }

        /// <summary>
        /// Unity callback function called when the script
        /// is created.
        /// </summary>
        private void Awake()
        {
            timeManager = GameObject.FindObjectOfType<TimeManager>();
        }

        /// <summary>
        /// Unity callback function called when the script
        /// is enabled.
        /// </summary>
        private void OnEnable()
        {
            if (timeManager != null)
            {
                timeManager.TimerTick += TimeManager_TimerTick;
            }
        }

        /// <summary>
        /// Unity callback function called when the script
        /// is disabled.
        /// </summary>
        private void OnDisable()
        {
            if (timeManager != null)
            {
                timeManager.TimerTick -= TimeManager_TimerTick;
            }
        }

        /// <summary>
        /// Callback function per in-game time tick
        /// </summary>
        /// <param name="ticksSinceStart">Number of ticks since the in-game timer started</param>
        private void TimeManager_TimerTick(int ticksSinceStart)
        {
            if (CurrentProcess != null)
            {
                --requestExpiryTimer;
                if (requestExpiryTimer <= 0)
                {
                    Destroy(CurrentProcess.gameObject);

                    requestExpiryText?.gameObject.SetActive(false);
                }
                else
                {
                    UpdateRequestExpiryText("Request Expiry: " + requestExpiryTimer);
                }
            }
        }

        /// <summary>
        /// Update the text display for the request expiry timer
        /// </summary>
        /// <param name="text">New text</param>
        private void UpdateRequestExpiryText(string text)
        {
            if (requestExpiryText != null)
            {
                requestExpiryText.text = text;
            }
        }
    }
}

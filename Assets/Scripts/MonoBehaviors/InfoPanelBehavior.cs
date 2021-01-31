using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ProcessScheduling
{
    public class InfoPanelBehavior : MonoBehaviour
    {
        public Text finishedProcessText;
        public Text missedProcessesText;
        public Text timeText;
        public Text timeMultiplierText;

        private GameManager gameManager;
        private LevelManager levelManager;
        private TimeManager timeManager;

        private void Awake()
        {
            gameManager = GameObject.FindObjectOfType<GameManager>();
            levelManager = GameObject.FindObjectOfType<LevelManager>();
            timeManager = GameObject.FindObjectOfType<TimeManager>();
        }

        private void Update()
        {
            if (gameManager == null)
            {
                return;
            }

            if (finishedProcessText != null)
            {
                finishedProcessText.text = "Finished: " + gameManager.numFinishedProcesses;
            }

            if (missedProcessesText != null)
            {
                missedProcessesText.text = "Missed: " + gameManager.numMissedProcesses + "/" + gameManager.LevelData.maxMissableProcesses;
            }

            if (timeText != null)
            {
                if (gameManager.LevelData != null)
                {
                    if (gameManager.LevelData.timeLimit == 0)
                    {
                        timeText.text = "Time Elapsed: " + gameManager.TimeElapsed;
                    }
                    else
                    {
                        timeText.text = "Time Remaining: " + gameManager.remainingTime;
                    }
                }
            }

            if (timeManager != null)
            {
                timeMultiplierText.text = "Time multiplier: x" + timeManager.timeMultiplier.ToString("F1");
            }
        }
    }
}

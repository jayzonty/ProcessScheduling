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
        public Text scoreText;
        public Text timeText;

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
                missedProcessesText.text = "Missed: " + gameManager.numMissedProcesses + "/" + gameManager.levelData.maxMissableProcesses;
            }

            if (scoreText != null)
            {
                scoreText.text = "Score: " + gameManager.score;
            }

            if (timeManager != null)
            {
                timeText.text = "Time: " + gameManager.remainingTime + " (x" + timeManager.timeMultiplier.ToString("F1") + ")";
            }
        }
    }
}

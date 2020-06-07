using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ProcessScheduling
{
    public class InfoPanelBehavior : MonoBehaviour
    {
        public Text finishedProcessText;

        private GameManager gameManager;

        private void Awake()
        {
            gameManager = GameObject.FindObjectOfType<GameManager>();
        }

        private void Update()
        {
            if (gameManager == null)
            {
                return;
            }

            if (finishedProcessText != null)
            {
                finishedProcessText.text = "Finished Processes: " + gameManager.numFinishedProcesses;
            }
        }
    }
}

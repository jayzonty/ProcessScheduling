using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ProcessScheduling
{
    public class AddCPUButtonBehavior : MonoBehaviour
    {
        private Button button;

        private GameManager gameManager;

        private void Awake()
        {
            button = GetComponent<Button>();

            gameManager = GameObject.FindObjectOfType<GameManager>();
        }

        private void OnEnable()
        {
            if (button != null)
            {
                button.onClick.AddListener(HandleClick);
            }
        }

        private void OnDisable()
        {
            if (button != null)
            {
                button.onClick.RemoveListener(HandleClick);
            }
        }

        private void HandleClick()
        {
            if (gameManager != null)
            {
                gameManager.AddCPU();
            }
        }
    }
}

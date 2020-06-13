using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

namespace ProcessScheduling
{
    public class TimePlayPauseButtonBehavior : MonoBehaviour
    {
        public Sprite playSprite;
        public Sprite pauseSprite;

        private Image m_image;
        private Button m_button;

        private TimeManager m_timeManager;

        private float m_originalMultiplier = 1.0f;

        private void Awake()
        {
            m_timeManager = GameObject.FindObjectOfType<TimeManager>();

            m_button = GetComponent<Button>();
            m_button.onClick.AddListener(HandleClick);

            m_image = GetComponent<Image>();

            /*GameState gameState = GameObject.FindObjectOfType<GameState>();
            if (gameState != null)
            {
                gameState.GameOverHandlers += HandleGameOver;
            }*/
        }

        private void HandleGameOver()
        {
            m_button.interactable = false;
        }

        private void Start()
        {
            m_image.sprite = pauseSprite;
        }

        private void HandleClick()
        {
            if (m_timeManager.timeMultiplier > 0.0f)
            {
                m_originalMultiplier = m_timeManager.timeMultiplier;

                m_timeManager.timeMultiplier = 0.0f;

                m_image.sprite = playSprite;
            }
            else
            {
                m_timeManager.timeMultiplier = m_originalMultiplier;

                m_image.sprite = pauseSprite;
            }
        }

        public void UpdateSprite()
        {
            if (m_timeManager.timeMultiplier > 0.0f)
            {
                m_image.sprite = pauseSprite;
            }
            else
            {
                m_image.sprite = playSprite;
            }
        }
    }
}

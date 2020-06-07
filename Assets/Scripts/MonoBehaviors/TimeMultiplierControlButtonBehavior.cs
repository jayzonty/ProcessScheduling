using UnityEngine;
using UnityEngine.UI;

namespace ProcessScheduling
{
    public class TimeMultiplierControlButtonBehavior : MonoBehaviour
    {
        private Button m_button;

        private TimeManager m_timeManager;

        public float timeMultiplierIncrement = 1.0f;

        private void Awake()
        {
            m_timeManager = GameObject.FindObjectOfType<TimeManager>();

            m_button = GetComponent<Button>();
            m_button.onClick.AddListener(HandleClick);

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
        }

        private void Update()
        {
            if (timeMultiplierIncrement < 0.0f)
            {
                m_button.interactable = (m_timeManager.timeMultiplier > 1.0f);
            }
            else if (timeMultiplierIncrement > 0.0f)
            {
                m_button.interactable = (m_timeManager.timeMultiplier < m_timeManager.maxMultiplier);
            }
        }

        private void HandleClick()
        {
            m_timeManager.timeMultiplier = Mathf.Clamp(m_timeManager.timeMultiplier + timeMultiplierIncrement, 1.0f, m_timeManager.maxMultiplier);

            PlayButtonBehavior playButton = GameObject.FindObjectOfType<PlayButtonBehavior>();
            if (playButton != null)
            {
                playButton.UpdateSprite();
            }
        }
    }
}

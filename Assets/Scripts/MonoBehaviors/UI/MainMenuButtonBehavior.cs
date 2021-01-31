using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ProcessScheduling
{
    public class MainMenuButtonBehavior : MonoBehaviour
    {
        /// <summary>
        /// Reference to the button component
        /// </summary>
        private Button buttonComponent;

        /// <summary>
        /// Unity callback function called when the script
        /// is created.
        /// </summary>
        private void Awake()
        {
            buttonComponent = GetComponent<Button>();
        }

        /// <summary>
        /// Unity callback function called when the script
        /// is enabled.
        /// </summary>
        private void OnEnable()
        {
            if (buttonComponent != null)
            {
                buttonComponent.onClick.AddListener(OnButtonClick);
            }
        }

        /// <summary>
        /// Unity callback function called when the script
        /// is disabled.
        /// </summary>
        private void OnDisable()
        {
            if (buttonComponent != null)
            {
                buttonComponent.onClick.AddListener(OnButtonClick);
            }
        }

        /// <summary>
        /// Handler function for when the button was clicked.
        /// </summary>
        private void OnButtonClick()
        {
            SceneManager.LoadScene("TitleScene");
        }
    }
}

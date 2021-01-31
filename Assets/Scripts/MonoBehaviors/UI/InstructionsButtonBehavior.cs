using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ProcessScheduling
{
    /// <summary>
    /// Script containing data and behavior for the instructions button
    /// </summary>
    public class InstructionsButtonBehavior : MonoBehaviour
    {
        /// <summary>
        /// Reference to the button component script
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
                buttonComponent.onClick.AddListener(OnButtonClicked);
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
                buttonComponent.onClick.RemoveListener(OnButtonClicked);
            }
        }

        /// <summary>
        /// Handler function for when the button was clicked
        /// </summary>
        private void OnButtonClicked()
        {
            SceneManager.LoadScene("InstructionsScene");
        }
    }
}

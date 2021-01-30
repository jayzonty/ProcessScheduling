using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ProcessScheduling
{
    [RequireComponent(typeof(Button))]
    public class PlayButtonBehavior : MonoBehaviour
    {
        private Button buttonComponent;

        private void Awake()
        {
            buttonComponent = GetComponent<Button>();
        }

        private void OnEnable()
        {
            buttonComponent.onClick.AddListener(ClickHandler);
        }

        private void OnDisable()
        {
            buttonComponent.onClick.RemoveListener(ClickHandler);
        }

        private void ClickHandler()
        {
            SceneManager.LoadScene("LevelSelectScene");
        }
    }
}

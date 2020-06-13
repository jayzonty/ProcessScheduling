using UnityEngine;
using UnityEngine.UI;

namespace ProcessScheduling
{
    [RequireComponent(typeof(Button))]
    public class ExitButtonBehavior : MonoBehaviour
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
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}

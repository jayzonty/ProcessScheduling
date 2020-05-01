using UnityEngine;

namespace ProcessScheduling
{
    public class TimeManager : MonoBehaviour
    {
        [SerializeField]
        private float timeMultiplier = 1.0f;

        public float TimeMultiplier
        {
            get
            {
                return timeMultiplier;
            }

            set
            {
                timeMultiplier = value;
            }
        }
    }
}

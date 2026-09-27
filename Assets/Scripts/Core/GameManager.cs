using UnityEngine;

namespace SpiderDud.Core
{
    /// <summary>
    /// Minimal scene-lifetime singleton. Placeholder hook for future game
    /// state (scoring, mission tracking, pause menus) as the prototype grows.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }
    }
}

using UnityEngine;

namespace SpiderDud.Player
{
    /// <summary>
    /// Safety net for a swinging game: if the player falls past the city
    /// floor (a missed swing, falling between buildings, etc.) teleport
    /// them back to their spawn point instead of falling forever.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class PlayerRespawn : MonoBehaviour
    {
        [SerializeField] private float fallDeathHeight = -50f;

        private CharacterController controller;
        private Vector3 spawnPoint;

        private void Awake()
        {
            controller = GetComponent<CharacterController>();
            spawnPoint = transform.position;
        }

        private void Update()
        {
            if (transform.position.y < fallDeathHeight)
            {
                Respawn();
            }
        }

        private void Respawn()
        {
            // Disable the controller while teleporting so it doesn't fight the move.
            controller.enabled = false;
            transform.position = spawnPoint;
            controller.enabled = true;
        }
    }
}

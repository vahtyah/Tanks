using UnityEngine;

namespace Testing
{
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 5f;
        public bool HasShootingInput { get; private set; }

        private void Update()
        {
            float horizontalInput = InputSimulator.GetAxisValue("Horizontal");
            float verticalInput = InputSimulator.GetAxisValue("Vertical");
            Vector3 movement = new Vector3(horizontalInput, 0f, verticalInput) * moveSpeed * Time.deltaTime;
            transform.Translate(movement);
            
            float shootInput = InputSimulator.GetAxisValue("Fire1");
            HasShootingInput = shootInput > 0;
        }

        // Helper method for testing
        public Vector3 GetMovementFromInput(float horizontal, float vertical)
        {
            return new Vector3(horizontal, 0f, vertical) * moveSpeed * Time.deltaTime;
        }
    }
}
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5.0f;           // Velocidad de movimiento del personaje
    public float rotationSpeed = 10.0f;      // Velocidad de rotación (factor de Slerp)

    private Rigidbody _charRb;               // Referencia al componente Rigidbody

    void Start()
    {
        _charRb = GetComponent<Rigidbody>();
    }

    // FixedUpdate es recomendable para operaciones de física
    void FixedUpdate()
    {
      
        float horizontalInput = Input.GetAxis("Horizontal"); 
        float verticalInput = Input.GetAxis("Vertical");   

        // 2. Calcular el vector de movimiento en el plano XZ (horizontal)
        // Similar a tu 'new Vector3(inputVector.x, 0f, inputVector.y)'
        Vector3 moveDirection = new Vector3(horizontalInput, 0f, verticalInput).normalized;

        // 3. Calcular la velocidad de movimiento
        Vector3 moveVelocity = moveDirection * moveSpeed;

        // 4. Calcular la nueva posición y aplicar el movimiento al Rigidbody
        Vector3 newPosition = _charRb.position + moveVelocity * Time.fixedDeltaTime;
        _charRb.MovePosition(newPosition);

        // 5. Rotar el personaje para que mire en la dirección del movimiento
        // Solo si hay movimiento para evitar que el personaje rote cuando está quieto
        // Usamos un pequeño umbral para evitar rotaciones por ruido de input o cuando el movimiento es mínimo
        if (moveDirection.magnitude > 0.01f)
        {
            // Calcula la rotación deseada
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            Quaternion smoothedRotation = Quaternion.Slerp(_charRb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
            _charRb.MoveRotation(smoothedRotation);
        }
    }
}
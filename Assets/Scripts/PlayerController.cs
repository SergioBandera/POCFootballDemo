using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5.0f;           // Velocidad de movimiento del personaje
    public float rotationSpeed = 10.0f;      // Velocidad de rotación (factor de Slerp)

    private Rigidbody _charRb;
    [SerializeField] private Transform _ball;
    private Rigidbody _ballRb;
    private Collider _ballCol;
    private Collider _charCol;
    // Referencia al componente Rigidbody

    void Awake()
    {
        _charRb = GetComponent<Rigidbody>();
        _ballRb = _ball.GetComponent<Rigidbody>();
        _ballCol = _ball.GetComponent<Collider>();
        _charCol = GetComponent<Collider>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            OnShoot();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            OnPass();
        }
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
    private void OnShoot()
    {
        if (_ball != null)
        {
            if (_ballRb != null && _ballCol != null && _charCol != null)
            {
                _ballCol.enabled = true;
                Physics.IgnoreCollision(_ballCol, _charCol, true);
                _ballRb.isKinematic = false;

                // Levanta la pelota al chutar
                Vector3 kickDirection = (transform.forward + Vector3.up * 0.3f).normalized;
                float kickForce = 10f;
                _ballRb.AddForce(kickDirection * kickForce, ForceMode.Impulse);
                //attachBallScript.ClearAttachedBall();
                //UpdateBallReferences();
                //SetControlledByAI();
            }
        }

    }

    private void OnPass()
    {
        Debug.Log("vamos a dar un pase");
        SelectTeamMateForPass();
    }
    private void SelectTeamMateForPass()
    {
        // Obtén todos los jugadores con el mismo tag (compañeros de equipo)
        GameObject[] teamMates = GameObject.FindGameObjectsWithTag(gameObject.tag);
        GameObject closestMate = null;
        float closestDistance = Mathf.Infinity;
        Vector3 passDirection = transform.forward;

        foreach (GameObject mate in teamMates)
        {
            if (mate == gameObject) continue; // Ignora a sí mismo

            Vector3 toMate = (mate.transform.position - transform.position).normalized;
            float angle = Vector3.Angle(passDirection, toMate);

            // Considera solo los que están casi en línea recta (por ejemplo, menos de 10 grados)
            if (angle < 20f)
            {
                // Comprueba si hay línea de visión usando Raycast
                RaycastHit hit;
               
;                float distance = Vector3.Distance(transform.position, mate.transform.position);
                if (!Physics.Raycast(transform.position, toMate, out hit, distance) || hit.collider.gameObject == mate)
                {
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestMate = mate;
                    }
                }
            }
        }

        if (closestMate != null)
        {
            Debug.Log($"Pasando el balón a {closestMate.name}");
            PassBallToMate(closestMate.transform);
        }
        else
        {
            Debug.Log("No hay compañero en línea recta para el pase.");
        }
    }

    private void PassBallToMate(Transform mateTransform)
    {
        
        if (_ballRb != null)
        {
            _ballCol.enabled = true;
            _ballRb.isKinematic = false;
            Vector3 passDirection = (mateTransform.position - transform.position).normalized;
            float passForce = 8f;
            _ballRb.AddForce(passDirection * passForce, ForceMode.Impulse);
        }
    }

}
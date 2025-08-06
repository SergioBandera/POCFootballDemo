using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5.0f;           // Velocidad de movimiento del personaje
    public float rotationSpeed = 10.0f;      // Velocidad de rotación (factor de Slerp)

    private Rigidbody _charRb;
    [SerializeField] private Transform _ball;
    private Rigidbody _ballRb;
    private Collider _ballCol;
    private Collider _charCol;
    private AttachBall attachBallScript;

    private bool isAutoMovingToBall = false;
    private Vector3 autoMoveTarget;

    void Awake()
    {
        _charRb = GetComponent<Rigidbody>();
        _ballRb = _ball.GetComponent<Rigidbody>();
        _ballCol = _ball.GetComponent<Collider>();
        _charCol = GetComponent<Collider>();
        attachBallScript = GetComponent<AttachBall>();
    }

    private void Update()
    {
        // Solo permite acciones si este jugador es el que tiene la pelota
        if (GameManager.Instance.GetPlayerWithBall() == attachBallScript)
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
    }

    void FixedUpdate()
    {
        if (isAutoMovingToBall)
        {
            Vector3 direction = (autoMoveTarget - transform.position).normalized;
            Vector3 moveToBallVelocity = direction * moveSpeed;
            Vector3 NewPjPosition = _charRb.position + moveToBallVelocity * Time.fixedDeltaTime;
            _charRb.MovePosition(NewPjPosition);

            // Rotar hacia la pelota
            if (direction.magnitude > 0.01f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                Quaternion smoothedRotation = Quaternion.Slerp(_charRb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
                _charRb.MoveRotation(smoothedRotation);
            }

            // Si está suficientemente cerca, para el movimiento automático
            if (Vector3.Distance(transform.position, autoMoveTarget) < 0.5f)
            {
                isAutoMovingToBall = false;
            }
            return; // No procesar input manual mientras va a por la pelota
        }

        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 moveDirection = new Vector3(horizontalInput, 0f, verticalInput).normalized;
        Vector3 moveVelocity = moveDirection * moveSpeed;
        Vector3 newPosition = _charRb.position + moveVelocity * Time.fixedDeltaTime;
        _charRb.MovePosition(newPosition);

        if (moveDirection.magnitude > 0.01f)
        {
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
                GameManager.Instance.SetPossession(null);

                Vector3 kickDirection = (transform.forward + Vector3.up * 0.3f).normalized;
                float kickForce = 10f;
                _ballRb.AddForce(kickDirection * kickForce, ForceMode.Impulse);
            }
        }
    }

    private void OnPass()
    {
        SelectTeamMateForPass();
    }

    private void SelectTeamMateForPass()
    {
        GameObject[] teamMates = GameObject.FindGameObjectsWithTag(gameObject.tag);
        GameObject closestMate = null;
        float closestDistance = Mathf.Infinity;

        foreach (GameObject mate in teamMates)
        {
            if (mate == gameObject) continue;

            Vector3 toMate = (mate.transform.position - transform.position).normalized;
            float angle = Vector3.Angle(transform.forward, toMate);

            if (angle < 20f)
            {
                float distance = Vector3.Distance(transform.position, mate.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestMate = mate;
                }
            }
        }

        if (closestMate != null)
        {
            Debug.Log($"Pasando el balón a {closestMate.name}");
            GameManager.Instance.SetPossession(null);
            PassBallToMate(closestMate.transform);

            // Hacer que el receptor vaya a por la pelota
            PlayerController mateMovement = closestMate.GetComponent<PlayerController>();
            if (mateMovement != null)
            {
                mateMovement.MoveToBall(_ball.position);
            }
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
            Vector3 passDirection = (mateTransform.position - transform.position).normalized;
            float passForce = 15f;
            _ballRb.AddForce(passDirection * passForce, ForceMode.Impulse);
        }
    }

    public void MoveToBall(Vector3 targetPosition)
    {
        isAutoMovingToBall = true;
        autoMoveTarget = targetPosition;
    }

    public void StopAutoMove()
    {
        Debug.Log("Deteniendo movimiento automático al recibir la pelota");
        isAutoMovingToBall = false;
        if (_charRb != null)
        {
            _charRb.velocity = Vector3.zero;
            _charRb.angularVelocity = Vector3.zero;
            _charRb.constraints = RigidbodyConstraints.FreezeRotation; // Opcional si no quieres más rotaciones físicas

        }
    }
}
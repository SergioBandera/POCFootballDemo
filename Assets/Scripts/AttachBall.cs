using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttachBall : MonoBehaviour
{
    private GameObject _ball;
    private Rigidbody _ballRb; 
    private Collider _ballCollider;
    private Collider _charCol;
    private float cooldownTimer = 0f;
    [SerializeField] private Transform _playerWithBall; // Asignar en inspector

    public bool hasBall { get; private set; }

    private Coroutine restoreCollisionCoroutine;

    // Update is called once per frame

    private void Update()
    {
        if (cooldownTimer > 0f)
            cooldownTimer -= Time.deltaTime;
    }

    private void FixedUpdate()
    {
        if (_ball != null && _playerWithBall != null && _ballRb.isKinematic)
        {
            _ball.transform.SetPositionAndRotation(_playerWithBall.position + _playerWithBall.forward * 0.6f + Vector3.up * 0.40f, _playerWithBall.rotation);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(cooldownTimer);
        if (collision.gameObject.CompareTag("Ball") && _ball == null && cooldownTimer <= 0f)
        {
        Debug.Log("has colisionado"+ cooldownTimer);
            _ball = collision.gameObject;
            _ballRb = _ball.GetComponent<Rigidbody>();
            _ballCollider = _ball.GetComponent<Collider>();
            _ballRb.isKinematic = true;
            _ballCollider.enabled = false;
            GameManager.Instance.SetPossession(gameObject);

        }
    }

    public void Attach()
    {
        // Detener movimiento automático al recibir la pelota
        var playerMovement = GetComponent<PlayerController>();
        if (playerMovement != null)
            playerMovement.StopAutoMove();
        hasBall = true;
        SetBallPhysicsAttached();

    }

    public void Detach()
    {
        hasBall = false;
        cooldownTimer = 0.5f; 
        SetBallPhysicsDetached();
        // Elimina la limpieza aquí, pásala a la corrutina
    }

    private void Awake()
    {
        _charCol = GetComponent<Collider>();
    }

    private void SetBallPhysicsAttached()
    {
        if (_ballRb != null)
            _ballRb.isKinematic = true;
        if (_ballCollider != null)
            _ballCollider.enabled = false;
        if (_ballCollider != null && _charCol != null)
        {
            Physics.IgnoreCollision(_ballCollider, _charCol, true);
        }
    }

    private void SetBallPhysicsDetached()
    {
        if (_ballRb != null)
            _ballRb.isKinematic = false;
        if (_ballCollider != null)
            _ballCollider.enabled = true;
        if (_ballCollider != null && _charCol != null)
        {
            // Inicia la restauración de la colisión tras un retardo
            if (restoreCollisionCoroutine != null)
                StopCoroutine(restoreCollisionCoroutine);
            restoreCollisionCoroutine = StartCoroutine(RestoreCollisionAfterDelay());
        }
    }

    private IEnumerator RestoreCollisionAfterDelay()
    {
        yield return new WaitForSeconds(0.2f); // Ajusta el tiempo si lo necesitas
        if (_ballCollider != null && _charCol != null)
        {
            Physics.IgnoreCollision(_ballCollider, _charCol, false);
        }
        _ball = null;
        _ballRb = null;
        _ballCollider = null;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttachBall : MonoBehaviour
{
    private GameObject _ball;
    private Rigidbody _ballRb; 
    private Collider _ballCollider;
    //private float attachCooldown = 0.2f;
    private float cooldownTimer = 0f;
    [SerializeField] private Transform _playerWithBall; // Asignar en inspector

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
        if (collision.gameObject.CompareTag("Ball") && _ball == null && cooldownTimer <= 0f)
        {
            _ball = collision.gameObject;
            _ballRb = _ball.GetComponent<Rigidbody>();
            _ballCollider = _ball.GetComponent<Collider>();
            _ballRb.isKinematic = true;
            _ballCollider.enabled = false;
            GameManager.Instance.SetPossession(gameObject);
            //SetTeamHaveBallByTag(gameObject.tag);
            //playerManager?.SelectPlayerWithBall();
            //PlayerController playerController = GetComponent<PlayerController>();
            //if (playerController != null)
            //{
            //playerController.UpdateBallReferences();
            //playerController.SetControlledByPlayer(true); // Ahora el jugador es controlado por el usuario
            //}
            //Debug.Log($"Jugador {gameObject.name} recibe la pelota en {transform.position}");
        }
    }
}

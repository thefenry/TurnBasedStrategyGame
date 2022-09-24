using System;
using Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float rotationSpeed = 100f;
    [SerializeField] private float zoomAmount = 1f;
    [SerializeField] private float zoomSpeed = 20f;
    [SerializeField] private float minZoomFollowOffset = 2f;
    [SerializeField] private float maxZoomFollowOffset = 12f;
    [SerializeField] private CinemachineVirtualCamera cinemachineVirtualCamera;

    private Vector3 _targetFollowOffset;
    private CinemachineTransposer _cinemachineTransposer;

    private void Start()
    {
        _cinemachineTransposer = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>();
        _targetFollowOffset = _cinemachineTransposer.m_FollowOffset;
    }

    private void Update()
    {
        HandleMovement();

        HandleRotation();

        HandleZoom();
    }

    private void HandleMovement()
    {
        Vector3 inputMoveDirection = new();

        if (Input.GetKey(KeyCode.W))
        {
            inputMoveDirection.z += 1f;
        }

        if (Input.GetKey(KeyCode.S))
        {
            inputMoveDirection.z -= 1f;
        }

        if (Input.GetKey(KeyCode.A))
        {
            inputMoveDirection.x -= 1f;
        }

        if (Input.GetKey(KeyCode.D))
        {
            inputMoveDirection.x += 1f;
        }

        Vector3 moveVector = transform.forward * inputMoveDirection.z + transform.right * inputMoveDirection.x;
        transform.position += moveVector * (moveSpeed * Time.deltaTime);
    }

    private void HandleRotation()
    {
        Vector3 rotationVector = new();

        if (Input.GetKey(KeyCode.Q))
        {
            rotationVector.y += 1f;
        }

        if (Input.GetKey(KeyCode.E))
        {
            rotationVector.y -= 1f;
        }

        transform.eulerAngles += rotationVector * (rotationSpeed * Time.deltaTime);
    }

    private void HandleZoom()
    {
        if (Input.mouseScrollDelta.y > 0)
        {
            _targetFollowOffset.y -= zoomAmount;
        }

        if (Input.mouseScrollDelta.y < 0)
        {
            _targetFollowOffset.y += zoomAmount;
        }

        _targetFollowOffset.y = Mathf.Clamp(_targetFollowOffset.y, minZoomFollowOffset, maxZoomFollowOffset);
        _cinemachineTransposer.m_FollowOffset = Vector3.Lerp(_cinemachineTransposer.m_FollowOffset, _targetFollowOffset,
            Time.deltaTime * zoomSpeed);
    }
}

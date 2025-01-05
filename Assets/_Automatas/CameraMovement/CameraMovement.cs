using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    private float _scrollMaxRange;
    private float _scrollMinRange;
    private float _scrollSpeed;
    [SerializeField] private GridDataSO _data;
    private Vector2 _initialPosition;
    private Camera _camera;

    private void Start()
    {
        _camera = Camera.main;
        SetInitialScrollValues();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _initialPosition = Input.mousePosition;
        }
        else if (Input.GetMouseButton(0))
        {
            Drag(_initialPosition, Input.mousePosition);
            MovementClamp();
        }

        if (Input.mouseScrollDelta.y != 0)
        {
            Scroll();
            ScrollClamp();
            MovementClamp();
        }
    }

    private void Scroll()
    {
        _camera.orthographicSize -= _scrollSpeed * Input.mouseScrollDelta.y;
    }

    private void Drag(Vector2 startPos, Vector2 targetPos)
    {
        Vector2 movementDirection = (startPos - targetPos).normalized;
        float movementAmount = 
            (_camera.ScreenToWorldPoint(new Vector3(startPos.x, startPos.y, 10)) - _camera.ScreenToWorldPoint(new Vector3(targetPos.x, targetPos.y, 10))).magnitude;

        Vector2 movementVector = movementDirection * movementAmount;

        _camera.transform.position = 
            new Vector3(_camera.transform.position.x + movementVector.x, _camera.transform.position.y + movementVector.y, _camera.transform.position.z);

        _initialPosition = targetPos;
    }

    private void MovementClamp()
    {
        Vector3 cameraPos = _camera.transform.position;
        Vector2 cameraHalfSize = new Vector2(_camera.aspect * _camera.orthographicSize, _camera.orthographicSize);
        float clampedX = Mathf.Clamp(cameraPos.x, -_data.Data.ModelBounds.x + cameraHalfSize.x, _data.Data.ModelBounds.x - cameraHalfSize.x);
        float clampedY = Mathf.Clamp(cameraPos.y, -_data.Data.ModelBounds.y + cameraHalfSize.y, _data.Data.ModelBounds.y - cameraHalfSize.y);
        _camera.transform.position = new Vector3(clampedX, clampedY, cameraPos.z);
    }

    private void ScrollClamp()
    {
        float clamp = Mathf.Clamp(_camera.orthographicSize, _scrollMinRange, _scrollMaxRange);
        _camera.orthographicSize = clamp;
    }

    private void SetInitialScrollValues()
    {
        _scrollMaxRange = _camera.orthographicSize;
        _scrollMinRange = (_data.Data.ModelBounds.y / (_data.Data.Resolution.y * .5f)) * 10;
        _camera.orthographicSize = _scrollMinRange;
        _scrollSpeed = _scrollMinRange;
    }
}
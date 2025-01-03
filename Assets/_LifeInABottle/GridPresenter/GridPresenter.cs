using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridPresenter : MonoBehaviour
{
    [SerializeField] private List<ColorSO> _colors;
    [SerializeField] private GameObject _model;
    [SerializeField] private GridSpawner _spawner;
    [SerializeField] private Material _material;
    private static readonly int _gridID = Shader.PropertyToID("_GridBuffer");
    private static readonly int _resolutionID = Shader.PropertyToID("_Resolution");
    private static readonly int _colorsID = Shader.PropertyToID("_Colors");
    private ComputeBuffer _colorBuffer;

    private void OnEnable()
    {
        ResizeModelFullScreen();
        SetInitialProperties();
    }

    private void Start()
    {
        SetBuffer(_spawner.GridBuffer);
    }

    public void SetBuffer(ComputeBuffer buffer)
    {
        _material.SetBuffer(_gridID, buffer);
    }

    private void SetInitialProperties()
    {
        _material.SetVector(_resolutionID, new Vector4(_spawner.Resolution.x, _spawner.Resolution.y, 0, 0));
        SetColorBuffer(0);
    }

    private void SetColorBuffer(int index)
    {
        _colorBuffer?.Dispose();
        _colorBuffer = null;

        List<Color> colorData = _colors[index].Colors;
        int length = colorData.Count;


        _colorBuffer = new ComputeBuffer(length, sizeof(float) * 4);
        _colorBuffer.SetData(colorData);
        _material.SetBuffer(_colorsID, _colorBuffer);
    }

    private void ResizeModelFullScreen()
    {
        Vector4 screenPositions = GetScreenPositions();

        float scaleX = screenPositions.z - screenPositions.x;
        float scaleY = screenPositions.w - screenPositions.y;

        _model.transform.localScale = new Vector3(scaleX, scaleY, 1);
    }

    private Vector4 GetScreenPositions()
    {
        Vector2 bottomLeft = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 10));
        Vector2 topRight = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 10));
        return new Vector4(bottomLeft.x, bottomLeft.y, topRight.x, topRight.y);
    }

    private void OnDisable()
    {
        _colorBuffer.Dispose();
        _colorBuffer = null;
    }
}

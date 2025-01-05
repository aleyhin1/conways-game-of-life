using System;
using Unity.Mathematics;
using UnityEngine;

public class SeedInput : MonoBehaviour
{
    public static Action OnSeedReady;
    [SerializeField] private ComputeShader _inputCompute;
    [SerializeField] private ComputeShader _initialStateCompute;
    [SerializeField] private GridDataSO _data;
    private int2 _resolution;
    private int _groupX;
    private int _groupY;
    private Camera _camera;
    private static readonly int _gridID = Shader.PropertyToID("_Grid");
    private static readonly int _resolutionID = Shader.PropertyToID("_Resolution");
    private static readonly int _highlightIndexID = Shader.PropertyToID("_HighlightIndex");
    private static readonly int _selectedIndexID = Shader.PropertyToID("_SelectedIndex");

    private void Start()
    {
        _camera = Camera.main;

        SetInitialFields();
        SetInitialProperties();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TransformGridToInitialState();
            OnSeedReady?.Invoke();
            Destroy(this.gameObject);
        }
        else
        {
            GetInput();
        }
    }

    private void GetInput()
    {
        Vector3 mousePosition = Input.mousePosition;
        Vector2 selectedIndex;

        if (Input.GetMouseButtonDown(0))
        {
            selectedIndex = GetGridPosition(mousePosition);
        }
        else
        {
            selectedIndex = new Vector2(-1, -1);
        }

        Vector2 highlightIndex = GetGridPosition(mousePosition);

        _inputCompute.SetVector(_highlightIndexID, new Vector4(highlightIndex.x, highlightIndex.y, 0, 0));
        _inputCompute.SetVector(_selectedIndexID, new Vector4(selectedIndex.x, selectedIndex.y, 0, 0));

        _inputCompute.Dispatch(0, _groupX, _groupY, 1);
    }

    private void TransformGridToInitialState()
    {
        _initialStateCompute.Dispatch(0, _groupX, _groupY, 1);
    }

    private Vector2 GetGridPosition(Vector3 position)
    {
        RaycastHit hitInfo;
        Physics.Raycast(_camera.ScreenPointToRay(position), out hitInfo);
        Vector2 textureCoord = hitInfo.textureCoord;
        Vector2 gridPosition = new Vector2(textureCoord.x * _resolution.x, textureCoord.y * _resolution.y);
        return gridPosition;
    }

    private void SetInitialFields()
    {
        _resolution = _data.Data.Resolution;
        _groupX = Mathf.CeilToInt(_resolution.x / 8 + 1);
        _groupY = Mathf.CeilToInt(_resolution.y / 8 + 1);
    }

    private void SetInitialProperties()
    {
        _inputCompute.SetBuffer(0, _gridID, _data.Data.GridBuffer);
        _inputCompute.SetVector(_resolutionID, new Vector4(_resolution.x, _resolution.y, 0, 0));

        _initialStateCompute.SetBuffer(0, _gridID, _data.Data.GridBuffer);
        _initialStateCompute.SetVector(_resolutionID, new Vector4(_resolution.x, _resolution.y, 0, 0));
    }
}

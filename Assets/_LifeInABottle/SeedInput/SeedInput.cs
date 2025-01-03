using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class SeedInput : MonoBehaviour
{
    [SerializeField] private GridSpawner _spawner;
    [SerializeField] private GridPresenter _presenter;
    [SerializeField] private GridLogic _logic;
    [SerializeField] private ComputeShader _inputCompute;
    [SerializeField] private ComputeShader _initialStateCompute;
    private static readonly int _gridID = Shader.PropertyToID("_Grid");
    private static readonly int _resolutionID = Shader.PropertyToID("_Resolution");
    private static readonly int _highlightIndexID = Shader.PropertyToID("_HighlightIndex");
    private static readonly int _selectedIndexID = Shader.PropertyToID("_SelectedIndex");
    private Vector2 _screenResolution;
    private ComputeBuffer _gridBuffer;
    private int2 _resolution;
    private int _groupX;
    private int _groupY;
    private bool _canGetInput = true;

    private void Awake()
    {
        _screenResolution = new Vector2(Screen.width, Screen.height);
    }

    private void Start()
    {
        _gridBuffer = _spawner.GridBuffer;
        _resolution = _spawner.Resolution;
        _groupX = Mathf.CeilToInt(_resolution.x / 8 + 1);
        _groupY = Mathf.CeilToInt(_resolution.y / 8 + 1);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _canGetInput = false;
        }

        if (_canGetInput)
        {
            GetInput();
        }
        else
        {
            TransformGridToInitialState();
            _logic.CanExecute = true;
            Destroy(this.gameObject);
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

        _inputCompute.SetBuffer(0, _gridID, _gridBuffer);
        _inputCompute.SetVector(_resolutionID, new Vector4(_resolution.x, _resolution.y, 0, 0));
        _inputCompute.SetVector(_highlightIndexID, new Vector4(highlightIndex.x, highlightIndex.y, 0, 0));
        _inputCompute.SetVector(_selectedIndexID, new Vector4(selectedIndex.x, selectedIndex.y, 0, 0));

        _inputCompute.Dispatch(0, _groupX, _groupY, 1);
        _presenter.SetBuffer(_gridBuffer);
    }

    private void TransformGridToInitialState()
    {
        _initialStateCompute.SetBuffer(0, _gridID, _gridBuffer);
        _initialStateCompute.SetVector(_resolutionID, new Vector4(_resolution.x, _resolution.y, 0, 0));

        _initialStateCompute.Dispatch(0, _groupX, _groupY, 1);
        _presenter.SetBuffer(_gridBuffer);
    }

    private Vector2 GetGridPosition(Vector3 position)
    {
        float ratioX = _resolution.x / _screenResolution.x;
        float ratioY = _resolution.y / _screenResolution.y;

        return new Vector2(position.x * ratioX, position.y * ratioY);
    }
}

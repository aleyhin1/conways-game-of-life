using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class GridLogic : MonoBehaviour
{
    public static Action<AutomataTypes> OnAutomataStarted;
    [SerializeField] private AutomataTypes _type;
    [SerializeField, Range(.001f, 360)] private float _tickTime;
    [SerializeField] private List<ComputeShader> _computes;
    [SerializeField] private ComputeShader _copyCompute;
    [SerializeField] private GridDataSO _data;
    private bool _isRunning = false;
    private float _timer;
    private ComputeShader _activeAutomata;
    private ComputeBuffer _previousGrid;
    private int2 _dispatchGroups;
    private static readonly int _gridID = Shader.PropertyToID("_Grid");
    private static readonly int _previousGridID = Shader.PropertyToID("_PreviousGrid");
    private static readonly int _resolutionID = Shader.PropertyToID("_Resolution");
    private static readonly int _inputID = Shader.PropertyToID("_Input");
    private static readonly int _outputID = Shader.PropertyToID("_Output");

    private void Awake()
    {
        RandomSeed.OnSeedReady += StartLogic;
        SeedInput.OnSeedReady += StartLogic;
        UI.OnBackToSeed += StopLogic;
    }

    private void Update()
    {
        if (!_isRunning) return;

        if (_timer > _tickTime)
        {
            _copyCompute.Dispatch(0, _dispatchGroups.x, _dispatchGroups.y, 1);

            _activeAutomata.Dispatch(0, _dispatchGroups.x, _dispatchGroups.y, 1);

            _timer = 0;
        }

        _timer += Time.deltaTime;
    }

    private void StartLogic()
    {
        _isRunning = true;
        _activeAutomata = _computes[(int)_type];

        SetComputeData();

        OnAutomataStarted?.Invoke(_type);
    }

    private void StopLogic()
    {
        DisposeComputeData();
        _isRunning = false;
    }

    private void SetComputeData()
    {
        int2 resolution = _data.Data.Resolution;
        int groupX = Mathf.CeilToInt(resolution.x / 8 + 1);
        int groupY = Mathf.CeilToInt(resolution.y / 8 + 1);
        _dispatchGroups = new int2(groupX, groupY);

        _previousGrid = new ComputeBuffer(resolution.x * resolution.y, sizeof(uint));

        _activeAutomata.SetBuffer(0, _gridID, _data.Data.GridBuffer);
        _activeAutomata.SetBuffer(0, _previousGridID, _previousGrid);
        _activeAutomata.SetVector(_resolutionID, new Vector4(resolution.x, resolution.y, 0, 0));

        _copyCompute.SetVector(_resolutionID, new Vector4(resolution.x, resolution.y, 0, 0));
        _copyCompute.SetBuffer(0, _inputID, _data.Data.GridBuffer);
        _copyCompute.SetBuffer(0, _outputID, _previousGrid);
    }

    private void DisposeComputeData()
    {
        _previousGrid?.Dispose();
        _previousGrid = null;
    }

    private void OnDestroy()
    {
        SeedInput.OnSeedReady -= StartLogic;
        UI.OnBackToSeed -= StopLogic;
        RandomSeed.OnSeedReady -= StartLogic;

        DisposeComputeData();
    }
}

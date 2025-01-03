using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;

public class GridSpawner : MonoBehaviour
{
    [SerializeField] private int2 _resolution;
    [SerializeField] private GridDataSO _data;
    [SerializeField] private ComputeShader _checkerCompute;
    private ComputeBuffer _gridBuffer;
    private static readonly int _gridID = Shader.PropertyToID("_Grid");
    private static readonly int _resolutionID = Shader.PropertyToID("_Resolution");


    private void OnEnable()
    {
        _gridBuffer = new ComputeBuffer(_resolution.x * _resolution.y, sizeof(uint));
        _checkerCompute.SetVector(_resolutionID, new Vector4(_resolution.x, _resolution.y, 0, 0));
        _checkerCompute.SetBuffer(0, _gridID, _gridBuffer);
        _data.Data.GridBuffer = _gridBuffer;
        _data.Data.Resolution = _resolution;
    }

    private void Start()
    {
        int groupX = Mathf.CeilToInt(_resolution.x / 8 + 1);
        int groupY = Mathf.CeilToInt(_resolution.y / 8 + 1);

        _checkerCompute.Dispatch(0, groupX, groupY, 1);
    }

    private void OnDisable()
    {
        _gridBuffer.Release();
        _gridBuffer = null;
    }
}

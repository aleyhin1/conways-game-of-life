using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridClearer : MonoBehaviour
{
    [SerializeField] private GridDataSO _data;
    [SerializeField] private ComputeShader _clearCompute;
    private int _groupX;
    private int _groupY;
    private static readonly int _gridID = Shader.PropertyToID("_Grid");
    private static readonly int _resolutionID = Shader.PropertyToID("_Resolution");

    private void Awake()
    {
        UI.OnClear += Clear;
    }

    private void Start()
    {
        _clearCompute.SetVector(_resolutionID, new Vector4(_data.Data.Resolution.x, _data.Data.Resolution.y, 0, 0));
        _clearCompute.SetBuffer(0, _gridID, _data.Data.GridBuffer);

        _groupX = Mathf.CeilToInt(_data.Data.Resolution.x / 8 + 1);
        _groupY = Mathf.CeilToInt(_data.Data.Resolution.y / 8 + 1);
    }

    private void Clear()
    {
        _clearCompute.Dispatch(0, _groupX, _groupY, 1);
    }

    private void OnDestroy()
    {
        UI.OnClear -= Clear;
    }
}

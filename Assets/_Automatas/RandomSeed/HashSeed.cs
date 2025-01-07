using System;
using Unity.Mathematics;
using UnityEngine;

public class HashSeed : MonoBehaviour
{
    public static Action OnSeedReady;
    public static Action OnRandomSeed;
    [SerializeField] private bool _useHash;
    [SerializeField] private int _seed;
    [SerializeField] private bool _useRandomSeed;
    [SerializeField] private ComputeShader _hashCompute;
    [SerializeField] private GridDataSO _data;
    private static readonly int _hashID = Shader.PropertyToID("_Hash");
    private static readonly int _resolutionID = Shader.PropertyToID("_Resolution");
    private static readonly int _seedID = Shader.PropertyToID("_Seed");


    private void Start()
    {
        if (!_useHash) return;

        OnRandomSeed?.Invoke();
        StartHash();
    }

    private void StartHash()
    {
        if (_useRandomSeed) _seed = UnityEngine.Random.Range(0, 2147483647);

        int2 resolution = _data.Data.Resolution;
        _hashCompute.SetBuffer(0, _hashID, _data.Data.GridBuffer);
        _hashCompute.SetVector(_resolutionID, new Vector4(resolution.x, resolution.y, 0, 0));
        _hashCompute.SetInt(_seedID, _seed);

        int groupX = Mathf.CeilToInt(resolution.x / 8 + 1);
        int groupY = Mathf.CeilToInt(resolution.y / 8 + 1);

        _hashCompute.Dispatch(0, groupX, groupY, 1);

        OnSeedReady?.Invoke();
    }
}

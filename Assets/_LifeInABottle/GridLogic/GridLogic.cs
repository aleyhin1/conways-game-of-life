using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class GridLogic : MonoBehaviour
{
    public bool CanExecute = false;
    public ComputeBuffer NextGridBuffer { get; private set; }
    [SerializeField, Min(0)] private float _iterationTime;
    [SerializeField] private GridSpawner _spawner;
    [SerializeField] private GridPresenter _presenter;
    [SerializeField] private ComputeShader _computeShader;
    private static readonly int _previousGridId = Shader.PropertyToID("_PreviousGrid");
    private static readonly int _nextGridId = Shader.PropertyToID("_NextGrid");
    private static readonly int _resolutionId = Shader.PropertyToID("_Resolution");
    private ComputeBuffer _previousGridBuffer;
    private float _time = 0;
    private int _groupX;
    private int _groupY;

    private void Start()
    {
        _previousGridBuffer = _spawner.GridBuffer;
        NextGridBuffer = new ComputeBuffer(_spawner.Resolution.x * _spawner.Resolution.y, sizeof(uint));

        _computeShader.SetBuffer(0, _previousGridId, _previousGridBuffer);
        _computeShader.SetBuffer(0, _nextGridId, NextGridBuffer);
        _computeShader.SetVector(_resolutionId, new Vector4(_spawner.Resolution.x, _spawner.Resolution.y, 0, 0));

        _groupX = Mathf.CeilToInt(_spawner.Resolution.x / 8);
        _groupY = Mathf.CeilToInt(_spawner.Resolution.y / 8);
    }

    private void Update()
    {
        if (CanExecute)
        {
            Iteration();
        }
    }

    private void Iteration()
    {
        if (_time > _iterationTime)
        {
            _computeShader.Dispatch(0, _groupX, _groupY, 1);
            _presenter.SetBuffer(NextGridBuffer);
            UpdateBuffers();
            _time = 0;
        }

        _time += Time.deltaTime;
    }

    private void UpdateBuffers()
    {
        ComputeBuffer temp = _previousGridBuffer;
        _previousGridBuffer = NextGridBuffer;
        NextGridBuffer = temp;
        _computeShader.SetBuffer(0, _previousGridId, _previousGridBuffer);
        _computeShader.SetBuffer(0, _nextGridId, NextGridBuffer);
        _computeShader.SetVector(_resolutionId, new Vector4(_spawner.Resolution.x, _spawner.Resolution.y, 0, 0));
    }

    private async void ReadBackBuffer()
    {
        uint[] array = new uint[_spawner.Resolution.x * _spawner.Resolution.y];
        NextGridBuffer.GetData(array);

        for (int y = 0; y < _spawner.Resolution.y; y++)
        {
            for (int x = 0; x < _spawner.Resolution.x; x++)
            {
                Debug.Log(array[x + _spawner.Resolution.x * y]);

            }
            await Task.Delay(5);
        }
    }

    private void OnDestroy()
    {
        NextGridBuffer.Dispose();
        _previousGridBuffer.Dispose();
        NextGridBuffer = null;
        _previousGridBuffer = null;
    }
}

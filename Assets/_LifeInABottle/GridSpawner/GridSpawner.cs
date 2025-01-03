using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;

public class GridSpawner : MonoBehaviour
{
    public static Action<ComputeBuffer> OnCheckerCreated;
    public ComputeBuffer GridBuffer { get; private set; }
    [field: SerializeField] public int2 Resolution { get; private set; }
    [SerializeField] private ComputeShader _checkerShader;
    private static readonly int _gridID = Shader.PropertyToID("_Grid");
    private static readonly int _resolutionID = Shader.PropertyToID("_Resolution");

    private void OnEnable()
    {
        GridBuffer = new ComputeBuffer(Resolution.x * Resolution.y, sizeof(uint));
        _checkerShader.SetVector(_resolutionID, new Vector4(Resolution.x, Resolution.y, 0, 0));
        _checkerShader.SetBuffer(0, _gridID, GridBuffer);
    }

    private void Start()
    {
        int groupX = Mathf.CeilToInt(Resolution.x / 8 + 1);
        int groupY = Mathf.CeilToInt(Resolution.y / 8 + 1);

        _checkerShader.Dispatch(0, groupX, groupY, 1);

        //ReadBackBuffer();
    }

    private async void ReadBackBuffer()
    {
        uint[] array = new uint[Resolution.x * Resolution.y];
        GridBuffer.GetData(array);

        for (int y = 0; y < Resolution.y; y++)
        {
            for (int x = 0; x < Resolution.x; x++)
            {
                Debug.Log(array[x + Resolution.x * y]);

            }
            await Task.Delay(5);
        }
    }

    private void OnDisable()
    {
        GridBuffer.Release();
        GridBuffer = null;
    }
}

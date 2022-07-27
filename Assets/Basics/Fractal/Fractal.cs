using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using static Unity.Mathematics.math;
using quaternion = Unity.Mathematics.quaternion;
using Random = UnityEngine.Random;

public class Fractal : MonoBehaviour
{
    [BurstCompile(FloatPrecision.Standard, FloatMode.Fast,CompileSynchronously = true)]
    private struct UpdateFractalLevelJob : IJobFor
    {
        public float spinAngleDelta;
        public float scale;

        [ReadOnly]
        public NativeArray<FractalPart> parents;
        
        public NativeArray<FractalPart> parts;

        [WriteOnly]
        public NativeArray<float3x4> matrices;

        public void Execute(int i)
        {
            FractalPart parent = parents[i / 5];
            FractalPart part = parts[i];
            part.SpinAngle += spinAngleDelta;
            part.WorldRotation = mul(parent.WorldRotation,
                mul(part.Rotation, quaternion.RotateY(part.SpinAngle)));
            part.WorldPosition =
                parent.WorldPosition +
                mul(parent.WorldRotation, 1.5f * scale * part.Direction);
            parts[i] = part;

            float3x3 r = float3x3(part.WorldRotation) * scale;
            matrices[i] = float3x4(r.c0, r.c1, r.c2, part.WorldPosition);
        }
    }
    
    private struct FractalPart {
        public float3 Direction, WorldPosition;
        public quaternion Rotation, WorldRotation;
        public float SpinAngle;
    }
    
    [SerializeField, Range(2 ,8)]
    private int Depth = 4;

    [SerializeField]
    private Mesh Mesh;

    [SerializeField]
    private Material Material;
    
    [SerializeField]
    Gradient Gradient;

    private static MaterialPropertyBlock _propertyBlock;

    private static readonly int 
        _baseColorId = Shader.PropertyToID("_BaseColor"),
        _matricesId = Shader.PropertyToID("_Matrices"),
        _sequenceNumbersId = Shader.PropertyToID("_SequenceNumbers");

    private static float3[] _directions = {
        up(), right(), left(), forward(), back()
    };

    private static quaternion[] _rotations = {
        quaternion.identity,
        quaternion.RotateZ(-0.5f * PI), quaternion.RotateZ(0.5f * PI),
        quaternion.RotateX(0.5f * PI), quaternion.RotateX(-0.5f * PI)
    };
    
    private NativeArray<FractalPart>[] _parts;

    private NativeArray<float3x4>[] _matrices;
    
    private ComputeBuffer[] _matricesBuffers;
    
    Vector4[] _sequenceNumbers;
    
    private void OnEnable()
    {
        _parts = new NativeArray<FractalPart>[Depth];
        _matrices = new NativeArray<float3x4>[Depth];
        _matricesBuffers = new ComputeBuffer[Depth];
        _sequenceNumbers = new Vector4[Depth];
        int stride = 12 * 4;
        
        for (int i = 0, lenght = 1; i < _parts.Length; i++, lenght *= 5)
        {
            _parts[i] = new NativeArray<FractalPart>(lenght, Allocator.Persistent);
            _matrices[i] = new NativeArray<float3x4>(lenght, Allocator.Persistent);
            _matricesBuffers[i] = new ComputeBuffer(lenght, stride);
            _sequenceNumbers[i] = new Vector4(Random.value, Random.value);
        }

        _parts[0][0] = CreatePart(0);
        for (int li = 1; li < _parts.Length; li++)
        {
            NativeArray<FractalPart> levelParts = _parts[li];
            for (int fpi = 0; fpi < levelParts.Length; fpi += 5)
            {
                for (int ci = 0; ci < 5; ci++)
                {
                    levelParts[fpi + ci] = CreatePart(ci);
                }
            }
        }

        _propertyBlock ??= new MaterialPropertyBlock();
    }

    private void OnDisable()
    {
        for (int i = 0; i < _matricesBuffers.Length; i++) {
            _matricesBuffers[i].Release();
            _parts[i].Dispose();
            _matrices[i].Dispose();
        }
        _parts = null;
        _matrices = null;
        _matricesBuffers = null;
        _sequenceNumbers = null;
    }
    
    void OnValidate () 
    {
        if (_parts != null && enabled)
        {
            OnDisable();
            OnEnable();
        }
    }

    private void Update()
    {
        float spinAngleDelta = 0.125f * PI * Time.deltaTime;
        FractalPart rootPart = _parts[0][0];
        rootPart.SpinAngle += spinAngleDelta;
        rootPart.WorldRotation =
            mul(transform.rotation,
            mul(rootPart.Rotation, quaternion.RotateY(rootPart.SpinAngle)));
        rootPart.WorldPosition = transform.position;
        _parts[0][0] = rootPart;
        float objectScale = transform.localScale.x;
        

        float3x3 r = float3x3(rootPart.WorldRotation) * objectScale;
        _matrices[0][0] = float3x4(r.c0, r.c1, r.c2, rootPart.WorldPosition);
        
        float scale = objectScale;
        JobHandle jobHandle = default;
        for (int li = 1; li < _parts.Length; li++)
        {
            scale *= 0.5f;
            jobHandle = new UpdateFractalLevelJob
            {
                spinAngleDelta = spinAngleDelta,
                scale = scale,
                parents = _parts[li - 1],
                parts = _parts[li],
                matrices = _matrices[li]
            }.ScheduleParallel(_parts[li].Length, 5 ,jobHandle);
        }
        jobHandle.Complete();

        var bounds = new Bounds(rootPart.WorldPosition, 3f * float3(objectScale));
        for (int i = 0; i < _matricesBuffers.Length; i++)
        {
            ComputeBuffer buffer = _matricesBuffers[i];
            buffer.SetData(_matrices[i]);
            _propertyBlock.SetColor(_baseColorId, Gradient.Evaluate(i / _matricesBuffers.Length - 1));
            _propertyBlock.SetBuffer(_matricesId, buffer);
            _propertyBlock.SetVector(_sequenceNumbersId, _sequenceNumbers[i]);
            Graphics.DrawMeshInstancedProcedural(
                Mesh, 0, Material, bounds, buffer.count, _propertyBlock);
        }
    }

    private FractalPart CreatePart(int childIndex) => new FractalPart
    {
        Direction = _directions[childIndex],
        Rotation = _rotations[childIndex]
    };
}

using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Profiling;

public partial class CameraRenderer
{
    private partial void DrawGizmos();
    private partial void DrawUnsupportedShaders();

    private partial void PrepareBuffer();
    private partial void PrepareForSceneWindow();
    
#if UNITY_EDITOR
    
    private string SampleName { get; set; }
    
    private static ShaderTagId[] legacyShaderTagIds =
    {
        new ShaderTagId("Always"),
        new ShaderTagId("ForwardBase"),
        new ShaderTagId("PrepassBase"),
        new ShaderTagId("Vertex"),
        new ShaderTagId("VertexLMRGBM"),
        new ShaderTagId("VertexLM")
    };

    private static Material _errorMaterial;

    private partial void DrawGizmos()
    {
        if (Handles.ShouldRenderGizmos())
        {
            _context.DrawGizmos(_camera, GizmoSubset.PreImageEffects);
            _context.DrawGizmos(_camera, GizmoSubset.PostImageEffects);
        }
    }

    private partial void DrawUnsupportedShaders()
    {
        if (_errorMaterial == null)
            _errorMaterial = new Material(Shader.Find("Hidden/InternalErrorShader"));
        var drawSettings = new DrawingSettings(legacyShaderTagIds[0], new SortingSettings(_camera))
        {
            overrideMaterial = _errorMaterial
        };
        for (int i = 1; i < legacyShaderTagIds.Length; i++)
        {
            drawSettings.SetShaderPassName(i, legacyShaderTagIds[i]);
        }

        var filterSettings = FilteringSettings.defaultValue;
        _context.DrawRenderers(_cullingResults, ref drawSettings, ref filterSettings);
    }
    
    private partial void PrepareBuffer()
    {
        Profiler.BeginSample("Editor Only");
        _buffer.name = SampleName = _camera.name;
        Profiler.EndSample();
    }
    
    private partial void PrepareForSceneWindow()
    {
        if (_camera.cameraType == CameraType.SceneView)
        {
            ScriptableRenderContext.EmitWorldGeometryForSceneView(_camera);
        }
    }
#else
    private const string SampleName = bufferName;
    
#endif
}
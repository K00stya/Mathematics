using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

public class GPUGraph : MonoBehaviour
{
	[SerializeField] 
	private ComputeShader computeShader;

	[SerializeField]
	private Material Material;

	[SerializeField]
	private Mesh Mesh;

	private static readonly int 
		positionsId = Shader.PropertyToID("_Positions"),
		resolutionId = Shader.PropertyToID("_Resolution"),
		stepId = Shader.PropertyToID("_Step"),
		timeId = Shader.PropertyToID("_Time"),
		transitionProgressId = Shader.PropertyToID("_TransitionProgress");
	
	const int maxResolution = 1000;
	
	[SerializeField, Range(10, maxResolution)] 
	int resolution = 10;

	[SerializeField] 
	FunctionLibrary.FunctionName Function;

	public enum TransitionMode
	{
		Cycle,
		Random
	}

	[SerializeField] 
	private TransitionMode transitionMode;
	[SerializeField, Min(0f)] 
	private float TunctionDuration = 1f, TransitionDuration = 1f;
	private  float _duration;
	private bool _transitioning;
	private FunctionLibrary.FunctionName _transitionFunction;
	private ComputeBuffer _positionsBuffer;

	private void OnEnable()
	{
		_positionsBuffer = new ComputeBuffer(maxResolution * maxResolution, 3 * 4);
	}

	private void OnDisable()
	{
		_positionsBuffer.Release();
		_positionsBuffer = null;
	}

	void Update()
	{
		_duration += Time.deltaTime;
		if (_transitioning)
		{
			if (_duration >= TransitionDuration)
			{
				_duration -= TransitionDuration;
				_transitioning = false;
			}
		}
		else if (_duration >= TunctionDuration)
		{
			_duration -= TunctionDuration;
			_transitioning = true;
			_transitionFunction = Function;
			PickNextFunction();
		}

		UpdateFunctionOnGPU();
	}

	void UpdateFunctionOnGPU()
	{
		float step = 2f / resolution;
		computeShader.SetInt(resolutionId, resolution);
		computeShader.SetFloat(stepId, step);
		computeShader.SetFloat(timeId, Time.time);
		
		if (_transitioning) {
			computeShader.SetFloat(
				transitionProgressId,
				Mathf.SmoothStep(0f, 1f, _duration / TransitionDuration)
			);
		}

		var kernelIndex = (int) Function + (int)(_transitioning ? _transitionFunction : Function) * 
				FunctionLibrary.FunctionCount;;
		computeShader.SetBuffer(kernelIndex, positionsId, _positionsBuffer);
		
		int groups = Mathf.CeilToInt(resolution / 8f);
		computeShader.Dispatch(kernelIndex, groups, groups, 1);

		Material.SetBuffer(positionsId, _positionsBuffer);
		Material.SetFloat(stepId, step);
		var bounds = new Bounds(Vector3.zero, Vector3.one * (2f + 2f / resolution));
		Graphics.DrawMeshInstancedProcedural(
			Mesh, 0 , Material, bounds, resolution * resolution);
	}

	void PickNextFunction()
	{
		Function = transitionMode == TransitionMode.Cycle
			? FunctionLibrary.GetNextFunctionName(Function)
			: FunctionLibrary.GetRandomFunctionNameOtherThan(Function);
	}
}

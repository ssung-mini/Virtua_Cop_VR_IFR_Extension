using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using ViveSR.anipal.Eye;

[ExecuteInEditMode]
public class DynamicBlurring_VisualSearchTask : MonoBehaviour
{

	public bool visualize = false;
	public bool visualizeColor = true;
	public bool dynamic = false;
	public bool eye_fixed = false;

	public bool softEdge = true;

	private Material material;
	private Material visualizer;
	[Space(2)]

	[Header("FoV conditions")]
	[Range(0f, 1f)]
	public float FR_center_X = 0.5f;
	[Range(0f, 1f)]
	public float FR_center_Y = 0.5f;

	[Range(0f, 1f)]
	public float ratio110 = 0.45f;

	/// <summary>
	/// Visual search task 시작 전, 여기서 값을 조절하고 실행할 것.
	/// </summary>
	[Range(0f, 110f)]
	public float outRadDeg = 62.5f;
	[Range(1, 512)]
	public int downsamplingRate = 4;

	[Range(0f, 110f)]
	public float inOutDiffDeg = 10f;

	
	[Range(0f, 1f)]
	private float inRad = 0.1694f;
	[Range(0f, 1f)]
	private float outRad = 0.2103f;

	[Space(2)]

	[Header("Sensitivity")]
	[Range(0, 10)]
	public int loop_times = 1;
	public float sigma = 4.3f;
	[ReadOnly]
	public int staticKernelSize = 11;

	public int blurIntensity = 10;
	public float visualJNDPoint = 0.7326771f;
	public float blurUnit = 0.3f;

	[ReadOnly]
	public float targetKernelValue = 0;

	[ReadOnly]
	public float tempKernelValue = 0;
	private float InOutDiff;

	//private Camera cam;

	[SerializeField]
	private float velocityMagnitude;

	private EyeDataManager m_EDM;

	void Start()
	{
		//설정한 OutRad로 초기화
		outRad = DegreeToUnityDegree(outRadDeg);

		//쉐이더 찾기
		material = new Material(Shader.Find("Hidden/FoveatedDownsampling"));
		visualizer = new Material(Shader.Find("Hidden/DynamicBlurringVisualizer"));

		//변수 설정
		InOutDiff = DegreeToUnityDegree(inOutDiffDeg);
		inRad = outRad - InOutDiff;

		//다른 스크립트 찾기
		//cam = GetComponent<Camera>();
		m_EDM = GetComponent<EyeDataManager>();
	}
    void FixedUpdate()
	{
		FR_center_X = m_EDM.gazePosition.x;
		FR_center_Y = m_EDM.gazePosition.y;
		//downsamplingRate = staticKernelSize;
	}

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		outRad = DegreeToUnityDegree(outRadDeg);
		InOutDiff = DegreeToUnityDegree(inOutDiffDeg);
		inRad = outRad - InOutDiff;

		//fixed update에서 변경된 변수들을 shader로 넘겨줌
		//해당 변수들을 실제로 적용하는건 shader

		int softEdgeInteger = softEdge ? 1 : 0;
		material.SetInt("_softEdge", softEdgeInteger);
		material.SetInt("_KernelSize", downsamplingRate);
		material.SetFloat("_StandardDeviation", sigma);
		material.SetFloat("_inRad", inRad);
		material.SetFloat("_outRad", outRad);
		material.SetInt("_loop", loop_times);
		material.SetFloat("_eyeXPos", FR_center_X);
		material.SetFloat("_eyeYPos", FR_center_Y);
		material.SetInt("_ResWidth", source.width);
		material.SetInt("_ResHeight", source.height);

		var temporaryTexture = RenderTexture.GetTemporary(source.width, source.height);
		Graphics.Blit(source, temporaryTexture, material, 0);

		if (visualize)
		{
			var temporaryTexture2 = RenderTexture.GetTemporary(source.width, source.height);
			//Graphics.Blit(temporaryTexture, temporaryTexture2, material, 1);
			Graphics.Blit(temporaryTexture, destination, material);

			int visualizeColorInteger = visualizeColor ? 1 : 0;
			visualizer.SetInt("_softEdge", softEdgeInteger);
			visualizer.SetInt("_visualizeColor", visualizeColorInteger);
			visualizer.SetInt("_KernelSize", downsamplingRate);
			visualizer.SetFloat("_inRad", inRad);
			visualizer.SetFloat("_outRad", outRad);
			visualizer.SetFloat("_eyeXPos", FR_center_X);
			visualizer.SetFloat("_eyeYPos", FR_center_Y);
			Graphics.Blit(temporaryTexture2, destination, visualizer, 0);

			RenderTexture.ReleaseTemporary(temporaryTexture2);
		}
		else
		{
			Graphics.Blit(temporaryTexture, destination, material);
		}
		
		RenderTexture.ReleaseTemporary(temporaryTexture);
	}

	private float DegreeToUnityDegree(float degree)
	{
		//화면 중심으로 부터의 시야 각도를 유니티에서 사용할
		//화면 중심으로 부터의 원의 반경으로 변경함
		//0 부터 1 사이의 값
		//해당 값은 oculus rift cv1에서 임의로 측정되었음
		float UnityDegree = degree * ratio110 / 110f;
		return UnityDegree;
	}

}

using UnityEngine;

public class BackgroundParallax : MonoBehaviour
{
	private Transform _cameraTransform;

	private Vector3 _startCameraPosition;

	private Transform _transform;

	private Vector3 _startPosition;

	[SerializeField]
	private float _parallaxKoef = 1f;

	[SerializeField]
	private float _parallaxUVKoef = 1f;

	[SerializeField]
	private float _lightmapParallaxUVKoef = 1f;

	private Material scrollMaterial;

	private void Start()
	{
		_transform = base.transform;
		_startPosition = _transform.position;
		_cameraTransform = Camera.main.transform;
		_startCameraPosition = _cameraTransform.position;
		scrollMaterial = GetComponent<Renderer>().material;
	}

	private void LateUpdate()
	{
		float num = (_startCameraPosition.x - _cameraTransform.position.x) * _parallaxKoef;
		if (scrollMaterial != null)
		{
			float num2 = _startCameraPosition.x - _cameraTransform.position.x;
			scrollMaterial.SetVector("_Offset", new Vector4(num2 * _parallaxUVKoef, 0f));
			scrollMaterial.SetVector("_LightmapOffset", new Vector4(num2 * _lightmapParallaxUVKoef, 0f));
		}
		num += _startPosition.x;
		Vector3 startPosition = _startPosition;
		startPosition.x = num;
		_transform.position = startPosition;
	}
}

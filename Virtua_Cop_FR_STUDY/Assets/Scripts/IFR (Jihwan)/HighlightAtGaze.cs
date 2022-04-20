using Tobii.G2OM;
using UnityEngine;

namespace Tobii.XR.Examples
{
    public class HighlightAtGaze : MonoBehaviour, IGazeFocusable
    {
        public DynamicBlurring cam;

        public Color HighlightColor = Color.red;
        public float AnimationTime = 0.1f;

        private Renderer _renderer;
        private Color _originalColor;
        private Color _targetColor;

        public Canvas fixationLossCanvas;

        private float time = 0.0f;

        //The method of the "IGazeFocusable" interface, which will be called when this object receives or loses focus
        public void GazeFocusChanged(bool hasFocus)
        {
            //If this object received focus, fade the object's color to highlight color
            if (hasFocus)
            {
                cam.eye_fixed = true;
                _targetColor = HighlightColor;
            }
            //If this object lost focus, fade the object's color to it's original color
            else
            {
                cam.eye_fixed = false;
                _targetColor = _originalColor;
            }
        }

        private void Start()
        {
            _renderer = GetComponent<Renderer>();
            _originalColor = _renderer.material.color;
            _targetColor = _originalColor;
        }

        private void Update()
        {
            Debug.Log(time);
            if (cam.eye_fixed) time = 0;
            else if (!cam.eye_fixed) time += Time.deltaTime;

            if (time > 0.2f) fixationLossCanvas.gameObject.SetActive(true);
            if (cam.eye_fixed) fixationLossCanvas.gameObject.SetActive(false);
            if (_renderer.material.HasProperty(Shader.PropertyToID("_BaseColor"))) // new rendering pipeline (lightweight, hd, universal...)
                _renderer.material.SetColor("_BaseColor", Color.Lerp(_renderer.material.GetColor("_BaseColor"), _targetColor, Time.deltaTime * (1 / AnimationTime)));
            else
                _renderer.material.color = Color.Lerp(_renderer.material.color, _targetColor, Time.deltaTime * (1 / AnimationTime));
        }
    }
}

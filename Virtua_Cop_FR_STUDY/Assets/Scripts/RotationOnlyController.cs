using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SpatialTracking;

namespace UnityEngine.XR.Interaction.Toolkit
{
    public class RotationOnlyController : XRController
    {
        private InputDevice _headInputDevice;
        private TrackedPoseDriver.TrackingType _trackingType;

        private InputDevice HeadInputDevice
        {
            get
            {
                return _headInputDevice.isValid
                    ? _headInputDevice
                    : (_headInputDevice = InputDevices.GetDeviceAtXRNode(XRNode.CenterEye));
            }
        }

        private void Start()
        {
            var trackedPoseDriver = FindObjectOfType<TrackedPoseDriver>();
            _trackingType = trackedPoseDriver.trackingType;
        }

        protected override void OnBeforeRender()
        {
            //No way to check if enabled as m_UpdateTrackingType is private
            UpdateTrackingInputOverride();
        }

        private void FixedUpdate()
        {
            UpdateTrackingInputOverride();
        }

        private void UpdateTrackingInputOverride()
        {
            if (_trackingType != TrackedPoseDriver.TrackingType.RotationOnly) return;

            //Try to get device position
            if (!inputDevice.TryGetFeatureValue(CommonUsages.devicePosition, out var devicePosition)) return;

            //Try to get center eye position
            if (HeadInputDevice.TryGetFeatureValue(CommonUsages.centerEyePosition, out var centerEyePosition))
            {
                transform.localPosition = devicePosition - centerEyePosition;
            }
        }
    }
}



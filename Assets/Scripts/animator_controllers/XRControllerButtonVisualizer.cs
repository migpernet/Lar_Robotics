using UnityEngine;
using UnityEngine.XR;

public class XRControllerButtonVisualizer : MonoBehaviour
{
    public XRNode controllerNode = XRNode.LeftHand;

    [Header("Buttons")]
    public Transform trigger;
    public Transform grip;
    public Transform primaryButton;
    public Transform secondaryButton;
    public Transform thumbstick;

    [Header("Press Settings")]
    public float pressDepth = 0.004f;
    public float thumbstickMove = 0.003f;
    public float thumbstickPressThreshold = 0.7f;

    private InputDevice device;

    private Vector3 triggerStart;
    private Vector3 gripStart;
    private Vector3 primaryStart;
    private Vector3 secondaryStart;
    private Vector3 thumbstickStart;

    void Start()
    {
        device = InputDevices.GetDeviceAtXRNode(controllerNode);

        if (trigger) triggerStart = trigger.localPosition;
        if (grip) gripStart = grip.localPosition;
        if (primaryButton) primaryStart = primaryButton.localPosition;
        if (secondaryButton) secondaryStart = secondaryButton.localPosition;
        if (thumbstick) thumbstickStart = thumbstick.localPosition;
    }

    void Update()
    {
        if (!device.isValid)
            device = InputDevices.GetDeviceAtXRNode(controllerNode);

        AnimateAnalog(CommonUsages.trigger, trigger, triggerStart);
        AnimateAnalog(CommonUsages.grip, grip, gripStart);

        AnimateBinary(CommonUsages.primaryButton, primaryButton, primaryStart);
        AnimateBinary(CommonUsages.secondaryButton, secondaryButton, secondaryStart);

        AnimateThumbstick();
    }

    void AnimateAnalog(InputFeatureUsage<float> usage, Transform button, Vector3 start)
    {
        if (button == null) return;

        if (device.TryGetFeatureValue(usage, out float value))
        {
            button.localPosition =
                start + Vector3.forward * (value * pressDepth);
        }
    }

    void AnimateBinary(InputFeatureUsage<bool> usage, Transform button, Vector3 start)
    {
        if (button == null) return;

        if (device.TryGetFeatureValue(usage, out bool pressed))
        {
            button.localPosition =
                start + Vector3.forward * (pressed ? pressDepth : 0f);
        }
    }

    void AnimateThumbstick()
    {
        if (thumbstick == null) return;

        if (device.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 axis))
        {
            Vector3 offset =
                new Vector3(axis.x, axis.y, 0f) * thumbstickMove;

            // leve sensação de pressionado quando deslocamento é grande
            if (axis.magnitude > thumbstickPressThreshold)
                offset += Vector3.back * pressDepth;

            thumbstick.localPosition = thumbstickStart + offset;
        }
    }
}

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class InputSystemQuickTest : MonoBehaviour
{
    void OnEnable()
    {
        Debug.Log("InputSystem enabled. Devices:");
        foreach (var d in InputSystem.devices)
            Debug.Log($" - {d.displayName} ({d.layout})");

        // Простая проверка клавиатуры
        InputSystem.onEvent += OnAnyInputEvent;
    }

    void OnDisable()
    {
        InputSystem.onEvent -= OnAnyInputEvent;
    }

    private void OnAnyInputEvent(InputEventPtr ev, InputDevice device)
    {

        // фильтровать по device, если много мусора
        // Debug.Log($"Event from {device.displayName}: {ev}");
    }

    void Update()
    {
        if (Keyboard.current != null)
        {
            if (Keyboard.current.spaceKey.wasPressedThisFrame)
                Debug.Log("Keyboard.space detected (Keyboard.current)");
            if (Keyboard.current.aKey.wasPressedThisFrame)
                Debug.Log("Keyboard.a detected");
        }

        if (Gamepad.current != null && Gamepad.current.buttonSouth.wasPressedThisFrame)
            Debug.Log("Gamepad button pressed");
    }
}
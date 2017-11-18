using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomInput : MonoBehaviour {

    public float speedSlow = .2f;
    public float speedFast = .1f;
    public float delayToSlow = .5f;
    public float delayToFast = .3f;

    List<KeyCode> registeredKeys = new List<KeyCode>();
    Dictionary<KeyCode, KeyState> keys = new Dictionary<KeyCode, KeyState>();

    public static CustomInput instance;

    private void Awake()
    {
        instance = this;
    }

    void Update () {
        foreach (KeyCode key in registeredKeys)
        {
            if (Input.GetKeyDown(key))
            {
                keys[key].PhysicalPress();
            }
        }
	}

    public bool GetKeyPress(KeyCode key)
    {
        Debug.Assert(registeredKeys.Contains(key),"Key not registered");

        if (Input.GetKey(key))
        {
            KeyState state = keys[key];
            if (!state.hasUsedPhysicalPress)
            {
                state.hasUsedPhysicalPress = true;
                return true;
            }
            float timeSinceLastVirtualPress = Time.time - state.lastVirtualPressTime;
            float timeSinceLastPhysicalPress = Time.time - state.lastPhysicalPressTime;

            if (timeSinceLastPhysicalPress > delayToSlow)
            {
                float timeBetweenVirtualPresses = (timeSinceLastPhysicalPress > delayToFast+delayToSlow)?speedFast:speedSlow;
           
                if (timeSinceLastVirtualPress > timeBetweenVirtualPresses)
                {
                    state.lastVirtualPressTime = Time.time;
                    return true;
                }

            }
        }
        return false;
    }

    public void RegisterKey(KeyCode key)
    {
        if (!registeredKeys.Contains(key))
        {
            registeredKeys.Add(key);
            KeyState state = new KeyState(key);
            keys.Add(key, state);
        }
    }

    public class KeyState
    {
        public readonly KeyCode key;
        public float lastPhysicalPressTime;
        public float lastVirtualPressTime;
        public bool hasUsedPhysicalPress;

        public KeyState(KeyCode key)
        {
            this.key = key;
        }

        public void PhysicalPress()
        {
            lastPhysicalPressTime = Time.time;
            lastVirtualPressTime = Time.time;
            hasUsedPhysicalPress = false;
        }
    }
}

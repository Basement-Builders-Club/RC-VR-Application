using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HandAnimatorController : MonoBehaviour
{

    [SerializeField] private InputActionProperty trigger;
    [SerializeField] private InputActionProperty grip;

    private Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }
    private void Update()
    {
        float triggerVal = trigger.action.ReadValue<float>();
        float gripVal = grip.action.ReadValue<float>();

        anim.SetFloat("Trigger", triggerVal);
        anim.SetFloat("Grip", gripVal);
    }
}

using UnityEngine;

namespace Toolbox.Graphics
{
    /// <summary>
    /// Simple event handlers that can be linked to UnityEvents in the inspector.
    /// </summary>
    public class AnimatorParamHandler : MonoBehaviour
    {
        public Animator Anim;


        public void SetBoolParam(string paramName, bool value)
        {
            Anim.SetBool(paramName, value);
        }

        public void SetBoolParamTrue(string paramName)
        {
            Anim.SetBool(paramName, true);
        }

        public void SetBoolParamFalse(string paramName)
        {
            Anim.SetBool(paramName, false);
        }

        public void SetBoolParam(int paramName, bool value)
        {
            Anim.SetBool(paramName, value);
        }

        public void SetBoolParamTrue(int paramName)
        {
            Anim.SetBool(paramName, true);
        }

        public void SetBoolParamFalse(int paramName)
        {
            Anim.SetBool(paramName, false);
        }

        public void SetTriggerParam(string paramName)
        {
            Anim.SetTrigger(paramName);
        }

        public void SetTriggerParam(int paramName)
        {
            Anim.SetTrigger(paramName);
        }

        public void ResetTriggerParam(string paramName)
        {
            Anim.ResetTrigger(paramName);
        }

        public void ResetTriggerParam(int paramName)
        {
            Anim.ResetTrigger(paramName);
        }
    }
}

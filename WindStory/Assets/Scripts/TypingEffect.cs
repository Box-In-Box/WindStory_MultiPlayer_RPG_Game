using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TypingEffect : MonoBehaviour
{
    [SerializeField] [Range(0.01f, 0.1f)] float CharPerSeconds;
    public string targetMsg;
    public Text msgText;
    public int index;
    public bool isPlayingTypingEffect;

    public void SetMsg(Text _msgText, string msg)
    {
        if(isPlayingTypingEffect)
        {
            msgText.text = targetMsg;
            CancelInvoke();
            EffectEnd();
        }
        else
        {
            isPlayingTypingEffect = true;
            msgText = _msgText;
            targetMsg = msg;
            EffectStart();
        }  
    }

    void EffectStart()
    {
        msgText.text = "";
        index = 0;

        Invoke("EffectIng", CharPerSeconds);
    }

    void EffectIng()
    {
       
        if(msgText.text == targetMsg)
        {
            EffectEnd();
            return;
        }

        msgText.text += targetMsg[index];
        index++;

        Invoke("EffectIng", CharPerSeconds);
    }

    void EffectEnd()
    {
        isPlayingTypingEffect = false;
    }
}

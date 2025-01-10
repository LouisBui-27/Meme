using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class loopButton : MonoBehaviour
{
   // public Text buttonText;
    [SerializeField] private Image buttonImage;
    [SerializeField]private Sprite pressButton;
    [SerializeField]private Sprite releaseButton;

    public void ToggleLoop()
    {
        GameManage.instance.ToggleLoop();
        updateButtonLoop();
    }
     private void updateButtonLoop()
    {
        bool loop = GameManage.instance.IsLooping();
        buttonImage.sprite = loop ? pressButton : releaseButton;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class spamButton : MonoBehaviour
{
    [SerializeField] private Image buttonImage;
    [SerializeField] private Sprite pressButton;
    [SerializeField] private Sprite releaseButton;

    public void Spam()
    {
        GameManage.instance.ToggleSpam();
        updateSpamText();
    }
    private void updateSpamText()
    {
        bool spam = GameManage.instance.IsSpamming();
        buttonImage.sprite = spam ? pressButton :releaseButton;
    }
}

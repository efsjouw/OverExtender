using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class AndroidLikeDialog : SingletonBehaviour<AndroidLikeDialog> {

    public Text titleText;
    public Text descriptionText;
    public Button negativeButton;
    public Button positiveButton;

    public GameObject dialogObject;
    public Image disableRaycastImage;
	
	void Start () {
        if (dialogObject.activeSelf) toggle();
    }
	
	public void show(string title, string description="", UnityAction positiveCallback = null, UnityAction negativeCallback = null)
    {
        titleText.text = title;
        descriptionText.text = description;
        positiveButton.onClick.RemoveAllListeners();
        if (positiveCallback == null) negativeButton.onClick.AddListener(close);
        else
        {
            positiveButton.onClick.AddListener(positiveCallback);
        }
        negativeButton.onClick.RemoveAllListeners();
        if (negativeCallback == null) negativeButton.onClick.AddListener(close);
        else
        {
            negativeButton.onClick.AddListener(negativeCallback);
        }
        toggle();
    }

    public void close()
    {
        toggle();
    }

    public void toggle()
    {
        dialogObject.SetActive(!dialogObject.activeSelf);
        disableRaycastImage.enabled = dialogObject.activeSelf;
    }
}

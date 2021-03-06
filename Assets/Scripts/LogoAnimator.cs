using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Animate the logo with upper/lower case
/// </summary>
public class LogoAnimator : MonoBehaviour {

    public Text textComponent;
    public float interval = 0.2f;

    private System.Random random;

    void Start () {
        random = new System.Random();
        StartCoroutine("animationRoutine");
    }

    void OnEnable()
    {
        StartCoroutine("animationRoutine");
    }

    void OnDisable()
    {
        StopCoroutine("animationRoutine");
    }

    private IEnumerator animationRoutine()
    {
        while(true)
        {
            this.doLowerUpperAnimation();
            yield return new WaitForSecondsRealtime(interval);
        }
    }

    /// <summary>
    /// Get a random char from the text string and change one character to either lower or upper
    /// </summary>
    private void doLowerUpperAnimation()
    {
        char[] chars = textComponent.text.ToCharArray();

        if(random == null) random = new System.Random();
        int changeIndex = random.Next(0, chars.Length);
        char changeChar = chars[changeIndex];

        //Quick fix for \n chars
        while(changeChar == '\n')
        {
            changeIndex = random.Next(0, chars.Length);
            changeChar = chars[changeIndex];
        }
        
        char changed = char.IsUpper(changeChar) ? char.ToLower(changeChar) : char.ToUpper(changeChar);
        chars[changeIndex] = changed;
        textComponent.text  = new string(chars);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public abstract class Estulo_PlayerCanvasScript : MonoBehaviour {

    public static Estulo_PlayerCanvasScript canvas;

    [Header("Component References")]
    [SerializeField] UIFader damageImage;
    [SerializeField] Text allText;
    [SerializeField] string gameStatusText;
    [SerializeField] string healthValue;
    [SerializeField] string killsValue;
    [SerializeField] string logText;
    [SerializeField] AudioSource deathAudio;

    //Ensure there is only one PlayerCanvas
    void Awake()
    {
        if (canvas == null)
            canvas = this;
        else if (canvas != this)
            Destroy(gameObject);
    }

    //Find all of our resources
    void Reset()
    {
        damageImage = GameObject.Find("DamagedFlash").GetComponent<UIFader>();

        allText = GameObject.Find("AllText").GetComponent<Text>();
        deathAudio = GameObject.Find("DeathAudio").GetComponent<AudioSource>();
    }

    public void Initialize()
    {
        //reticule.enabled = true;
        //null reference
        //gameStatusText.text = "";
    }

    public void HideReticule()
    {
        //reticule.enabled = false;
    }

    public void FlashDamageEffect()
    {
        damageImage.Flash();
    }

    public void PlayDeathAudio()
    {
        if (!deathAudio.isPlaying)
            deathAudio.Play();
    }

    public void SetKills(int amount)
    {
        string text = allText.text;
        string[] stringSeparators = new string[] { "\n" };
        string[] lines = text.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries);
        allText.text = lines[0] + "\n" + "Kills: " + amount.ToString() + "\n" + lines[2] + "\n" + lines[3] + "\n" + lines[4] + "\n" + lines[5];
    }

    public void SetHealth(int amount)
    {
        string text = allText.text;
        string[] stringSeparators = new string[] { "\n" };
        string[] lines = text.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries);
        Debug.Log(lines.Length);
        Debug.Log(lines[0]);
        Debug.Log(lines[1]);
        allText.text = "Health: " + amount.ToString() + "\n" + lines[1] + "\n" + lines[2] + "\n" + lines[3] + "\n" + lines[4] + "\n" + lines[5];
    }

    public void WriteGameStatusText(string text)
    {
       // gameStatusText.text = text;
    }

    public void WriteLogText(string text, float duration)
    {
        CancelInvoke();
        //logText.text = text;
        //Invoke("ClearLogText", duration);
    }

    void ClearLogText()
    {
        //logText.text = "";
    }
}

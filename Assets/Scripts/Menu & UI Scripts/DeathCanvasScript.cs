using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DeathCanvasScript : MonoBehaviour
{

    public String[] setupStrings;
    public String[] punchlineStrings;


    private TextMeshProUGUI setup;
    private TextMeshProUGUI punchline;
    private TextMeshProUGUI setupBackground;
    private TextMeshProUGUI punchlineBackground;


    void Awake()
    {
        setupBackground = transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        setup = transform.GetChild(3).GetComponent<TextMeshProUGUI>();
        punchlineBackground = transform.GetChild(4).GetComponent<TextMeshProUGUI>();
        punchline = transform.GetChild(5).GetComponent<TextMeshProUGUI>();
    }

    void OnEnable()
    {
        int chosenLine = UnityEngine.Random.Range(0, setupStrings.Length);

        setup.text = setupStrings[chosenLine];
        setupBackground.text = setupStrings[chosenLine];
        punchline.text = punchlineStrings[chosenLine];
        punchlineBackground.text = punchlineStrings[chosenLine];
    }
}

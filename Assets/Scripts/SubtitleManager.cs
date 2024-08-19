using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SubtitleManager : MonoBehaviour
{
    private List<string> subtitles = new List<string>();
    private List<float> time = new List<float>();
    [SerializeField] public TextMeshProUGUI textSubtitles; 

    private DataSubtitles data = new DataSubtitles();

    void Start()
    {
        subtitles = data.GetSubtitles();
        time = data.GetTime();

        StartCoroutine(StartSubtitles());
    }


    IEnumerator StartSubtitles()
    {
        for (int i = 0; i < subtitles.Count; i++)
        {
            float typingSpeed = (time[i] / subtitles[i].Length) - 0.02f;
            yield return StartCoroutine(TypeSentence(subtitles[i], typingSpeed));

            float remaingTime = time[i] - (subtitles[i].Length * typingSpeed);
            if (remaingTime > 0)
            {
                yield return new WaitForSeconds(remaingTime);
            }
        }
    }

    IEnumerator TypeSentence(string sentence, float typingSpeed)
    {
        textSubtitles.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            textSubtitles.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
    }
}

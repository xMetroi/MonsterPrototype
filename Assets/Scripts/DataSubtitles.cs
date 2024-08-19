using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataSubtitles : MonoBehaviour
{
    private List<string> subtitles = new List<string>();
    private List<float> time = new List<float>();

    public List<string> GetSubtitles()
    {
        subtitles.Add("In the peaceful village of Rumblemon, a");
        subtitles.Add("young monster trainer faces an");
        subtitles.Add("unexpected tragedy. A group of");
        subtitles.Add("ruthless villains has kidnapped one of");
        subtitles.Add("his beloved creatures. Now,");
        subtitles.Add("to save it, he must embark on a series of");
        subtitles.Add("challenging battles. The mission");
        subtitles.Add("won't be easy, but with courage and");
        subtitles.Add("determination, anything is");
        subtitles.Add("possible. Are you ready to face the");
        subtitles.Add("dangers and rescue your monster?");

        return subtitles;
    }

    public List<float> GetTime()
    {
        time.Add(3f);
        time.Add(2.5f);
        time.Add(2.5f);
        time.Add(3);
        time.Add(2.5f);
        time.Add(3);
        time.Add(3);
        time.Add(2.5f);
        time.Add(2.5f);
        time.Add(2.5f);
        time.Add(2.5f);
        time.Add(2.5f);
        time.Add(2.5f);

        return time;
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class WordFilter : MonoBehaviour
{

    private Text replyField;

    string[] badWords = { "anal", "anus", "arse", "ass", "ballsack", "balls", "bastard", "bitch", "biatch", "bloody", "blowjob", "blow job", "bollock", "bollok", "boner", "boob", "bugger", "bum", "butt", "buttplug", "clitoris", "cock", "coon", "crap", "cunt", "damn", "dick", "dildo", "dyke", "fag", "feck", "fellate", "fellatio", "felching", "fuck", "f u c k", "fudgepacker", "fudge packer", "flange", "Goddamn", "God damn", "hell", "homo", "jerk", "jizz", "knobend", "knob end", "labia", "lmao", "lmfao", "muff", "nigger", "nigga", "omg", "penis", "piss", "poop", "prick", "pube", "pussy", "queer", "scrotum", "sex", "shit", "s hit", "sh1t", "slut", "smegma", "spunk", "tit", "tosser", "turd", "twat", "vagina", "wank", "whore", "wtf" };

    private void Start()
    {
        replyField = GameObject.Find("replyField").GetComponent<Text>();
    }

    public bool checkWords()
    {
        string text1 = replyField.text.Replace('\n', ' ');
        string[] text = Regex.Replace(text1, "[^\\w\\._ ]", "").Split(' ');
        foreach (string s in text){
            if (!s.Equals(""))
            {
                print(s);
                foreach (string ss in badWords)
                {
                    if (s.Equals(ss))
                    {
                        print("not very cash munny of u");
                        return false;
                    }
                }
            }
        }
        return true;
    }
}

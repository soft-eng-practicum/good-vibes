using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostDB : MonoBehaviour
{
    public string[] posts;
    public int currentLen;

    public void addMsg(string s)
    {
        posts[currentLen] = s;
        currentLen++;
    }


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsLoader : MonoBehaviour
{
    public static bool activate = false;

    public GameObject credits;
    void Start()
    {
        if(activate) {
            activate = false;
            credits.SetActive(true);
        }
    }
}

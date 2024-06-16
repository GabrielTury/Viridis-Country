using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Transitioner : MonoBehaviour
{
    void Start()
    {
        SceneManager.LoadScene(PlayerPrefs.GetInt("LEVELID"));
    }
}

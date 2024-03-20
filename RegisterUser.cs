using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
public class RegisterUser : MonoBehaviour
{
    public TMP_InputField inputname;
    public void GoToScene(string LevelName)
    {
        PlayerPrefs.SetString("UserName", inputname.text);
        SceneManager.LoadScene(LevelName);
    }
}

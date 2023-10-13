using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class UIManager : MonoBehaviour
{
    public GameObject MainScreen, LayoutScreen;
    public Button continueBtn;
    string path;

    private void Start()
    {
        path = Application.persistentDataPath + "/gamedata.json";
        if (File.Exists(path))
            continueBtn.interactable = true;
        else
            continueBtn.interactable = false;
    }
    public void playPressed()
    {
        if(File.Exists(path))
        {
            File.Delete(path);
        }

        LayoutScreen.SetActive(true);
        MainScreen.SetActive(false);
    }

    public void continueGame()
    {
        if (File.Exists(path))
        {
            SceneManager.LoadSceneAsync(1);
        }
    }
    public void PuzzleLayout()
    {
        PlayerPrefs.SetInt("rows",2 );
        PlayerPrefs.SetInt("cols",2);

        LayoutScreen.SetActive(false);
        SceneManager.LoadSceneAsync(1);
       // GameScreen.SetActive(true);
    }

    public void PuzzleLayout2()
    {
        PlayerPrefs.SetInt("rows", 2);
        PlayerPrefs.SetInt("cols", 3);

        LayoutScreen.SetActive(false);
        SceneManager.LoadSceneAsync(1);
        // GameScreen.SetActive(true);
    }
    public void PuzzleLayout3()
    {
        PlayerPrefs.SetInt("rows", 5);
        PlayerPrefs.SetInt("cols", 6);

        LayoutScreen.SetActive(false);
        SceneManager.LoadSceneAsync(1);
        // GameScreen.SetActive(true);
    }


}

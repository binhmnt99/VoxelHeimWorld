using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TurnBase
{
    public class SceneMenu : MonoBehaviour
    {
        public void Play()
        {
            SceneManager.LoadScene(1);
        }
        public void Exit()
        {
            Application.Quit();
        }

        public void Return()
        {
            SceneManager.LoadScene(0);
        }
    }
}

using UnityEngine;
using UnityEngine.SceneManagement;

namespace ForgeHorizon.StarterAssetsAddons.ThirdPerson
{
    public class SimpleUI : MonoBehaviour
    {
    
        public GameObject menu;
        public GameObject controls;
        public GameObject main;
        public GameObject player;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }               
        }

        public void StartDemo()
        {
            menu.SetActive(false);
            main.SetActive(true);
            player.SetActive(true);
        }

        public void DemoControls()
        {
            menu.SetActive(false);
            controls.SetActive(true);
        }

        public void BackDemo()
        {
            menu.SetActive(true);
            controls.SetActive(false);
        }
        
        public void ExitDemo()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
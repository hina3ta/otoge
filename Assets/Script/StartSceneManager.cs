﻿using UnityEngine;
using UnityEngine.SceneManagement;

namespace Script
{
    public class StartSceneManager : MonoBehaviour
    {
        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return)) {
                SceneManager.LoadScene("PlayScene");
            }
        }
    }
}
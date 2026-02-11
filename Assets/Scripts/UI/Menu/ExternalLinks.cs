using System;
using UnityEngine;
using UnityEngine.Playables;

namespace UI.Menu
{
    public class ExternalLinks : MonoBehaviour
    {
        public void OpenURL(string targetUrl)
        {
            Application.OpenURL(targetUrl);
        }
    }
}

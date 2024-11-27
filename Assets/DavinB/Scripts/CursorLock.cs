using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DavinB
{
    public class CursorLock : MonoBehaviour
    {
        // Update is called once per frame
        void Awake()
        {
            Cursor.lockState = CursorLockMode.Confined;
        }
    }
}
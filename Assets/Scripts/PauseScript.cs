using UnityEngine;

namespace DefaultNamespace
{
    public class PauseScript: MonoBehaviour
    {
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Debug.Break();
            }
        }
    }
}
using UnityEngine;
using UnityEngine.SceneManagement;
using WorkstationDesigner.Workstation;

namespace WorkstationDesigner.Util
{
    public class SceneStartScript: MonoBehaviour
    {
        /// <summary>
        /// Number of cycles this script as run
        /// </summary>
        private int iterations = 0;

        /// <summary>
        /// Minimum number of cycles the script needs to run before the start up callbacks are run
        /// 
        /// This is used to let the scene finsih loading and be rendered for the user, in case the callback blocks the main thread
        /// 
        /// This value was chosen experimentally
        /// </summary>
        private const int ITERATION_THRESHOLD = 2;

        public void OnGUI()
        {
            if (this.enabled)
            {
                if (iterations >= ITERATION_THRESHOLD)
                {
                    // Always setup new workstation environment when entering main scene
                    if (SceneManager.GetActiveScene().name == AppUtil.MainSceneName)
                    {
                        WorkstationManager.New();
                    }

                    // Call on scene start callback
                    AppUtil.OnSceneStart();

                    // Disable this component because it won't need to run again
                    this.enabled = false;
                }
                iterations++;
            }
        }
    }
}

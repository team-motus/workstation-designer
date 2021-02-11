using UnityEngine;

namespace WorkstationDesigner.Util
{
    public class SceneStartScript: MonoBehaviour
    {
        public void Awake()
        {
            AppUtil.OnSceneStart();
        }
    }
}

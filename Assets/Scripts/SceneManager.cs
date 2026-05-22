using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class SceneLoader : MonoBehaviour
{
// This MonoBehaviour could be placed as a component inside the first scene in the Build Profiles Scene List.
// When the Player starts it instantiates this MonoBehaviour, which in turn loads
// an additional scene.

    // This scene must be listed in the Scene List in the Build Profiles Window,
    // or available from a loaded AssetBundle.
    public List<Scene> mySceneList;
    void Start()
    {
        var parameters = new LoadSceneParameters(LoadSceneMode.Additive);
        
        mySceneList.Add(SceneManager.LoadScene("Universe", parameters));
    }

    private void OnDestroy()
    {
        // When closing the Scene containing this MonoBehaviour we also remove the Scene we loaded
        SceneManager.UnloadSceneAsync("Universe");
    }
}
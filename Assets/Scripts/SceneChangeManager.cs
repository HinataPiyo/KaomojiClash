using System.Collections.Generic;
using UnityEngine;
using ENUM;

public class SceneChangeManager : MonoBehaviour
{
    public static SceneChangeManager I { get; private set; }

    public static readonly Dictionary<Scene, string> SceneNameDict = new Dictionary<Scene, string>()
    {
        { Scene.Title, "TitleScene" },
        { Scene.Home, "HomeScene" },
        { Scene.Battle, "BattleScene" },
    };

    public static Scene CurrentScene { get; private set; }
    public const Scene InitScene = Scene.Home;


    void Awake()
    {
        if(I == null)
        {
            I = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // if(CurrentScene != InitScene)
        // {
        //     ChangeScene(InitScene);
        // }
    }

    /// <summary>
    /// シーンを変更する
    /// </summary>
    /// <param name="scene">enumで定義したSceneタイプを渡す</param>
    public void ChangeScene(Scene scene)
    {
        if(SceneNameDict.ContainsKey(scene))
        {
            CurrentScene = scene;
            UnityEngine.SceneManagement.SceneManager.LoadScene(SceneNameDict[scene]);
        }
        else
        {
            Debug.LogError($"SceneChangeManager: Scene '{scene}' not found in SceneNameDict.");
        }
    }
}
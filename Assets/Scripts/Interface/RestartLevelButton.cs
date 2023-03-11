using MoreMountains.Feedbacks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class RestartLevelButton : InterfaceButton
{
    public void Start()
    {
        pointerClickFeedback.GetFeedbackOfType<MMF_LoadScene>().DestinationSceneName = SceneManager.GetActiveScene().name;
    }
}

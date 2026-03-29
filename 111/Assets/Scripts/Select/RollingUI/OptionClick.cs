using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;//使用IPointerClickHandler接口需将其引入
using UnityEngine.SceneManagement;//使用SceneManager需将其引入

public class OptionClick : MonoBehaviour, IPointerClickHandler
{

    public void OnPointerClick(PointerEventData eventData)
    {
        
        Debug.Log("进入" + GetComponentInChildren<Text>().text);
        
        //跳转场景
        //SceneManager.LoadScene(Index);
        //SceneManager.LoadScene("SceneName");
    }

}




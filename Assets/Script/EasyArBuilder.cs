using UnityEngine;
using System.Collections;
using EasyAR;
using UnityEngine.UI;
//                            _ooOoo_  
//                           o8888888o  
//                           88" . "88  
//                           (| -_- |)  
//                            O\ = /O  
//                        ____/`---'\____  
//                      .   ' \\| |// `.  
//                       / \\||| : |||// \  
//                     / _||||| -:- |||||- \  
//                       | | \\\ - /// | |  
//                     | \_| ''\---/'' | |  
//                      \ .-\__ `-` ___/-. /  
//                   ___`. .' /--.--\ `. . __  
//                ."" '< `.___\_<|>_/___.' >'"".  
//               | | : `- \`.;`\ _ /`;.`/ - ` : | |  
//                 \ \ `-. \_ __\ /__ _/ .-` / /  
//         ======`-.____`-.___\_____/___.-`____.-'======  
//                            `=---='  
//                           南无阿弥陀佛
//         .............................................  
//                  佛祖镇楼             BUG辟易  
//          佛曰:  
//                  写字楼里写字间，写字间里程序员；  
//                  程序人员写程序，又拿程序换酒钱。  
//                  酒醒只在网上坐，酒醉还来网下眠；  
//                  酒醉酒醒日复日，网上网下年复年。  
//                  但愿老死电脑间，不愿鞠躬老板前；  
//                  奔驰宝马贵者趣，公交自行程序员。  
//                  别人笑我忒疯癫，我笑自己命太贱；  
//                  不见满街漂亮妹，哪个归得程序员？ 
//
//小天 2017.10.25

public delegate void ScanCallBack(string data);

[RequireComponent(typeof(EasyARBehaviour))]
public class EasyArBuilder : MonoBehaviour
{
    public main testMain = null;

    public EasyARBehaviour easyar;

    void Awake()
    {
        easyar = GetComponent<EasyARBehaviour>();
        var id = Application.bundleIdentifier;
        
        //{
        //    easyar.Key = "eTuGUH9LVnbfWfRuDl6EEU7li2pfJWiEn5BhrPdNOWUlCVMm02qNrdHHEqk33n94PImibgoT4qN2KJ5W7ylL7y9n81Q1PVmZKqe1VcyB3eSpUSAQN3yXO7AmN5Q0PbJVT4T21U2GhFpUylDHwg9nedIgf56o7mQW6HL8modw7GhQ1L152S6KxxR34JJdsgzAHuXV2gpj";
        //}

        Debug.Log("Use EasyAR Key " + easyar.Key + " id=" + id);

        easyar.Initialize();

        //foreach (var behaviour in ARBuilder.Instance.ARCameraBehaviours)
        //{
        //    behaviour.TargetFound += OnTargetFound;
        //    behaviour.TextMessage += OnTextMessage;
        //}
        
        foreach (var behaviour in ARBuilder.Instance.ARCameraBehaviours)
        {
            behaviour.TargetFound += OnTargetFound;
            behaviour.TargetLost += OnTargetLost;
            behaviour.TextMessage += OnTextMessage;
        }
        foreach (var behaviour in ARBuilder.Instance.ImageTrackerBehaviours)
        {
            behaviour.TargetLoad += OnTargetLoad;
            behaviour.TargetUnload += OnTargetUnload;
        }


        
    }
    void OnTargetFound(ARCameraBaseBehaviour arcameraBehaviour, TargetAbstractBehaviour targetBehaviour, Target target)
    {
        Debug.Log("<Global Handler> Found: " + target.Name);

        //main.Capture();
        testMain.Set_Scan(target.Name, true);
    }


    //void OnTargetFound(ARCameraBaseBehaviour arcameraBehaviour, TargetAbstractBehaviour targetBehaviour, Target target)
    //{
    //    Debug.Log("<Global Handler> Found: " + target.Id);
    //}

    void OnTargetLost(ARCameraBaseBehaviour arcameraBehaviour, TargetAbstractBehaviour targetBehaviour, Target target)
    {
        //Debug.Log("<Global Handler> Lost: " + target.Id);
        testMain.Set_Scan(target.Name, false);
    }

    void OnTargetLoad(ImageTrackerBaseBehaviour trackerBehaviour, ImageTargetBaseBehaviour targetBehaviour, Target target, bool status)
    {
        Debug.Log("<Global Handler> Load target (" + status + "): " + target.Id + " (" + target.Name + ") " + " -> " + trackerBehaviour);
    }

    void OnTargetUnload(ImageTrackerBaseBehaviour trackerBehaviour, ImageTargetBaseBehaviour targetBehaviour, Target target, bool status)
    {
        Debug.Log("<Global Handler> Unload target (" + status + "): " + target.Id + " (" + target.Name + ") " + " -> " + trackerBehaviour);
    }

    private void OnTextMessage(ARCameraBaseBehaviour arcameraBehaviour, string text)
    {
        Debug.Log("got text: " + text);
    }

    void Update() 
    {
    }

}

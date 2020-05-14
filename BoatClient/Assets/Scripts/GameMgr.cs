using UnityEngine;
using System.Collections;

public class GameMgr : MonoBehaviour
{
    public static GameMgr instance;

    public string id = "Boat";

    // Use this for initialization
    void Awake()
    {
        instance = this;
    }
}

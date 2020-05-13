using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalSetting
{
    public static int lookMode = 0;
    public static bool isMapClick = false;
    public static Vector3 MapToWorldPos = Vector3.zero;
    //public static Vector3 playerPos = Vector3.zero;
    //场景中所有船只
    public static Dictionary<string, SimulateBoat> list = new Dictionary<string, SimulateBoat>();
}

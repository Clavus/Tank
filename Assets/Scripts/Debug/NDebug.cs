using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class NDebug {

    public static void Log(string str)
    {
        string prefix = "";
        if (NetworkServer.active)
            prefix += "[SERVER] ";

        if (NetworkClient.active)
            prefix += "[CLIENT] ";

        if (prefix == "")
            prefix += "[UNDEF] ";

        Debug.Log(prefix + str);
    }

    public static void PrintState(NetworkBehaviour script)
    {
        string print = "NetworkServer.active " + NetworkServer.active + ", "
                       + "isServer " + script.isServer + ", "
                       + "isClient " + script.isClient + ", "
                       + "isLocalPlayer " + script.isLocalPlayer;
        Debug.Log(print);
    }

}

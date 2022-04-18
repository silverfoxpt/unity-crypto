using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

public class APIMain : MonoBehaviour
{
    void Start()
    {
        // APIFetch newJoke = APIHelper.GetAJoke();    
        Debug.Log(GetPublicIPAddress()); 
    }

    //unlimited use
    private string GetPublicIPAddress()
    {
        string pubIp =  new System.Net.WebClient().DownloadString("https://api.ipify.org");
        return pubIp;
    }

    //https://www.weatherapi.com/api.aspx
    //https://freegeoip.app/
}

using LA2RTS;
using LA2RTS.LA2Entities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientContextMenuScript : ContextMenu
{

    public IEnumerable<RTSClient> Clients;

    public void SendLogInCommand()
    {
        SendLogInCommand1();
    }

    public void SendLogInCommand1()
    {
        if (Clients != null)
        {
            foreach (RTSClient cl in Clients)
            {
                if (cl != null && cl.UserChar.Status == LA2UserChar.ClientStatus.Off)
                {
                    cl.SendEnterCredentialsCommand("cschim_se1", "8PweCDQdJGVtoXo6");
                    break;
                }
            }
        }

    }

    public void SendLogInCommand2()
    {
        if (Clients != null)
        {
            foreach (RTSClient cl in Clients)
            {
                if (cl != null && cl.UserChar.Status == LA2UserChar.ClientStatus.Off)
                {
                    cl.SendEnterCredentialsCommand("k4elf_mage1", "UJpfPeXDcy7nqddf");
                    break;
                }
            }
        }

    }

    public void SendLogInCommand3()
    {
        if (Clients != null)
        {
            foreach (RTSClient cl in Clients)
            {
                if (cl != null && cl.UserChar.Status == LA2UserChar.ClientStatus.Off)
                {
                    cl.SendEnterCredentialsCommand("tcspoilod1d", "uaSRSB1H9xW5XylE");
                    break;
                }
            }
        }

    }

    public void SendLogInCommand4()
    {
        if (Clients != null)
        {
            foreach (RTSClient cl in Clients)
            {
                if (cl != null && cl.UserChar.Status == LA2UserChar.ClientStatus.Off)
                {
                    cl.SendEnterCredentialsCommand("w7epicwinhrtr", "XrC9iHCJkQ8TYMov");
                    break;
                }
            }
        }

    }

    public void SendEnterWorldCommand()
    {
        if (Clients != null)
            foreach (RTSClient cl in Clients)
            {
                if (cl != null)
                    cl.SendEnterWorldCommand();
            }
    }
}

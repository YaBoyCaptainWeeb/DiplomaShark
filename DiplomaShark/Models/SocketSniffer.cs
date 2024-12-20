using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace DiplomaShark.Models
{
    internal partial class SocketSniffer : ObservableObject
    {
        //[ObservableProperty]
        //private Socket _mainSocket;

        //public SocketSniffer(IPAddress address)
        //{
        //    MainSocket = new Socket(AddressFamily.InterNetwork,SocketType.Raw,ProtocolType.IP);
        //    MainSocket.Bind(new IPEndPoint(address, 0);
        //    MainSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.HeaderIncluded, true);
        //    MainSocket.IOControl()
        //}
    }
}

using DiplomaShark.ProtocolSniffers;
using PacketDotNet;
using SharpPcap;
using SkiaSharp;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DiplomaShark.Models
{
    public partial class SocketSniffer
    {
        private ObservableCollection<CapturedPacketInfo>? Packets = [];
        private ICaptureDevice? captureDevice;
        private ObservableCollection<CapturedPacketInfo>? capturedPacketInfos = [];
        private string? TargetInterfaceDescription { get; set; }
        private static int Counter { get; set; }
        RawCapture? raw;
        Packet? packet;


        //Task? task;
        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        CancellationToken cancel;

        public SocketSniffer(string description)
        {
            try
            {
                TargetInterfaceDescription = description;
                GetSocket();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

        }
        private void GetSocket()
        {
            Counter = 1;

            CaptureDeviceList deviceList = CaptureDeviceList.Instance;
            captureDevice = deviceList.First(x => x.Description == TargetInterfaceDescription);

            captureDevice.Open(DeviceModes.Promiscuous, 1000);
            cancel = cancellationTokenSource.Token;
            Task.Run(OnPacketArrival, cancel);
        }

        public void StopCapture()
        {
            cancellationTokenSource.Cancel();
            captureDevice!.Close();
        } 

        public void PauseCapture()
        {
            cancellationTokenSource.Cancel();
            //task!.Dispose();
        }

        public void ContinueCapture()
        {
            Task.Run(OnPacketArrival, cancel);
        }
        public ObservableCollection<CapturedPacketInfo> GetCapturedPacketInfos() // Попробовать с коллекцией Packets во ViewModel
        {
            return Packets!;
        }

        private async void OnPacketArrival()
        {
            try
            {
                while (true)
                {
                    captureDevice!.GetNextPacket(out PacketCapture e);
                    raw = e.GetPacket();
                    packet = Packet.ParsePacket(raw.LinkLayerType, raw.Data);

                    IPPacket ip = packet.Extract<IPPacket>();

                    IgmpPacket igmp = packet.Extract<IgmpPacket>();
                    IcmpV4Packet icmp = packet.Extract<IcmpV4Packet>();
                    TcpPacket tcp = packet.Extract<TcpPacket>();
                    UdpPacket udp = packet.Extract<UdpPacket>();

                    if (tcp != null)
                    {
                        BuildCapturedTCPPacket(tcp);
                        Counter++;
                    }
                    else if (udp != null)
                    {
                        BuildCapturedUDPPacket(udp);
                        Counter++;
                    }
                    else if (igmp != null)
                    {
                        BuildCapturedICMPPacket(icmp);
                        Counter++;
                    }
                    else if (igmp != null)
                    {
                        BuildCapturedIGMPPacket(igmp);
                        Counter++;
                    }


                    Packets = new ObservableCollection<CapturedPacketInfo>(capturedPacketInfos!);

                    await Task.Delay(3000);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private void BuildCapturedTCPPacket(TcpPacket tcp)
        {
            IPPacket tcpIP = packet!.Extract<IPPacket>();

            if (tcp != null)
            {
                DateTime time = raw!.Timeval.Date;
                int length = tcp.TotalPacketLength;
                int ttl = tcpIP.TimeToLive;
                
                string flags = ((int)tcp.Flags).ToString();
                string sequenceNum = tcp.SequenceNumber.ToString();
                bool ACK = tcp.Acknowledgment;
                string ACKNum = tcp.AcknowledgmentNumber.ToString();
                string WIN = tcp.WindowSize.ToString();
                bool push = tcp.Push, reset = tcp.Reset, sync = tcp.Synchronize, finished = tcp.Finished, urgent = tcp.Urgent;
                int urgentPointer = tcp.UrgentPointer;

                string sourceIPAddress = tcpIP.SourceAddress.ToString();
                string sourcePort = tcp.SourcePort.ToString();
                string destinationIPAddress = tcpIP.DestinationAddress.ToString();
                string destinationPort = tcp.DestinationPort.ToString();


                CapturedPacketInfo pack = new CapturedPacketInfo(
                    Counter, length, ttl, ProtocolSniffers.ProtocolType.TCP,
                    destinationIPAddress, destinationPort, sourceIPAddress, sourcePort, time.ToString("HH:mm:ss.ffff"), tcp.PrintHex(), tcp.ToString(StringOutputType.VerboseColored),
                    flags, sequenceNum, ACK, ACKNum, WIN, push, reset, sync, finished, urgent, urgentPointer
                    );

                capturedPacketInfos!.Add(pack);
            }
        }

        private void BuildCapturedUDPPacket(UdpPacket udp)
        {
            IPPacket udpIP = packet!.Extract<IPPacket>();

            if (udp != null)
            {
                DateTime time = raw!.Timeval.Date;
                int length = udp.TotalPacketLength;
                int ttl = udpIP.TimeToLive;


                string sourceIPAddress = udpIP.SourceAddress.ToString();
                string sourcePort = udp.SourcePort.ToString();
                string destinationIPAddress = udpIP.DestinationAddress.ToString();
                string destinationPort = udp.DestinationPort.ToString();

                CapturedPacketInfo pack = new CapturedPacketInfo(Counter, length, ttl, ProtocolSniffers.ProtocolType.UDP,
                    destinationIPAddress, destinationPort, sourceIPAddress, sourcePort,
                    time.ToString("dd/MM/yyyy HH:mm:ss.ffff"), udp.PrintHex(), udp.ToString());

                capturedPacketInfos!.Add(pack);
            }
        }

        private void BuildCapturedICMPPacket(IcmpV4Packet icmp)
        {
            IPPacket ICMPIP = packet!.Extract<IPPacket>();

            if (icmp != null)
            {
                DateTime time = raw!.Timeval.Date;
                int length = icmp.TotalPacketLength;
                int ttl = ICMPIP.TimeToLive;

                string sourceIPAddress = ICMPIP.SourceAddress.ToString();
                string sourcePort = string.Empty;
                string destinationIPAddress = ICMPIP.DestinationAddress.ToString();
                string destinationPort = string.Empty;


                int id = icmp.Id;

                CapturedPacketInfo pack = new CapturedPacketInfo(Counter, length, ttl, ProtocolSniffers.ProtocolType.ICMP,
                    destinationIPAddress, destinationPort, sourceIPAddress, sourcePort, time.ToString("HH:mm:ss.ffff"),
                    icmp.PrintHex(), icmp.ToString(), icmpId: id
                    );

                capturedPacketInfos!.Add(pack);
            }
        }

        private void BuildCapturedIGMPPacket(IgmpPacket igmp)
        {
            IPPacket IGMPIP = packet!.Extract<IPPacket>();

            if (igmp != null)
            {
                DateTime time = raw!.Timeval.Date;
                int length = igmp.TotalPacketLength;
                int ttl = IGMPIP.TimeToLive;

                string sourceIPAddress = IGMPIP.SourceAddress.ToString();
                string sourcePort = string.Empty;
                string destinationIPAddress = IGMPIP.DestinationAddress.ToString();
                string destinationPort = string.Empty;

                string type = igmp.Type.ToString();

                CapturedPacketInfo pack = new CapturedPacketInfo(Counter, length, ttl, ProtocolSniffers.ProtocolType.IGMP,
                    destinationIPAddress, destinationPort, sourceIPAddress, sourcePort, time.ToString("HH:mm:ss.ffff"),
                    igmp.PrintHex(), igmp.ToString(), iGMPType: type
                    );

                capturedPacketInfos!.Add(pack);
            }
        }
    }
}

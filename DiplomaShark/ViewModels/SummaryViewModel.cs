using CommunityToolkit.Mvvm.ComponentModel;
using DiplomaShark.Models;
using DiplomaShark.ProtocolSniffers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;

namespace DiplomaShark.ViewModels
{
    internal partial class SummaryViewModel : ViewModelBase
    {
        private List<CapturedPacketInfo>? Packets;
        [ObservableProperty]
        private ObservableCollection<SummaryModel> _summary = [];
        [ObservableProperty]
        private string _timeString = "00:00:00";
        [ObservableProperty]
        private string _packetsCount = "Количество пакетов: ";
        [ObservableProperty]
        private string _registeredProtocols = "Обнаруженные пакеты: \n";

        public SummaryViewModel(Collection<CapturedPacketInfo>? packets, string timeString)
        {
            Packets = new List<CapturedPacketInfo>(packets!);
            TimeString = "Время профилирования: " + timeString;
            BuildSummary();
        }

        private void BuildSummary()
        {
            if (Packets != null && Packets.Count > 0)
            {
                PacketsCount += Packets.Count;
                var x = Packets!.Select(x => x.ProtocolType).Where(x => x != ProtocolType.Uknown).Distinct().ToList();

                foreach (var item in x)
                {
                    RegisteredProtocols += item.ToString() + " - " + Packets!.Where(x => x.ProtocolType == item).Count() + "\n";
                }

                foreach (var item in Packets.Select(x => x).DistinctBy(x => x.SourceIPAddress).ToList())
                {
                    IPAddress addr = IPAddress.Parse(item.SourceIPAddress);
                    IPHostEntry? entry;
                    try
                    {
                        entry = Dns.GetHostEntry(addr);
                    }
                    catch (Exception)
                    {
                        entry = null;
                    }

                    SummaryModel summ = new SummaryModel()
                    {
                        IP = item.SourceIPAddress,
                        SendedPackets = Packets!.Where(x => item.SourceIPAddress == x.SourceIPAddress).Count(),
                        ReceivedPackets = Packets!.Where(x => item.SourceIPAddress == x.DestinationIPAddress).Count(),
                        Info = BuildDNSEntry(entry)

                    };

                    Summary.Add(summ);
                }
            }
        }

        private string BuildDNSEntry(IPHostEntry? entry)
        {
            if (entry != null)
            {
                string result = "Имя хоста:" + entry.HostName + "\nAlias-DNS записи:\n";

                foreach (var item in entry.Aliases)
                {
                    result += item.ToString() + "\n";
                }
                return result;
            } else
            {
                return null;
            }
        }
    }
}

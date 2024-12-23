using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Net.NetworkInformation;

namespace DiplomaShark.Models
{
    internal partial class Interfaces : ObservableObject
    {
        [ObservableProperty]
        private IPInterfaceStatistics? _statistics;
        [ObservableProperty]
        private SocketSniffer? _socketSniff;

        private string? _name;
        public string? Name
        {
            get => $"{(_name != string.Empty ? _name : "\nНе определено")}";
            set => _name = value;
        }
        private string? _interfaceDescription;
        public string? InterfaceDescription
        {
            get => $"{(_interfaceDescription != string.Empty ? _interfaceDescription : "\nНе определено")}";
            set
            {
                _interfaceDescription = value;
            }

        }

        private string? _interfaceType;
        public string? InterfaceType
        {
            get => $"{(_interfaceType != string.Empty ? _interfaceType : "\nНе определено")}";
            set
            {
                _interfaceType = value;
            }
        }

        private string? _ipAddress;
        public string? IpAddress
        {
            get => _ipAddress != string.Empty ? _ipAddress : "\nНе определено";
            set => _ipAddress = value;
        }

        private string? _IPv4Mask;
        public string? IPv4Mask
        {
            get => _IPv4Mask != string.Empty ? _IPv4Mask : "\nНе определено";
            set => _IPv4Mask = value;
        }

        private string? _GateWay;
        public string? GateWay
        {
            get => _GateWay != string.Empty ? _GateWay : "\nНе определено";
            set => _GateWay = value;
        }

        private string? _interfaceMAC;
        public string? InterfaceMAC
        {
            get => $"{(_interfaceMAC != string.Empty ? _interfaceMAC : "\nНе определено")}";
            set
            {
                _interfaceMAC = value;
            }
        }

        private int? _interfaceSpeed;
        public string? InterfaceSpeed
        {
            get => $"{_interfaceSpeed / 1000000} Мб/с";
            set
            {
                _interfaceSpeed = Convert.ToInt32(value);

            }
        }
    }
}

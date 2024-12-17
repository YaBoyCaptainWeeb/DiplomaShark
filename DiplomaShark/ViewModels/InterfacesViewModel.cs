using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DiplomaShark.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Windows.Input;

namespace DiplomaShark.ViewModels
{
    internal partial class InterfacesViewModel : ViewModelBase
    {
        #region Поля
        [ObservableProperty]
        private ObservableCollection<Interfaces>? _interfacesList;
        [ObservableProperty]
        private bool _AllChecked = true;
        [ObservableProperty]
        // private ObservableCollection 
        #endregion

        #region  Внутренние переменные
        private ObservableCollection<Interfaces>? networkInterfaces;
        #endregion

        #region Relay комманды
        #endregion

        public void StartProfiling()
        {
            if (!CanStartProfiling())
            {
                return;
            }
            List<NetworkInterface> adapters = NetworkInterface.GetAllNetworkInterfaces()
                .Where(x => x.OperationalStatus == OperationalStatus.Up && x.Speed > 0 && x.GetPhysicalAddress().ToString().Count() != 0)
                .ToList();

            // Нужно создать новую модель данных с 
            var x = adapters[0].GetIPv4Statistics();
                    
            if (adapters.Count() > 0)
            {
                
                networkInterfaces = new ObservableCollection<Interfaces>();
                foreach (var adapter in adapters)
                {
                    Interfaces network = new Interfaces()
                    {
                        InterfaceDescription = adapter.Description,
                        InterfaceType = adapter.NetworkInterfaceType.ToString(),
                        InterfaceMAC = adapter.GetPhysicalAddress().ToString(),
                        InterfaceSpeed = adapter.Speed.ToString(),
                    };

                    networkInterfaces.Add(network);

                }
                InterfacesList = networkInterfaces;
                GetInterfaceStatisticsInfo();
            }
        }

        private bool CanStartProfiling()
        {
            return NetworkInterface.GetAllNetworkInterfaces().ToArray().Length > 0;
        }

        private void GetInterfaceStatisticsInfo()
        {
            switch (AllChecked)
            {
                case true:
                    Debug.WriteLine("WIP");
                    break;
                default:
                    Debug.WriteLine("Пока что не реализовано");
                    break;
            }
        }
    }
}

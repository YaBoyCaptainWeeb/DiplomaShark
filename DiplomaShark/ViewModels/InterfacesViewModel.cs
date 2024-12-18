using Avalonia.Controls;
using Avalonia.Interactivity;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DiplomaShark.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace DiplomaShark.ViewModels
{
    internal partial class InterfacesViewModel : ViewModelBase
    {
        #region Поля
        [ObservableProperty]
        private ObservableCollection<Interfaces>? _interfacesList = [];
        [ObservableProperty]
        private bool _AllChecked = true;
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(StartProfilingCommand))]
        private bool _canStartProfiling;
        #endregion

        #region  Внутренние переменные
        public ObservableCollection<Interfaces>? ChoosenItems = [];

        #endregion

        #region Relay комманды // Получается, с атрибутами они не нужны?
        //private RelayCommand? getInterfaceStatisticsInfoCommand;
        //public RelayCommand GetInterfaceStatisticsInfoCommand => getInterfaceStatisticsInfoCommand ??= new RelayCommand<SelectionChangedEventArgs>(GetInterfaceStatisticsInfo);
        //public IRelayCommand? GetListBoxPointerCommand
        #endregion

        #region Controls элементы, указатели
        ListBox? listBox;
        #endregion

        public InterfacesViewModel()
        {
            //_getInterfaceStatisticsInfo = new RelayCommand<SelectionChangedEventArgs>(GetInterfaceStatisticsInfo);
            //GetListBoxPointerCommand = new RelayCommand<RoutedEventArgs>(GetListBoxPointer);
            _canStartProfiling = NetworkInterface.GetAllNetworkInterfaces().ToArray().Length > 0;
        }
        [RelayCommand(CanExecute = nameof(CanStartProfiling))]
        private void StartProfiling()
        {
            CanStartProfiling = false;

            List<NetworkInterface> adapters = NetworkInterface.GetAllNetworkInterfaces()
                .Where(x => x.OperationalStatus == OperationalStatus.Up && x.Speed > 0 && (x.NetworkInterfaceType == NetworkInterfaceType.Ethernet || x.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 || x.NetworkInterfaceType == NetworkInterfaceType.FastEthernetT || x.NetworkInterfaceType == NetworkInterfaceType.GigabitEthernet))
                .ToList();
            

            // Дорабатываем статистику интерфейсов
            var x = adapters[0].GetIPv4Statistics();
            var y = adapters[0].GetIPStatistics();

            if (adapters.Count() > 0)
            {

                ObservableCollection<Interfaces>? networkInterfaces = new ObservableCollection<Interfaces>();
                foreach (var adapter in adapters)
                {
                    Interfaces network = new Interfaces()
                    {
                        Name = adapter.Name,
                        InterfaceDescription = adapter.Description,
                        InterfaceType = adapter.NetworkInterfaceType.ToString(),
                        InterfaceMAC = adapter.GetPhysicalAddress().ToString(),
                        InterfaceSpeed = adapter.Speed.ToString(),
                    };

                    networkInterfaces.Add(network);

                }
                InterfacesList = networkInterfaces;
                GetInterfaceStatisticsInfo(listBox!);
            }
        }

        [RelayCommand]
        private void GetInterfaceStatisticsInfo(SelectionChangedEventArgs? e)
        {
            try
            {

                switch (AllChecked)
                {
                    case true:
                        foreach (Interfaces item in (e.Source as ListBox).Items)
                        {
                            Debug.WriteLine(item.InterfaceDescription);
                        }
                        break;
                    default:
                        Debug.WriteLine("Пока что не реализовано");
                        break;
                }

            }
            catch (NullReferenceException)
            {
                return;
            }

        }
        [RelayCommand]
        private void GetListBoxPointer(RoutedEventArgs? e) // Так проще. Изначально хранить указатель на список, вместо того, чтобы пытаться через
                                                           // CommunityToolkit создать свое собственное подходящее событие для списка и т.д
        {
            listBox = (e!.Source as ListBox)!;

        }
        private void GetInterfaceStatisticsInfo(ListBox list)
        {
            try
            {

                switch (AllChecked)
                {
                    case true:
                        foreach (Interfaces item in list.Items)
                        {
                            Debug.WriteLine(item.InterfaceDescription);
                        }
                        break;
                    default:
                        Debug.WriteLine("Пока что не реализовано");
                        break;
                }

            }
            catch (NullReferenceException)
            {
                return;
            }

        }
    }
}

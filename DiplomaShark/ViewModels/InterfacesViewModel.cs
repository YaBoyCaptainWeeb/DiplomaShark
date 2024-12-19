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
using System.Reactive.Linq;

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
        public List<Interfaces> ListBoxChoosenItems = [];
        //public ObservableCollection<IPInterfaceStatistics> ChoosenInterfaceStat { get; set; }
        //public IPGlobalStatistics AllInterfacesStat { get; set; }
        #endregion

        #region Controls элементы, указатели
        ListBox? listBox;
        DataGrid? StatisticsDataGrid;
        #endregion

        public InterfacesViewModel()
        {
            _canStartProfiling = NetworkInterface.GetAllNetworkInterfaces().ToArray().Length > 0;
        }
        [RelayCommand(CanExecute = nameof(CanStartProfiling))]
        private void StartProfiling()
        {
            CanStartProfiling = false;
            List<NetworkInterface> adapters = NetworkInterface.GetAllNetworkInterfaces()
            .Where(x => x.OperationalStatus == OperationalStatus.Up && (x.NetworkInterfaceType == NetworkInterfaceType.Ethernet || x.NetworkInterfaceType == NetworkInterfaceType.Wireless80211))
                .ToList();

            if (adapters.Count() > 0)
            {
                ObservableCollection<Interfaces>? networkInterfaces = [];

                foreach (var adapter in adapters)
                {
                    IPInterfaceProperties props = adapter.GetIPProperties();
                    UnicastIPAddressInformation? UnicastAddresses = props.UnicastAddresses
                            .FirstOrDefault(x => x.Address.AddressFamily == AddressFamily.InterNetwork);

                    if (UnicastAddresses != null)
                    {
                        Interfaces network = new Interfaces()
                        {
                            Statistics = adapter.GetIPStatistics(),

                            Name = adapter.Name,
                            InterfaceDescription = adapter.Description,
                            InterfaceType = adapter.NetworkInterfaceType.ToString(),
                            IpAddress = UnicastAddresses.Address.ToString(),
                            IPv4Mask = UnicastAddresses.IPv4Mask.ToString(),
                            GateWay = props.GatewayAddresses.FirstOrDefault()!.Address.ToString(),
                            InterfaceMAC = adapter.GetPhysicalAddress().ToString(),
                            InterfaceSpeed = adapter.Speed.ToString(),
                        };

                        networkInterfaces.Add(network);
                    }
                }
                InterfacesList = networkInterfaces;
                GetInterfaceStatisticsInfo(listBox!);
            }
        }

        [RelayCommand]
        private void GetInterfaceStatisticsInfo(SelectionChangedEventArgs? e) // При выборе элемента из списка
        {
            ListBox list = (e!.Source as ListBox)!;
            IEnumerable<Interfaces> itemsList = list.SelectedItems!.Cast<Interfaces>();

            try
            {
                if (AllChecked)
                {
                    //AllChecked = false;
                    ListBoxChoosenItems = itemsList.ToList();

                    ListBoxChoosenItems.ForEach(x => Debug.WriteLine($"{x.Name}"));
                }
                else
                {
                    ListBoxChoosenItems = itemsList.ToList();

                    ListBoxChoosenItems.ForEach(x => Debug.WriteLine($"{x.Name}"));

                }

            }
            catch (NullReferenceException)
            {
                return;
            }

        }
        [RelayCommand(CanExecute = nameof(AllChecked))]
        private void LoadAllItems()
        {
            try
            {
                ListBoxChoosenItems.Clear();

                foreach (Interfaces item in listBox.Items)
                {
                    Debug.WriteLine(item.InterfaceDescription);
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
        [RelayCommand]
        private void GetDatagridPointer(RoutedEventArgs? e) 
        {
            StatisticsDataGrid = (e!.Source as DataGrid)!;
        }

        private void GetInterfaceStatisticsInfo(ListBox list)
        { // ДОДЕЛАТЬ
            
            try
            {
                if (AllChecked)
                {
                    foreach (Interfaces item in list.Items)
                    {
                        Debug.WriteLine(item.InterfaceDescription);
                    }
                } else
                {
                    return;
                }
                
            }
            catch (NullReferenceException)
            {
                return;
            }

        }

    }
}

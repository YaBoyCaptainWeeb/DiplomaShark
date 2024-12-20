using Avalonia.Controls;
using Avalonia.Interactivity;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DiplomaShark.Models;
using MsBox.Avalonia;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using SharpPcap;

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

        #region Relay комманды

        #endregion

        #region  Внутренние переменные
        private List<Interfaces> ListBoxChoosenItems = [];
        private List<NetworkInterface> Adapters = [];
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
            try
            {
                CanStartProfiling = false;
                Adapters = NetworkInterface.GetAllNetworkInterfaces()
                .Where(x => x.OperationalStatus == OperationalStatus.Up && (x.NetworkInterfaceType == NetworkInterfaceType.Ethernet || x.NetworkInterfaceType == NetworkInterfaceType.Wireless80211))
                    .ToList();

                if (Adapters.Count() > 0)
                {
                    ObservableCollection<Interfaces>? networkInterfaces = [];

                    foreach (var adapter in Adapters)
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
                    InterfacesList = networkInterfaces; // Реализовать SharPcap и PacketDotNet прослушку траффика.

                    CaptureDeviceList devices = CaptureDeviceList.Instance;
                    foreach(ICaptureDevice dev in devices)
                    {
                        Debug.WriteLine(dev.ToString());
                    }

                    RefreshGlobasIPStatsCommand.Execute(null);
                }
            }
            catch (Exception ex)
            {
                var box = MessageBoxManager.GetMessageBoxStandard("Ошибка профилирования", ex.Message, MsBox.Avalonia.Enums.ButtonEnum.Ok);

                box.ShowAsync();
                return;
            }
        }

        [RelayCommand]
        private void GetInterfaceStatisticsInfo(SelectionChangedEventArgs? e) // При выборе элемента из списка
        { // ---------------------------------------------------------------------------- WIP
            ListBox list = (e!.Source as ListBox)!;
            IEnumerable<Interfaces> itemsList = list.SelectedItems!.Cast<Interfaces>();

            try
            {
                if (itemsList.Count() > 0)
                {
                    AllChecked = false;
                    ListBoxChoosenItems = itemsList.ToList();

                    ListBoxChoosenItems.ForEach(x => Debug.WriteLine($"{x.Name}"));

                    StatisticsDataGrid!.ItemsSource = ListBoxChoosenItems;
                }
                return;
            }
            catch (Exception ex)
            {
                var box = MessageBoxManager.GetMessageBoxStandard("Ошибка выбора интерфейсов", ex.Message, MsBox.Avalonia.Enums.ButtonEnum.Ok);

                box.ShowAsync();
                return;
            }

        }
        [RelayCommand(IncludeCancelCommand = true)]
        private async Task RefreshGlobasIPStats(CancellationToken token)
        {
            try
            {
                NetworkInterface currentAdapter;

                while (true)
                {
                    token.ThrowIfCancellationRequested();
                    await Task.Delay(1000);

                    foreach (Interfaces inter in InterfacesList!)
                    {
                        currentAdapter = Adapters.First(x => x.Description == inter.InterfaceDescription);
                       
                        inter.Statistics = currentAdapter.GetIPStatistics();
                    }
                }
            } catch (Exception ex)
            {
                CanStartProfiling = true;
                InterfacesList!.Clear();
                Debug.WriteLine(ex.Message);
            }
        }


        [RelayCommand(CanExecute = nameof(AllChecked))]
        private void LoadAllItems() // загружает все элементы в listbox, если проставить чекбокс
        {
            try
            {
                ListBoxChoosenItems.Clear();
                StatisticsDataGrid!.ItemsSource = InterfacesList;
                listBox!.UnselectAll();
                return;
            }
            catch (Exception ex)
            {
                var box = MessageBoxManager.GetMessageBoxStandard("Ошибка загрузки статистики", ex.Message, MsBox.Avalonia.Enums.ButtonEnum.Ok);

                box.ShowAsync();

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
    }
}

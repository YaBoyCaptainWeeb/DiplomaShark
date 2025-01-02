using Avalonia.Controls;
using Avalonia.Interactivity;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DiplomaShark.Models;
using DiplomaShark.ProtocolSniffers;
using MsBox.Avalonia;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DiplomaShark.ViewModels
{
    internal partial class InterfacesViewModel : ViewModelBase
    {
        #region Поля
        [ObservableProperty]
        private ObservableCollection<Interfaces>? _interfacesList = [];

        [ObservableProperty]
        private ObservableCollection<CapturedPacketInfo>? _packets = [];

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(StartProfilingCommand))]
        private bool _canStartProfiling;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(GetInterfaceStatisticsInfoCommand))]
        private bool _canGetInterfaceStatisticsInfo;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(PauseProfilingCommand))]
        private bool _canPauseProfiling = false;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(ContinueProfilingCommand))]
        private bool _canContinueProfiling = false;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(StopProfilingCommand))]
        private bool _canStopProfiling = false;
        #endregion

        #region  Внутренние переменные 
        private Interfaces? ListBoxChoosenItem; // Переделать на ОДИН выбранный интерфейс
        private List<NetworkInterface> Adapters = [];
        private bool PauseOrNot;


        #endregion

        #region Controls элементы, указатели
        ListBox? listBox;
        DataGrid? StatisticsDataGrid, PacketsDataGrid;
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
                PauseOrNot = false;
                CanStartProfiling = false;
                CanStopProfiling = true;
                CanPauseProfiling = true;

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
                                SocketSniff = new SocketSniffer(adapter.Description),

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

                    CanGetInterfaceStatisticsInfo = InterfacesList.Count > 1;
                    ListBoxChoosenItem = InterfacesList[0];

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

        [RelayCommand(CanExecute = nameof(CanGetInterfaceStatisticsInfo))]
        private void GetInterfaceStatisticsInfo(SelectionChangedEventArgs? e) // При выборе элемента из списка
        { // ---------------------------------------------------------------------------- WIP
            ListBox list = (e!.Source as ListBox)!;
            ObservableCollection<Interfaces> selected = new() { (list.SelectedItem! as Interfaces)! };

            try
            {
                if (selected != null && selected[0] != ListBoxChoosenItem)
                {
                    ListBoxChoosenItem = selected[0];

                    StatisticsDataGrid!.ItemsSource = selected;
                    Packets = ListBoxChoosenItem!.SocketSniff!.GetCapturedPacketInfos();
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
                NetworkInterface currentAdapter = Adapters.First(x => x.Description == ListBoxChoosenItem!.InterfaceDescription);


                while (true)
                {
                    await Task.Delay(3000);
      
                    ListBoxChoosenItem!.Statistics = currentAdapter.GetIPStatistics();
                    Packets = ListBoxChoosenItem!.SocketSniff!.GetCapturedPacketInfos();

                    token.ThrowIfCancellationRequested();
                }
            }
            catch (Exception ex)
            {
                if (ex is OperationCanceledException)
                {
                    if (PauseOrNot)
                    {
                        ListBoxChoosenItem!.SocketSniff!.StopCapture();
                        CanStartProfiling = true;
                        CanStopProfiling = false;
                        CanPauseProfiling = false;
                        CanContinueProfiling = false;
                        InterfacesList = [];
                        Packets = [];
                        var msgbox = MessageBoxManager.GetMessageBoxStandard("Профилирование закончено", $"Профилирование остановлено", MsBox.Avalonia.Enums.ButtonEnum.Ok);

                        await msgbox.ShowAsync();
                    }
                    else
                    {
                        var box = MessageBoxManager.GetMessageBoxStandard("Профилирование приостановлено", $"Профилирование приостановлено", MsBox.Avalonia.Enums.ButtonEnum.Ok, windowStartupLocation: WindowStartupLocation.CenterOwner);

                        await box.ShowAsync();
                    }
                }
                else
                {
                    var box = MessageBoxManager.GetMessageBoxStandard("Ошибка", ex.Message, MsBox.Avalonia.Enums.ButtonEnum.Ok, windowStartupLocation: WindowStartupLocation.CenterOwner);

                    await box.ShowAsync();
                }

            }
        }
        [RelayCommand(CanExecute = nameof(CanStopProfiling))]
        private void StopProfiling()
        {
            PauseOrNot = true;
            if (RefreshGlobasIPStatsCancelCommand.CanExecute(null))
            {
                RefreshGlobasIPStatsCancelCommand.Execute(null);
            } else
            {
                ListBoxChoosenItem!.SocketSniff!.StopCapture();
                CanStartProfiling = true;
                CanStopProfiling = false;
                CanPauseProfiling = false;
                CanContinueProfiling = false;
                InterfacesList = [];
                Packets = [];
                var msgbox = MessageBoxManager.GetMessageBoxStandard("Профилирование закончено", $"Профилирование остановлено", MsBox.Avalonia.Enums.ButtonEnum.Ok, windowStartupLocation: WindowStartupLocation.CenterOwner);

                msgbox.ShowAsync();
            }
        }
        [RelayCommand(CanExecute = nameof(CanPauseProfiling))]
        private void PauseProfiling()
        {
            CanPauseProfiling = false;
            CanContinueProfiling = true;

            ListBoxChoosenItem?.SocketSniff?.PauseCapture();
            RefreshGlobasIPStatsCancelCommand.Execute(null);

        }
        [RelayCommand(CanExecute = nameof(CanContinueProfiling))]
        private void ContinueProfiling()
        {
            CanContinueProfiling = false;
            CanPauseProfiling = true;

            RefreshGlobasIPStatsCommand.Execute(null);
            ListBoxChoosenItem?.SocketSniff?.ContinueCapture();
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
        [RelayCommand]
        private void GetPacketDatagridPointer(RoutedEventArgs? e)
        {
            PacketsDataGrid = (e!.Source as DataGrid);
        }
    }
}

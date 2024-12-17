using CommunityToolkit.Mvvm.ComponentModel;
using DiplomaShark.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace DiplomaShark.Models
{
    internal class Interfaces
    {
        private string? _interfaceDescription;
        public string InterfaceDescription
        {
            get => $"Название интерфейса: \n{(_interfaceDescription != string.Empty ? _interfaceDescription : "\nНе определено")}";
            set
            {
                _interfaceDescription = value;
            }
        }

        private string? _interfaceType;
        public string InterfaceType
        {
            get => $"Тип интерфейса: \n{(_interfaceType != string.Empty ? _interfaceType : "\nНе определено")}";
            set
            {
                _interfaceType = value;
            }
        }

        private string? _interfaceMAC;
        public string InterfaceMAC
        {
            get => $"Физический адрес интерфейса: \n{(_interfaceMAC != string.Empty ? _interfaceMAC : "\nНе определено")}";
            set
            {
                _interfaceMAC = value;
            }
        }

        private int? _interfaceSpeed;
        public string InterfaceSpeed
        {
            get => $"Пропускная способность интерфейса: \n{_interfaceSpeed / 1000000} Мб/с";
            set
            {
                _interfaceSpeed = Convert.ToInt32(value);

            }
        }
    }
}

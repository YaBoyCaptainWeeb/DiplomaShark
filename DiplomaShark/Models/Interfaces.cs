using System;

namespace DiplomaShark.Models
{
    internal class Interfaces
    {
        private string? _name;
        public string Name
        {
            get => $"Имя устройства: \n{(_name != string.Empty ? _name : "\nНе определено")}";
            set => _name = value;
        }
        private string? _interfaceDescription;
        public string InterfaceDescription
        {
            get => $"Описание интерфейса: \n{(_interfaceDescription != string.Empty ? _interfaceDescription : "\nНе определено")}";
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

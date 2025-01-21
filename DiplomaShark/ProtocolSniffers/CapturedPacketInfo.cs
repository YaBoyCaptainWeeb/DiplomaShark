using CommunityToolkit.Mvvm.ComponentModel;
using DiplomaShark.ViewModels;

namespace DiplomaShark.ProtocolSniffers
{
    public enum ProtocolType
    {
        TCP,
        UDP,
        ICMP,
        IGMP,
        ARP,
        Uknown
    }
    public partial class CapturedPacketInfo : ViewModelBase
    {
        [ObservableProperty]
        private int? number;
        private int? IcmpId;
        private int length;
        private int TTL;
        [ObservableProperty]
        private ProtocolType protocolType;
        [ObservableProperty]
        private string? destinationIPAddress = string.Empty;
        private string destinationPort = string.Empty;
        [ObservableProperty]
        private string sourceIPAddress = string.Empty;
        private string sourcePort = string.Empty;
        [ObservableProperty]
        private string time = string.Empty;

        [ObservableProperty]
        private string hexContent = string.Empty;
        [ObservableProperty]
        private string testString = string.Empty;

        private string? flags = null;
        private string? sequenceNumber = null;
        private bool? ACK;
        private string? ACKNum = null;
        private string? WIN = null;
        private bool? PUSH = null;
        private bool? RESET = null;
        private bool? SYNC = null;
        private bool? FIN = null;
        private bool? URGENT = null;
        private int? urgentPointer = null;
        private string? IGMPType = null;
        private int? HardwareAddressLength = null;
        private string? HardwareAddressType = null;
        private string? Operation = null;
        private int? ProtocolAddressLength = null;
        private string? ProtocolAddressType = null;
        private string? SenderHardwareAddress = null;
        private string? SenderProtocolAddress = null;
        private string? TargetHardwarAaddress = null;
        private string? TargetProtocolAddress = null;

        public CapturedPacketInfo(
            int number, int length, int tTL,
            ProtocolType protocolType,
            string destinationIPAddress, string destinationPort, string sourceIPAddress, string sourcePort,
            string time,
            string hexContent, string testString,
            string? flags = null, string? sequenceNumber = null, bool? aCK = null, string? aCKNum = null,
            string? wIN = null, bool? pUSH = null, bool? rESET = null, bool? sYNC = null,
            bool? fIN = null, bool? uRGENT = null, int? urgentPointer = null, int? icmpId = null, string? iGMPType = null,
            string? protocoladresstype = null, int? protocoladdresslength = null,
            int? hardwareAddressLength = null, string? hardwareAddressType = null, string? senderHardwareAddress = null,
            string? senderProtocolAddress = null, string? targetHardwarAaddress = null, string? targetProtocolAddress = null, string? operation = null)
        {
            this.number = number;
            IcmpId = icmpId;
            this.length = length;
            TTL = tTL;
            ProtocolType = protocolType;
            this.destinationIPAddress = destinationIPAddress;
            this.destinationPort = destinationPort;
            this.sourceIPAddress = sourceIPAddress;
            this.sourcePort = sourcePort;
            this.time = time;
            this.hexContent = hexContent;
            this.testString = testString;
            this.flags = flags;
            this.sequenceNumber = sequenceNumber;
            ACK = aCK;
            ACKNum = aCKNum;
            WIN = wIN;
            PUSH = pUSH;
            RESET = rESET;
            SYNC = sYNC;
            FIN = fIN;
            URGENT = uRGENT;
            this.urgentPointer = urgentPointer;
            IGMPType = iGMPType;
            ProtocolAddressType = protocoladresstype;
            ProtocolAddressLength = protocoladdresslength;
            HardwareAddressLength = hardwareAddressLength;
            HardwareAddressType = hardwareAddressType;
            SenderHardwareAddress = senderHardwareAddress;
            SenderProtocolAddress = senderProtocolAddress;
            TargetHardwarAaddress = targetHardwarAaddress;
            TargetProtocolAddress = targetProtocolAddress;
        }

        public string InfoString
        {
            get
            {
                switch (this.ProtocolType)
                {
                    case ProtocolType.TCP:
                        return $"{sourcePort}->{destinationPort} TTL={TTL} {{[SYN={SYNC}] | [ACK={ACK}]}} ACKNum={ACKNum} \nSeq={sequenceNumber} Win={WIN} Len={length}" + "\n" +
                              $"PSH={PUSH} RST={RESET} FIN={FIN} " + (URGENT == true ? $"URG={URGENT} URG_Pointer={urgentPointer}" : $"URG={URGENT}");


                    case ProtocolType.UDP:
                        return $"{sourcePort} -> {destinationPort} TTL={TTL} Len={length}";

                    case ProtocolType.ICMP:
                        return $"{sourcePort} -> {destinationPort} TTL={TTL} ICMP_Id={IcmpId}";

                    case ProtocolType.IGMP:
                        return $"{sourcePort} -> {destinationPort} TTL={TTL} IGMP_Type={IGMPType}";

                    case ProtocolType.ARP:
                        return $"{sourcePort} -> {destinationPort} TTL={TTL} HardwareAddressLength={HardwareAddressLength} HardwareAddressType={HardwareAddressType} \n" +
                            $"ProtocolAddressLength={ProtocolAddressLength} SenderHardwareAddress={SenderHardwareAddress} SebderProtocolAddress={SenderProtocolAddress} \n" +
                            $"TargetHardwareAddress={TargetHardwarAaddress} TargetProtocolAddress={TargetProtocolAddress}";

                    default:
                        return TestString;
                }
            }
        }
    }
}

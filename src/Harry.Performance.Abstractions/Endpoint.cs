using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.InteropServices;

namespace Harry.Performance
{
    [StructLayout(LayoutKind.Auto)]
    public struct EndPoint
    {
        public IPAddress IPAddress { get; set; }

        public int Port { get; set; }

        public string ServiceName { get; set; }


        public bool Equals(EndPoint other) => other.Port == Port && other.IPAddress == this.IPAddress && Equals(other.ServiceName, this.ServiceName);

        public override bool Equals(object obj) => obj is EndPoint && Equals((EndPoint)obj);

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Port.GetHashCode();
                hashCode = (hashCode * 397) ^ (IPAddress != null ? IPAddress.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (ServiceName != null ? ServiceName.GetHashCode() : 0);
                return hashCode;
            }
        }

        public static bool operator ==(EndPoint left, EndPoint right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(EndPoint left, EndPoint right)
        {
            return !left.Equals(right);
        }

        public override string ToString()
        {
            return $"[{IPAddress}:{Port}]{ServiceName}";
        }
    }
}

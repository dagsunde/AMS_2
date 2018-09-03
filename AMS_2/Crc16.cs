﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMS_2 {
    public static class Crc16 {

        const ushort polynomial = 0xA001;
        static readonly ushort[] table = new ushort[ 256 ];

        static Crc16() {
            ushort value;
            ushort temp;
            for ( ushort i = 0; i < table.Length; ++i ) {
                value = 0;
                temp = i;
                for ( byte j = 0; j < 8; ++j ) {
                    if ( ( ( value ^ temp ) & 0x0001 ) != 0 ) {
                        value = (ushort)( ( value >> 1 ) ^ polynomial );
                    } else {
                        value >>= 1;
                    }
                    temp >>= 1;
                }
                table[ i ] = value;
            }
        }
        public static ushort ComputeChecksum( byte[] bytes, int start, int endPos ) {
            ushort crc = 0;
            for ( int i = start; i < endPos; ++i ) {
                byte index = (byte)( crc ^ bytes[ i ] );
                crc = (ushort)( ( crc >> 8 ) ^ table[ index ] );
            }
            return crc;
        }
    }
}

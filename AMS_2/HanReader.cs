using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;

using log4net;


namespace AMS_2 {

    public class HanReader {

        //private readonly ILog log = LogManager.GetLogger( typeof( OrderMonitor ) );

        private static readonly log4net.ILog log = LogManager.GetLogger( System.Reflection.MethodBase.GetCurrentMethod().DeclaringType );

        public const uint dataHeader = 8;
        public bool compensateFor09HeaderBug = false;

        private SerialPort han;
        private byte[] buffer = new byte[ 512 ];
        private int bytesRead;
        private DlmsReader reader;
        private int listSize;

        private bool debug = true;
        public HanReader() {

        }

        public void setup( SerialPort hanPort ) {
            setup( hanPort, "COM3", 2400 );
        }

        public void setup( SerialPort hanPort, string portName, ulong baudrate ) {

            // Initialize H/W serial port for MBus communication
            if ( hanPort != null ) {
                hanPort = new SerialPort( portName, (int)baudrate, Parity.None, 8, StopBits.One );
                hanPort.Open();
            }

            han = hanPort;
            bytesRead = 0;

            reader = new DlmsReader();

        }

        public bool read() {

            if ( han.BytesToRead > 0 ) {
                byte newByte = (byte)han.ReadByte();
                return read( newByte );
            }
            return false;

        }

        public bool read( byte data ) {

            bool retVal = false;

            if ( reader.Read( data ) ) {
                bytesRead = reader.GetRawData( buffer, 0, 512 );
                if ( debug ) {
                    log.DebugFormat( "Got valid DLMS data ({0} bytes):", bytesRead );
                    debugPrint( buffer, 0, bytesRead );
                }

                /*
                    Data should start with E6 E7 00 0F
                    and continue with four bytes for the InvokeId
                */
                if ( bytesRead < 9 ) {
                    if ( debug ) log.Debug( "Invalid HAN data: Less than 9 bytes received" );
                    retVal = false;
                } else if (
                      buffer[ 0 ] != 0xE6 ||
                      buffer[ 1 ] != 0xE7 ||
                      buffer[ 2 ] != 0x00 ||
                      buffer[ 3 ] != 0x0F
                  ) {
                    if ( debug ) log.Debug( "Invalid HAN data: Start should be E6 E7 00 0F" );
                    retVal = false;
                }

                if ( debug ) log.Debug( "HAN data is valid" );
                listSize = getInt( 0, buffer, 0, bytesRead );
                retVal = true;
            }

            return retVal;
        }

        public int getListSize() {
            return listSize;
        }

        public DateTime getPackageTime() {

            int packageTimePosition = (int)dataHeader
                + ( compensateFor09HeaderBug ? 1 : 0 );

            return getTime( buffer, packageTimePosition, bytesRead );

        }

        public int getInt( int objectId ) {
            return getInt( objectId, buffer, 0, bytesRead );
        }

        public String getString( int objectId ) {
            return getString( objectId, buffer, 0, bytesRead );
        }

        public DateTime getTime( int objectId ) {
            return getTime( objectId, buffer, 0, bytesRead );
        }



        //-- Private ------------------------------------------------------------------------------------------
        private int findValuePosition( int dataPosition, byte[] buffer, int start, int length ) {

            // The first byte after the header gives the length 
            // of the extended header information (variable)
            int headerSize = (int)dataHeader + ( compensateFor09HeaderBug ? 1 : 0 );
            int firstData = headerSize + buffer[ headerSize ] + 1;

            for ( int i = start + firstData; i < length; i++ ) {
                if ( dataPosition-- == 0 )
                    return i;
                else if ( buffer[ i ] == 0x0A ) // OBIS code value
                    i += buffer[ i + 1 ] + 1;
                else if ( buffer[ i ] == 0x09 ) // string value
                    i += buffer[ i + 1 ] + 1;
                else if ( buffer[ i ] == 0x02 ) // byte value (1 byte)
                    i += 1;
                else if ( buffer[ i ] == 0x12 ) // integer value (2 bytes)
                    i += 2;
                else if ( buffer[ i ] == 0x06 ) // integer value (4 bytes)
                    i += 4;
                else {
                    if ( debug ) {
                        log.DebugFormat( "Unknown data type found: 0x{0}", buffer[ i ].ToString( "X" ) );
                    }
                    return 0; // unknown data type found
                }
            }

            if ( debug ) {
                log.DebugFormat(  "Passed the end of the data. Length was: {0}", length );
            }

            return 0;

        }

        private DateTime getTime( int dataPosition, byte[] buffer, int start, int length ) {

            // TODO: check if the time is represented always as a 12 byte string (0x09 0x0C)
            int timeStart = findValuePosition( dataPosition, buffer, start, length );
            timeStart += 1;
            return getTime( buffer, start + timeStart, length - timeStart );

        }
        private DateTime getTime( byte[] buffer, int start, int length ) {

            int pos = start;
            int dataLength = buffer[ pos++ ];

            if ( dataLength == 0x0C ) {
                int year = buffer[ pos ] << 8 |
                    buffer[ pos + 1 ];

                int month = buffer[ pos + 2 ];
                int day = buffer[ pos + 3 ];
                int hour = buffer[ pos + 5 ];
                int minute = buffer[ pos + 6 ];
                int second = buffer[ pos + 7 ];

                return toUnixTime( year, month, day, hour, minute, second );
            } else {
                // Date format not supported
                return DateTime.MinValue;
            }

        }
        private int getInt( int dataPosition, byte[] buffer, int start, int length ) {

            int valuePosition = findValuePosition( dataPosition, buffer, start, length );

            if ( valuePosition > 0 ) {
                int value = 0;
                int bytes = 0;
                switch ( buffer[ valuePosition++ ] ) {
                    case 0x12:
                        bytes = 2;
                        break;
                    case 0x06:
                        bytes = 4;
                        break;
                    case 0x02:
                        bytes = 1;
                        break;
                }

                for ( int i = valuePosition; i < valuePosition + bytes; i++ ) {
                    value = value << 8 | buffer[ i ];
                }

                return value;
            }
            return 0;

        }
        private String getString( int dataPosition, byte[] buffer, int start, int length ) {

            string retVal = String.Empty;
            int valuePosition = findValuePosition( dataPosition, buffer, start, length );

            if ( valuePosition > 0 ) {
                String value = String.Empty;
                for ( int i = valuePosition + 2; i < valuePosition + buffer[ valuePosition + 1 ] + 2; i++ ) {
                    value += ((char)buffer[ i ]).ToString() ;
                }
                retVal = value;
            }
            return retVal;

        }

        private DateTime toUnixTime( int year, int month, int day, int hour, int minute, int second ) {

            DateTime result = new DateTime( year, month, day, hour, minute, second );

            return result;

            //byte daysInMonth[] = new byte[]() { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
            //long secondsPerMinute = 60;
            //long secondsPerHour = secondsPerMinute * 60;
            //long secondsPerDay = secondsPerHour * 24;

            //long time = ( year - 1970 ) * secondsPerDay * 365L;

            //for ( int yearCounter = 1970; yearCounter < year; yearCounter++ )
            //    if ( ( yearCounter % 4 == 0 ) && ( ( yearCounter % 100 != 0 ) || ( yearCounter % 400 == 0 ) ) )
            //        time += secondsPerDay;

            //if ( month > 2 && ( year % 4 == 0 ) && ( ( year % 100 != 0 ) || ( year % 400 == 0 ) ) )
            //    time += secondsPerDay;

            //for ( int monthCounter = 1; monthCounter < month; monthCounter++ )
            //    time += daysInMonth[ monthCounter - 1 ] * secondsPerDay;

            //time += ( day - 1 ) * secondsPerDay;
            //time += hour * secondsPerHour;
            //time += minute * secondsPerMinute;
            //time += second;

        }

        private void debugPrint( byte[] buffer, int start, int length) {

            StringBuilder buf = new StringBuilder();
            buf.AppendLine( "" );

            for ( int i = start; i < start + length; i++ ) {
                if ( buffer[ i ] < 0x10 )
                   buf.Append( "0" );
                buf.Append( buffer[ i ].ToString("X") );
                buf.Append( " " );
                if ( ( i - start + 1 ) % 16 == 0 )
                    buf.AppendLine( "" );
                else if ( ( i - start + 1 ) % 4 == 0 )
                    buf.Append( " " );

            }
            buf.AppendLine( "" );

            log.Debug( buf.ToString() );
        }

    }
}

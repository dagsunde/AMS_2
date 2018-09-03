using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;

namespace AMS_2 {
    class Program {

        private static readonly ILog log = LogManager.GetLogger( typeof( Program ) );

        static void Main( string[] args ) {

            log4net.Config.XmlConfigurator.Configure();
            System.IO.Ports.SerialPort p = new System.IO.Ports.SerialPort();

            HanReader han = new HanReader();
            han.setup( p, "COM3", 2400 );

            while ( true ) {

                // Read one byte from the port, and see if we got a full package
                if ( han.read() ) {

                    // Get the list identifier
                    int listSize = han.getListSize();

                    log.Debug( "" );
                    log.Debug( "List size: " );
                    log.Debug( listSize );
                    log.Debug( ": " );

                    // Only care for the ACtive Power Imported, which is found in the first list
                    if ( listSize == (int)Kamstrup.KamstrupLists.List1 ) {

                        Console.ForegroundColor = ConsoleColor.White;
                        Power pwr = new Power( han );
                        pwr.Save();
                        Console.WriteLine( pwr.ToString() );


                    } else if ( listSize == (int)Kamstrup.KamstrupLists.List2 ) {

                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Energy energy = new Energy( han );
                        energy.Save();
                        Console.WriteLine( energy.ToString() );
                    }


                }
            }
        }
    }
}


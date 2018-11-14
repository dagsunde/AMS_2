using System;
using log4net;

namespace AMS_2 {
    class Program {

        private static readonly ILog log = LogManager.GetLogger( typeof( Program ) );

        static void Main(string[] args) {

            DayAheadPrice prices = new DayAheadPrice();
            prices.GetDayAhead();


            log4net.Config.XmlConfigurator.Configure();
            System.IO.Ports.SerialPort p = new System.IO.Ports.SerialPort();

            var persist = true;
            var comPort = "COM3";

            if (args.Length < 2) {
                Console.WriteLine("Usage: ");
                Console.WriteLine("\t AMS_2 [COMX] [PERSIST | NOPERSIST]");
                Console.WriteLine();
                Console.WriteLine("Using Persist without the DB installed *will* fail!");
            } else {

                if (!args[1].Trim().ToUpper().Equals("PERSIST")) {
                    persist = false;
                }

                comPort = args[0].Trim().ToUpper();

                HanReader han = new HanReader();
                han.setup(p, comPort, 2400);

                while (true) {

                    // Read one byte from the port, and see if we got a full package
                    if (han.read()) {

                        // Get the list identifier
                        int listSize = han.getListSize();

                        log.Debug("");
                        log.Debug("List size: ");
                        log.Debug(listSize);
                        log.Debug(": ");

                        // Only care for the ACtive Power Imported, which is found in the first list
                        if (listSize == (int)Kamstrup.KamstrupLists.List1) {

                            Console.ForegroundColor = ConsoleColor.White;
                            Power pwr = new Power(han);
                            if (persist) {
                                pwr.Save();
                            }
                            Console.WriteLine(pwr.ToString());


                        } else if (listSize == (int)Kamstrup.KamstrupLists.List2) {

                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Energy energy = new Energy(han);
                            if (persist) {
                                energy.Save();
                            }
                            Console.WriteLine(energy.ToString());
                        }


                    }
                }
            }
        }
    }
}


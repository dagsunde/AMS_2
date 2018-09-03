using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMS_2 {
    public class Kamstrup {

        public enum KamstrupLists {
            List1 = 0x19,
            List2 = 0x23
        }

        public enum Kamstrup_List1 {
            ListSize,
            ListVersionIdentifier,
            MeterID_OBIS,
            MeterID,
            MeterType_OBIS,
            MeterType,
            ActiveImportPower_OBIS,
            ActiveImportPower,
            ActiveExportPower_OBIS,
            ActiveExportPower,
            ReactiveImportPower_OBIS,
            ReactiveImportPower,
            ReactiveExportPower_OBIS,
            ReactiveExportPower,
            CurrentL1_OBIS,
            CurrentL1,
            CurrentL2_OBIS,
            CurrentL2,
            CurrentL3_OBIS,
            CurrentL3,
            VoltageL1_OBIS,
            VoltageL1,
            VoltageL2_OBIS,
            VoltageL2,
            VoltageL3_OBIS,
            VoltageL3
        }


        public enum Kamstrup_List2 {
            ListSize,
            ListVersionIdentifier,
            MeterID_OBIS,
            MeterID,
            MeterType_OBIS,
            MeterType,
            ActiveImportPower_OBIS,
            ActiveImportPower,
            ActiveExportPower_OBIS,
            ActiveExportPower,
            ReactiveImportPower_OBIS,
            ReactiveImportPower,
            ReactiveExportPower_OBIS,
            ReactiveExportPower,
            CurrentL1_OBIS,
            CurrentL1,
            CurrentL2_OBIS,
            CurrentL2,
            CurrentL3_OBIS,
            CurrentL3,
            VoltageL1_OBIS,
            VoltageL1,
            VoltageL2_OBIS,
            VoltageL2,
            VoltageL3_OBIS,
            VoltageL3,
            MeterClock_OBIS,
            MeterClock,
            CumulativeActiveImportEnergy_OBIS,
            CumulativeActiveImportEnergy,
            CumulativeActiveExportEnergy_OBIS,
            CumulativeActiveExportEnergy,
            CumulativeReactiveImportEnergy_OBIS,
            CumulativeReactiveImportEnergy,
            CumulativeReactiveExportEnergy_OBIS,
            CumulativeReactiveExportEnergy
        }
    }
}

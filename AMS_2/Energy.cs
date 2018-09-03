using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data.SqlClient;
using Dapper;

namespace AMS_2 {
    public class Energy : Power {

        public Decimal CumulativeActiveImportEnergy { get; set; }
        public Decimal CumulativeActiveExportEnergy { get; set; }
        public Decimal CumulativeReactiveImportEnergy { get; set; }
        public Decimal CumulativeReactiveExportEnergy { get; set; }

        public Energy( HanReader han ) {
            PackageTime = han.getTime( (int)Kamstrup.Kamstrup_List2.MeterClock );
            MeterID = han.getString( (int)Kamstrup.Kamstrup_List2.MeterID );
            MeterType = han.getString( (int)Kamstrup.Kamstrup_List2.MeterType );
            ActiveImportPower = han.getInt( (int)Kamstrup.Kamstrup_List2.ActiveImportPower );
            ActiveExportPower = han.getInt( (int)Kamstrup.Kamstrup_List2.ActiveExportPower );
            ReactiveImportPower = han.getInt( (int)Kamstrup.Kamstrup_List2.ReactiveImportPower );
            ReactiveExportPower = han.getInt( (int)Kamstrup.Kamstrup_List2.ReactiveExportPower );

            CurrentL1 = han.getInt( (int)Kamstrup.Kamstrup_List2.CurrentL1 ) / 100.0M;
            CurrentL2 = han.getInt( (int)Kamstrup.Kamstrup_List2.CurrentL2 ) / 100.0M;
            CurrentL3 = han.getInt( (int)Kamstrup.Kamstrup_List2.CurrentL3 ) / 100.0M;

            VoltageL1 = han.getInt( (int)Kamstrup.Kamstrup_List2.VoltageL1 );
            VoltageL2 = han.getInt( (int)Kamstrup.Kamstrup_List2.VoltageL2 );
            VoltageL3 = han.getInt( (int)Kamstrup.Kamstrup_List2.VoltageL3 );

            CumulativeActiveImportEnergy = han.getInt( (int)Kamstrup.Kamstrup_List2.CumulativeActiveImportEnergy ) / 100.0M; ;
            CumulativeActiveExportEnergy = han.getInt( (int)Kamstrup.Kamstrup_List2.CumulativeActiveExportEnergy ) / 100.0M; ;

            CumulativeReactiveImportEnergy = han.getInt( (int)Kamstrup.Kamstrup_List2.CumulativeReactiveImportEnergy ) / 100.0M;
            CumulativeReactiveExportEnergy = han.getInt( (int)Kamstrup.Kamstrup_List2.CumulativeReactiveExportEnergy ) / 100.0M;
        }

        public override void Save() {

            base.Save();

            var connStr = ConfigurationManager.ConnectionStrings[ "ConnString" ].ConnectionString;

            using ( var cn = new SqlConnection( connStr ) ) {
                cn.Open();

                var sql = @"INSERT INTO [EnergyConsuption]( [PackageTime], [MeterID], [CumulativeActiveImportEnergy], [CumulativeActiveExportEnergy], [CumulativeReactiveImportEnergy], [CumulativeReactiveExportEnergy])
                                  VALUES ( @PackageTime, @MeterID, @CumulativeActiveImportEnergy, @CumulativeActiveExportEnergy, @CumulativeReactiveImportEnergy, @CumulativeReactiveExportEnergy)";

                cn.Execute( sql, this );

            }

        }
        public override string ToString() {
            StringBuilder buf = new StringBuilder();
            buf.AppendLine();

            string d = PackageTime.ToString( "yyyy-MM-dd HH:mm:ss" );
            buf.AppendLine( $"PackageTime: {PackageTime.ToString()}" );

            buf.AppendLine( $"MeterID: {MeterID}" );
            buf.AppendLine( $"MeterType: {MeterType}" );

            buf.AppendLine( $"ActiveImportPower: {ActiveImportPower}" );
            buf.AppendLine( $"ActiveExportPower: {ActiveExportPower}" );
            buf.AppendLine( $"ReactiveImportPower: {ReactiveImportPower}" );
            buf.AppendLine( $"ReactiveExportPower: {ReactiveExportPower}" );

            string c = CurrentL1.ToString( "0.00" );
            buf.AppendLine( $@"CurrentL1: {c}" );
            c = CurrentL2.ToString( "0.00" );
            buf.AppendLine( $@"CurrentL2: {c}" );
            c = CurrentL3.ToString( "0.00" );
            buf.AppendLine( $@"CurrentL3: {c}" );

            c = VoltageL1.ToString( "0.00" );
            buf.AppendLine( $@"VoltageL1: {c}" );
            c = VoltageL2.ToString( "0.00" );
            buf.AppendLine( $@"VoltageL2: {c}" );
            c = VoltageL3.ToString( "0.00" );
            buf.AppendLine( $@"VoltageL3: {c}" );

            c = CumulativeActiveImportEnergy.ToString( "0.00" );
            buf.AppendLine( $@"CumulativeActiveImportEnergy: {c}" );
            c = CumulativeActiveExportEnergy.ToString( "0.00" );
            buf.AppendLine( $@"CumulativeActiveExportEnergy: {c}" );

            c = CumulativeReactiveImportEnergy.ToString( "0.00" );
            buf.AppendLine( $@"CumulativeReactiveImportEnergy: {c}" );
            c = CumulativeReactiveExportEnergy.ToString( "0.00" );
            buf.AppendLine( $@"CumulativeReactiveExportEnergy: {c}" );

            return buf.ToString();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

using Dapper;
using System.Configuration;

namespace AMS_2 {
    public class Power {

        public Power() {

        }

        public Power(HanReader han) {
            PackageTime = han.getPackageTime();
            MeterID = han.getString( (int)Kamstrup.Kamstrup_List1.MeterID );
            MeterType = han.getString( (int)Kamstrup.Kamstrup_List1.MeterType );
            ActiveImportPower = han.getInt( (int)Kamstrup.Kamstrup_List1.ActiveImportPower );
            ActiveExportPower = han.getInt( (int)Kamstrup.Kamstrup_List1.ActiveExportPower );
            ReactiveImportPower = han.getInt( (int)Kamstrup.Kamstrup_List1.ReactiveImportPower );
            ReactiveExportPower = han.getInt( (int)Kamstrup.Kamstrup_List1.ReactiveExportPower );

            CurrentL1 = han.getInt( (int)Kamstrup.Kamstrup_List1.CurrentL1 ) / 100.0M;
            CurrentL2 = han.getInt( (int)Kamstrup.Kamstrup_List1.CurrentL2 ) / 100.0M;
            CurrentL3 = han.getInt( (int)Kamstrup.Kamstrup_List1.CurrentL3 ) / 100.0M;

            VoltageL1 = han.getInt( (int)Kamstrup.Kamstrup_List1.VoltageL1 );
            VoltageL2 = han.getInt( (int)Kamstrup.Kamstrup_List1.VoltageL2 );
            VoltageL3 = han.getInt( (int)Kamstrup.Kamstrup_List1.VoltageL3 );
        }

        public virtual void Save() {

            var connStr = ConfigurationManager.ConnectionStrings[ "ConnString" ].ConnectionString;

            using ( var cn = new SqlConnection(connStr)) {
                cn.Open();

                var sql = @"INSERT INTO [PowerConsumption]( [PackageTime] ,[MeterID] ,[MeterType] ,[ActiveImportPower] ,[ActiveExportPower] ,[ReactiveImportPower] ,[ReactiveExportPower] ,[CurrentL1] ,[CurrentL2] ,[CurrentL3] ,[VoltageL1] ,[VoltageL2] ,[VoltageL3] )
                                  VALUES ( @PackageTime, @MeterID, @MeterType, @ActiveImportPower, @ActiveExportPower, @ReactiveImportPower, @ReactiveExportPower, @CurrentL1, @CurrentL2, @CurrentL3, @VoltageL1, @VoltageL2, @VoltageL3 )";

                cn.Execute( sql, this );

            }
        }

        public DateTime PackageTime { get; set; }
        public string MeterID { get; set; }
        public string MeterType { get; set; }
        public int ActiveImportPower { get; set; }
        public int ActiveExportPower { get; set; }
        public int ReactiveImportPower { get; set; }
        public int ReactiveExportPower { get; set; }
        public Decimal CurrentL1 { get; set; }
        public Decimal CurrentL2 { get; set; }
        public Decimal CurrentL3 { get; set; }
        public Decimal VoltageL1 { get; set; }
        public Decimal VoltageL2 { get; set; }
        public Decimal VoltageL3 { get; set; }

        public override string ToString() {
            StringBuilder buf = new StringBuilder();
            buf.AppendLine();

            string d = PackageTime.ToString("yyyy -MM-dd HH:mm:ss");
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

            return buf.ToString();
        }
    }
}

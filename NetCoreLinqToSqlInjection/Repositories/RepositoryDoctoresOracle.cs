using NetCoreLinqToSqlInjection.Models;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Diagnostics;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Numerics;

namespace NetCoreLinqToSqlInjection.Repositories
{
    #region Stored Procedures
    //create procedure SP_DELETE_DOCTOR
    //(@iddoctor int)
    //as
    //	delete from DOCTOR where DOCTOR_NO=@iddoctor
    //go
    #endregion

    public class RepositoryDoctoresOracle : IRepositoryDoctores
    {
        private DataTable tableDoctores;
        private OracleConnection cn;
        private OracleCommand com;
        public RepositoryDoctoresOracle()
        {
            string connectionString = @"Data Source=LOCALHOST:1521/XE; Persist Security Info=True; User Id = SYSTEM; Password = oracle";
            this.tableDoctores = new DataTable();
            this.cn = new OracleConnection(connectionString);
            this.com = new OracleCommand(connectionString);
            this.com.Connection = this.cn;
            OracleDataAdapter ad = new OracleDataAdapter("select * from DOCTOR", connectionString);
            ad.Fill(this.tableDoctores);
        }

        public List<Doctor> GetDoctores()
        {
            var consulta = from datos in this.tableDoctores.AsEnumerable()
                           select datos;
            List<Doctor> doctores = new List<Doctor>();
            foreach (var row in consulta)
            {
                Doctor doc = new Doctor();
                doc.IdDoctor = row.Field<int>("DOCTOR_NO");
                doc.Apellido = row.Field<string>("APELLIDO");
                doc.Especialidad = row.Field<string>("ESPECIALIDAD");
                doc.Salario = row.Field<int>("SALARIO");
                doc.IdHospital = row.Field<int>("HOSPITAL_COD");
                doctores.Add(doc);
            }
            return doctores;
        }

        public void InsertDoctor (int idDoctor, string apellido, string especialidad, int salario, int idHospital)
        {
            string sql = "insert into DOCTOR values (:idhospital, :iddoctor, :apellido, :especialidad, :salario)";
            OracleParameter pamIdHospital = new OracleParameter(":idhospital", idHospital);
            this.com.Parameters.Add(pamIdHospital);
            OracleParameter pamIdDoctor  = new OracleParameter(":iddoctor", idDoctor);
            this.com.Parameters.Add(pamIdDoctor);
            OracleParameter pamApellido = new OracleParameter(":apellido", apellido);
            this.com.Parameters.Add(pamApellido);
            OracleParameter pamEspecialidad = new OracleParameter(":especialidad", especialidad);
            this.com.Parameters.Add(pamEspecialidad);
            OracleParameter pamSalario = new OracleParameter(":salario", salario);
            this.com.Parameters.Add(pamSalario);



            this.com.CommandType = CommandType.Text;
            this.com.CommandText = sql;
            this.cn.Open();
            this.com.ExecuteNonQuery();
            this.cn.Close();
            this.com.Parameters.Clear();
        }


        public void DeleteDoctor(int idDoctor)
        {
            string sql = "SP_DELETE_DOCTOR";
            OracleParameter pamIdDoctor = new OracleParameter(":iddoctor", idDoctor);
            this.com.Parameters.Add(pamIdDoctor);
            this.com.CommandType = CommandType.StoredProcedure;
            this.com.CommandText = sql;
            this.cn.Open();
            this.com.ExecuteNonQuery();
            this.cn.Close();
            this.com.Parameters.Clear();
        }
    }
}

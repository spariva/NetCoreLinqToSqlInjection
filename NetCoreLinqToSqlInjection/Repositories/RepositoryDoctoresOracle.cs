using NetCoreLinqToSqlInjection.Models;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Diagnostics;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Numerics;
using System;

namespace NetCoreLinqToSqlInjection.Repositories
{
    #region Stored Procedures
//    CREATE OR REPLACE PROCEDURE SP_DELETE_DOCTOR
//    (p_iddoctor DOCTOR.DOCTOR_NO%TYPE)
//AS
//BEGIN
//  DELETE FROM DOCTOR WHERE DOCTOR_NO=p_iddoctor;
//  COMMIT;
//END;
//    create or replace procedure SP_UPDATE_DOCTOR
//    (p_iddoctor DOCTOR.DOCTOR_NO%TYPE, p_apellido DOCTOR.APELLIDO%TYPE, p_especialidad DOCTOR.ESPECIALIDAD%TYPE,
//    p_salario DOCTOR.SALARIO%TYPE, p_idhospital DOCTOR.HOSPITAL_COD%TYPE)
//as begin
//       update DOCTOR set HOSPITAL_COD=p_idhospital, DOCTOR_NO=p_iddoctor, APELLIDO=p_apellido, ESPECIALIDAD=p_especialidad, SALARIO=p_salario
//       where DOCTOR_NO = p_iddoctor;
//       commit;
//end;
    #endregion

    public class RepositoryDoctoresOracle : IRepositoryDoctores
    {
        private DataTable tablaDoctores;
        private OracleConnection cn;
        private OracleCommand com;
        public RepositoryDoctoresOracle()
        {
            string connectionString = @"Data Source=LOCALHOST:1521/XE; Persist Security Info=True; User Id = SYSTEM; Password = oracle";
            this.tablaDoctores = new DataTable();
            this.cn = new OracleConnection(connectionString);
            this.com = new OracleCommand(connectionString);
            this.com.Connection = this.cn;
            OracleDataAdapter ad = new OracleDataAdapter("select * from DOCTOR", connectionString);
            ad.Fill(this.tablaDoctores);
        }

        public List<Doctor> GetDoctores()
        {
            var consulta = from datos in this.tablaDoctores.AsEnumerable()
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

        public Doctor GetDoctor(int iddoctor)
        {
            var consulta = from datos in this.tablaDoctores.AsEnumerable()
                           where datos.Field<int>("DOCTOR_NO") == iddoctor
                           select datos;
            DataRow row = consulta.First();
            Doctor doc = new Doctor()
            {
                IdDoctor = row.Field<int>("DOCTOR_NO"),
                Apellido = row.Field<string>("APELLIDO"),
                Especialidad = row.Field<string>("ESPECIALIDAD"),
                Salario = row.Field<int>("SALARIO"),
                IdHospital = row.Field<int>("HOSPITAL_COD")
            };

            return doc;
        }

        public void UpdateDoctor(int idDoctor, string apellido, string especialidad, int salario, int idHospital)
        {
            string sql = "SP_UPDATE_DOCTOR";
            OracleParameter pamIdDoctor = new OracleParameter(":iddoctor", idDoctor);
            this.com.Parameters.Add(pamIdDoctor);
            OracleParameter pamApellido = new OracleParameter(":apellido", apellido);
            this.com.Parameters.Add(pamApellido);
            OracleParameter pamEspecialidad = new OracleParameter(":especialidad", especialidad);
            this.com.Parameters.Add(pamEspecialidad);
            OracleParameter pamSalario = new OracleParameter(":salario", salario);
            this.com.Parameters.Add(pamSalario);
            OracleParameter pamIdHospital = new OracleParameter(":idhospital", idHospital);
            this.com.Parameters.Add(pamIdHospital);

            this.com.CommandType = CommandType.StoredProcedure;
            this.com.CommandText = sql;
            this.cn.Open();
            this.com.ExecuteNonQuery();
            this.cn.Close();
            this.com.Parameters.Clear();
        }

        public List<Doctor> GetDoctoresEspecialidad(string especialidad)
        {
            var consulta = from datos in this.tablaDoctores.AsEnumerable()
                           where datos.Field<string>("ESPECIALIDAD") == especialidad
                           select datos;
            consulta.OrderBy(x => x.Field<string>("ESPECIALIDAD") == especialidad);

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

        public List<string> GetEspecialidades()
        {
            var consulta = (from datos in this.tablaDoctores.AsEnumerable()
                           select datos.Field<string>("ESPECIALIDAD")).Distinct();
            List<string> especialidades = new List<string>();
            foreach (var esp in consulta)
            {
                especialidades.Add(esp);
            }
            return especialidades;
        }
    }
}

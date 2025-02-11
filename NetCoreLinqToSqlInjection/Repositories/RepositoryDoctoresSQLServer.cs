using System.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Numerics;
using Microsoft.Data.SqlClient;
using NetCoreLinqToSqlInjection.Models;

namespace NetCoreLinqToSqlInjection.Repositories
{
    #region Stored Procedures
    //      create procedure SP_DELETE_DOCTOR
    //      (@iddoctor int)
    //      as
    //	        delete from DOCTOR where DOCTOR_NO=@iddoctor
    //      go
    //    create procedure SP_UPDATE_DOCTOR(@iddoctor int, @apellido nvarchar(50), @especialidad nvarchar(50), @salario int, @idhospital int)
    //as
    //	update DOCTOR set HOSPITAL_COD = @idhospital, DOCTOR_NO = @iddoctor, APELLIDO = @apellido, ESPECIALIDAD = @especialidad, SALARIO = @salario

    //    where DOCTOR_NO = @iddoctor;
    //    go
    #endregion
    public class RepositoryDoctoresSQLServer : IRepositoryDoctores
    {
        private DataTable tablaDoctores;
        private SqlConnection cn;
        private SqlCommand com;

        public RepositoryDoctoresSQLServer()
        {
            string connectionString = @"Data Source=LOCALHOST\DESARROLLO;Initial Catalog=HOSPITAL;Persist Security Info=True;User ID=SA;Encrypt=True;Trust Server Certificate=True";
            this.cn = new SqlConnection(connectionString);
            this.com = new SqlCommand();
            this.com.Connection = this.cn;
            this.tablaDoctores = new DataTable();
            SqlDataAdapter ad = new SqlDataAdapter
                ("select * from DOCTOR", connectionString);
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

        public void InsertDoctor
            (int idDoctor, string apellido, string especialidad
            , int salario, int idHospital)
        {
            string sql = "insert into DOCTOR values (@idhospital, @iddoctor "
                + ", @apellido, @especialidad, @salario)";
            this.com.Parameters.AddWithValue("@iddoctor", idDoctor);
            this.com.Parameters.AddWithValue("@apellido", apellido);
            this.com.Parameters.AddWithValue("@especialidad", especialidad);
            this.com.Parameters.AddWithValue("@salario", salario);
            this.com.Parameters.AddWithValue("@idhospital", idHospital);
            this.com.CommandType = CommandType.Text;
            this.com.CommandText = sql;
            this.cn.Open();
            this.com.ExecuteNonQuery();
            this.cn.Close();
            this.com.Parameters.Clear();
        }

        public void UpdateDoctor (int idDoctor, string apellido, string especialidad, int salario, int idHospital)
        {
            string sql = "SP_UPDATE_DOCTOR";
            this.com.Parameters.AddWithValue("@iddoctor", idDoctor);
            this.com.Parameters.AddWithValue("@apellido", apellido);
            this.com.Parameters.AddWithValue("@especialidad", especialidad);
            this.com.Parameters.AddWithValue("@salario", salario);
            this.com.Parameters.AddWithValue("@idhospital", idHospital);
            this.com.CommandType = CommandType.StoredProcedure;
            this.com.CommandText = sql;
            this.cn.Open();
            this.com.ExecuteNonQuery();
            this.cn.Close();
            this.com.Parameters.Clear();
        }

        public void DeleteDoctor(int idDoctor)
        {
            string sql = "SP_DELETE_DOCTOR";
            this.com.Parameters.AddWithValue("@iddoctor", idDoctor);
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

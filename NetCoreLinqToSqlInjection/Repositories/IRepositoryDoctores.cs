using NetCoreLinqToSqlInjection.Models;

namespace NetCoreLinqToSqlInjection.Repositories
{
    public interface IRepositoryDoctores
    {
        List<Doctor> GetDoctores();

        List<Doctor> GetDoctoresEspecialidad(string especialidad);

        List<string> GetEspecialidades();

        Doctor GetDoctor(int idDoctor);

        void InsertDoctor(int idDoctor, string apellido, string especialidad
            , int salario, int idHospital);

        void UpdateDoctor(int idDoctor, string apellido, string especialidad
            , int salario, int idHospital);
        void DeleteDoctor(int idDoctor);

    }
}

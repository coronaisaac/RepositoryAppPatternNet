using AppDaltonCatalogo.Infrastructure.SQL.Helpers;
using DaltORM;

namespace AppDaltonCatalogo.Infrastructure.SQL.Repositories
{
    public class AuthRepositories : IAuth
    {
        private readonly Database database;

        public AuthRepositories(Database database)
        {
            this.database = database;
        }

        public Response<string> Login(LoginDto command)
        {
            try
            {
                if (string.IsNullOrEmpty(command.NameSocial)) command.Psw = EncryptHelpers.GetSHA256(command.Psw);

                var mpp = MappingInjection
                   .Injecction
                   .AccessMapProfile
                   .Map<strAuthLogin>(command);

                var resultExec = mpp.Execute();


                return Response<string>
                        .FromSuccess(resultExec.FirstOrDefault());
            }
            catch (Exception e)
            {
                return Response<string>
                    .FromError(e.Message);
            }
        }

    
       
       
    }
}

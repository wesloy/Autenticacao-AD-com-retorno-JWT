using Novell.Directory.Ldap;
using SI_MicroServicos.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SI_MicroServicos.Services
{
    public class UserServices
    {
        public UserProfile GetAndValidate(UserAuth userIn)
        {
            var dbContext = new _DbContext();
            var valitaded = LoginLdap(userIn.User, userIn.Password);
            if (valitaded == false)
            {
                return null;
            }
            var userX = dbContext.Users.Where(p => p.Username == userIn.User).FirstOrDefault();
            if (userX != null)
            {
                //Update
                userX.LastSuccessfulLogin = DateTime.Now;
                dbContext.Users.Update(userX);

                dbContext.LogAuditorias.Add(
                    new LogAuditoria{
                    User = userIn.User,
                    DetalhesAuditoria = string.Concat("Efetuou Login no sistema: ", userIn.Sistema,"Data do login : ", DateTime.Now)

                });
            }
            else {
                userX = new UserProfile() {
                    AuthType = UserAuthType.ActiveDirectory,
                    LastSuccessfulLogin = DateTime.Now,
                    Password = "Active Directory Auth",
                    Type = UserType.NormalUser,
                    Username = userIn.User,
                    Email = userIn.User + "@dominio.com",
                    Sistema = userIn.Sistema
                };
                dbContext.Users.Add(userX);

                dbContext.LogAuditorias.Add(
                    new LogAuditoria{
                    User = userIn.User,
                    DetalhesAuditoria = string.Concat("Efetuou Login no sistema: ", userIn.Sistema,"Data do login : ", DateTime.Now)

                });
            }
            dbContext.SaveChangesAsync();
            return userX;          
        }

        private static bool LoginLdap(string username, string password)
        {
            try
            {
                using (var conn = new LdapConnection())
                {
                    conn.Connect("dominio.com.br", 389);
                    conn.Bind(LdapConnection.LdapV3, $"DOMINIO\\{username}", password);
                }
                return true;
            }
            catch (LdapException)
            {
                return false;
            }
        }
    }
}

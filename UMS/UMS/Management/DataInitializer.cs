using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UMS.Models;
using System.Data.Entity;

namespace UMS.Management
{
    public class DataInitializer:DropCreateDatabaseIfModelChanges<UMSDBEntities>
    {
        protected override void Seed(UMSDBEntities context)
        {
            var users = new List<UserInfo>
            {
                new UserInfo{ UserName = "rcruz7", FirstName="Ricardo", LastName="Cruz",
                              Email ="rcruzmorales@csc.com",Address = "Calle 4 #32",City="Isabela",Country="Jobo",
                              Zipcode = "00778",PhoneNumber="787-579-2355",Status_Id =1 ,Role_Id = 1,Picture_Id = 1
                },

                new UserInfo { UserName = "fayala", FirstName = "Felix Alfonso", LastName = "Ayala",
                               Email = "fayala@gmail.com", Address = "Calle 5 #32", City = "Gurabo", Country = "Mamey",
                               Zipcode = "00662", PhoneNumber = "787-233-7092", Status_Id = 1, Role_Id = 2, Picture_Id = 1
                }

             };

            users.ForEach(u => context.UserInfoes.Add(u));
            context.SaveChanges();

            var credentials = new List<UserCredential>
            {
                new UserCredential{Password = "123"},
                new UserCredential{Password = "345"}
            };

            credentials.ForEach(c => context.UserCredentials.Add(c));
            context.SaveChanges();


        }
    }
}
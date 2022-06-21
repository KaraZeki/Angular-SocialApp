using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using ServerApp.Models;

namespace ServerApp.Data
{
     public static class SeedDatabase
    {
        public static async Task Seed(UserManager<User> userManager)
        {
            if(!userManager.Users.Any())
            {
                var users = File.ReadAllText("Data/user.json");
                var listOfUsers = JsonConvert.DeserializeObject<List<User>>(users);

                foreach (var user in listOfUsers)
                {
                    await userManager.CreateAsync(user, "SocialApp_123");
                }                
            }
        }
    }
}
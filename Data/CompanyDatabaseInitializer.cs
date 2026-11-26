using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using Newtonsoft.Json;
using VizProgLaba1._1.Model;

namespace VizProgLaba1._1.Data
{
    /// <summary>
    /// Создаёт базу и переносит исходные данные из JSON только один раз.
    /// </summary>
    public class CompanyDatabaseInitializer : CreateDatabaseIfNotExists<CompanyEntities>
    {
        protected override void Seed(CompanyEntities context)
        {
            string dataDirectory = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "..",
                "..",
                "DataModels");
            dataDirectory = Path.GetFullPath(dataDirectory);
            string rolesPath = Path.Combine(dataDirectory, "RoleData.json");
            string personsPath = Path.Combine(dataDirectory, "PersonData.json");

            if (!File.Exists(rolesPath) || !File.Exists(personsPath))
            {
                return;
            }

            var seedRoles = JsonConvert.DeserializeObject<List<RoleSeed>>(
                File.ReadAllText(rolesPath)) ?? new List<RoleSeed>();
            var seedPersons = JsonConvert.DeserializeObject<List<PersonSeed>>(
                File.ReadAllText(personsPath)) ?? new List<PersonSeed>();

            var rolesBySourceId = new Dictionary<int, Role>();
            foreach (RoleSeed seedRole in seedRoles)
            {
                var role = new Role
                {
                    NameRole = seedRole.NameRole
                };
                context.Roles.Add(role);
                rolesBySourceId[seedRole.Id] = role;
            }

            context.SaveChanges();

            foreach (PersonSeed seedPerson in seedPersons)
            {
                Role role;
                if (!rolesBySourceId.TryGetValue(seedPerson.RoleId, out role))
                {
                    continue;
                }

                context.Persons.Add(new Person
                {
                    FirstName = seedPerson.FirstName,
                    LastName = seedPerson.LastName,
                    Birthday = ParseBirthday(seedPerson.Birthday),
                    RoleId = role.Id,
                    Role = role
                });
            }

            context.SaveChanges();
            base.Seed(context);
        }

        private static DateTime ParseBirthday(string value)
        {
            return DateTime.ParseExact(
                value,
                "dd.MM.yyyy",
                CultureInfo.GetCultureInfo("ru-RU"));
        }

        private sealed class RoleSeed
        {
            public int Id { get; set; }
            public string NameRole { get; set; }
        }

        private sealed class PersonSeed
        {
            public int Id { get; set; }
            public int RoleId { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Birthday { get; set; }
        }
    }
}

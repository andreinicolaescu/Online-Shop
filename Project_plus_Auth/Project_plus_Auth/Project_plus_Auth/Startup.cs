using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Owin;
using Project_plus_Auth.Models;

[assembly: OwinStartupAttribute(typeof(Project_plus_Auth.Startup))]
namespace Project_plus_Auth
{
    public partial class Startup
    {
        private void createAdminUserAndApplicationRoles()
        {
            ApplicationDbContext context = new ApplicationDbContext();

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            // Se adauga rolurile aplicatiei             
            if (!roleManager.RoleExists("Administrator"))
            {                 
                // Se adauga rolul de administrator                 
                var role = new IdentityRole();
                role.Name = "Administrator";
                roleManager.Create(role);

                // se adauga utilizatorul administrator                 
                var user = new ApplicationUser();
                user.UserName = "admin@admin.com";
                user.Email = "admin@admin.com"; 

                var adminCreated = UserManager.Create(user, "Administrator1!");
                if (adminCreated.Succeeded)
                {
                    UserManager.AddToRole(user.Id, "Administrator");
                }
            } 
            if (!roleManager.RoleExists("Editor"))
            {
                var role = new IdentityRole(); role.Name = "Editor";
                roleManager.Create(role);

                // se adauga utilizatorul editor 
                /*
                var user = new ApplicationUser();
                user.UserName = "editor@editor.com";
                user.Email = "editor@editor.com";

                var editorCreated = UserManager.Create(user, "Editor1!");
                if (editorCreated.Succeeded)
                {
                    UserManager.AddToRole(user.Id, "Editor");
                }
                */
            } 
 
            if (!roleManager.RoleExists("User"))
            {
                var role = new IdentityRole();
                role.Name = "User";
                roleManager.Create(role);

                /*
                // se adauga utilizatorul user                 
                var user = new ApplicationUser();
                user.UserName = "user@user.com";
                user.Email = "user@user.com";

                var userCreated = UserManager.Create(user, "User1!");
                if (userCreated.Succeeded)
                {
                    UserManager.AddToRole(user.Id, "User");
                }
                */
            } 
 
        } 

        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            createAdminUserAndApplicationRoles();
        }
    }
}

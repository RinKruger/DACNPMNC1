using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TestingProject.App_Start.StartUp))]
namespace TestingProject.App_Start
{
    public partial class StartUp
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
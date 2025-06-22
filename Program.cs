using Microsoft.Extensions.DependencyInjection;

namespace pos_system
{
    internal static class Program
    {
        public static IServiceProvider ServiceProvider { get; private set; }
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            ApplicationConfiguration.Initialize();
            var services = new ServiceCollection();
            services.AddTransient<pos_system.pos.UI.Forms.Auth.LoginForm>();
            ServiceProvider = services.BuildServiceProvider();


            Application.Run(ServiceProvider.GetRequiredService<pos_system.pos.UI.Forms.Auth.LoginForm>());
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
        }

    }
}
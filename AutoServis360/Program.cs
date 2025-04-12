using System;
using System.Windows.Forms;

namespace AutoServis360
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            LoginForm loginForm = new LoginForm();
            if (loginForm.ShowDialog() == DialogResult.OK && loginForm.LoggedInUser != null)
            {
                User user = loginForm.LoggedInUser;
                
                if (user.Position == "Recepce")
                {
                    
                    Application.Run(new ReceptionForm());
                }
                else if (user.Position == "Mechanic")
                {
                    MechanicDAO mechanicDAO = new MechanicDAO();
                    Mechanic mechanic = mechanicDAO.GetMechanicByUserId(user.Id);

                    if (mechanic != null)
                    {
                        Application.Run(new MechanicForm(mechanic));
                    }
                    else
                    {
                        MessageBox.Show("Mechanik nebyl nalezen!", "Chyba", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}

using RealEstateCRMWinForms.Data;
using RealEstateCRMWinForms.Models;
using System.ComponentModel;

namespace RealEstateCRMWinForms.ViewModels
{
    public class UserViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public BindingList<User> Users { get; set; }

        public UserViewModel()
        {
            Users = new BindingList<User>();
        }

        public void LoadUsers()
        {
            Users.Clear();

            try
            {
                using (var dbContext = DbContextHelper.CreateDbContext())
                {
                    var usersFromDb = dbContext.Users.ToList();

                    foreach (var user in usersFromDb)
                    {
                        Users.Add(user);
                    }
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }






    }
}

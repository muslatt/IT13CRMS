using RealEstateCRMWinForms.ViewModels;

namespace RealEstateCRMWinForms
{
    public partial class Form1 : Form
    {
        private readonly UserViewModel _viewModel;

        public Form1()
        {
            InitializeComponent();
            _viewModel = new UserViewModel();
            dataGridView1.DataSource = _viewModel.Users;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _viewModel.LoadUsers();
        }
    }
}

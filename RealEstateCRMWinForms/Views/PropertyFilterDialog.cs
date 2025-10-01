using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace RealEstateCRMWinForms.Views
{
    public class PropertyFilterOptions
    {
        public string? Status { get; set; }
        public string? PropertyType { get; set; }
        public string? TransactionType { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public int? MinBedrooms { get; set; }
        public int? MinBathrooms { get; set; }
    }

    public class PropertyFilterDialog : Form
    {

        private ComboBox cmbType;
        private ComboBox cmbTransaction;
        private NumericUpDown numMinPrice;
        private NumericUpDown numMaxPrice;
        private NumericUpDown numMinBeds;
        private NumericUpDown numMinBaths;
        private Button btnClear;
        private Button btnCancel;
        private Button btnApply;

        private readonly List<string> _types;

        public PropertyFilterOptions? Result { get; private set; }
        public bool Cleared { get; private set; }

        public PropertyFilterDialog(PropertyFilterOptions current, List<string> availableTypes)
        {
            _types = availableTypes ?? new List<string>();
            InitializeComponent();
            LoadInitial(current);
        }

        private void InitializeComponent()
        {
            Text = "Filter Properties";
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Size = new Size(520, 360);
            BackColor = Color.White;
            Font = new Font("Segoe UI", 10F);

            int labelX = 20; int controlX = 180; int width = 300; int y = 20; int spacing = 34;

            Controls.Add(new Label { Text = "Property Type", Location = new Point(labelX, y + 4), AutoSize = true });
            cmbType = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Location = new Point(controlX, y), Width = width };
            cmbType.Items.Add("");
            foreach (var t in _types) cmbType.Items.Add(t);
            Controls.Add(cmbType); y += spacing;

            Controls.Add(new Label { Text = "Transaction", Location = new Point(labelX, y + 4), AutoSize = true });
            cmbTransaction = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Location = new Point(controlX, y), Width = width };
            cmbTransaction.Items.AddRange(new object[] { "", "Buying", "Viewing" });
            Controls.Add(cmbTransaction); y += spacing;

            Controls.Add(new Label { Text = "Min Price", Location = new Point(labelX, y + 4), AutoSize = true });
            numMinPrice = new NumericUpDown { Location = new Point(controlX, y), Width = width, Maximum = 1000000000, DecimalPlaces = 0, ThousandsSeparator = true };
            Controls.Add(numMinPrice); y += spacing;

            Controls.Add(new Label { Text = "Max Price", Location = new Point(labelX, y + 4), AutoSize = true });
            numMaxPrice = new NumericUpDown { Location = new Point(controlX, y), Width = width, Maximum = 1000000000, DecimalPlaces = 0, ThousandsSeparator = true };
            Controls.Add(numMaxPrice); y += spacing;

            Controls.Add(new Label { Text = "Min Bedrooms", Location = new Point(labelX, y + 4), AutoSize = true });
            numMinBeds = new NumericUpDown { Location = new Point(controlX, y), Width = width, Maximum = 50, DecimalPlaces = 0 };
            Controls.Add(numMinBeds); y += spacing;

            Controls.Add(new Label { Text = "Min Bathrooms", Location = new Point(labelX, y + 4), AutoSize = true });
            numMinBaths = new NumericUpDown { Location = new Point(controlX, y), Width = width, Maximum = 50, DecimalPlaces = 0 };
            Controls.Add(numMinBaths); y += spacing + 8;

            btnClear = new Button { Text = "Clear", Location = new Point(20, y), Size = new Size(100, 32), BackColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnClear.FlatAppearance.BorderColor = Color.FromArgb(209, 213, 219);
            btnClear.Click += (s, e) => { Cleared = true; DialogResult = DialogResult.Cancel; Close(); };
            Controls.Add(btnClear);

            btnCancel = new Button { Text = "Cancel", Location = new Point(280, y), Size = new Size(100, 32), BackColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnCancel.FlatAppearance.BorderColor = Color.FromArgb(209, 213, 219);
            btnCancel.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };
            Controls.Add(btnCancel);

            btnApply = new Button { Text = "Apply", Location = new Point(390, y), Size = new Size(100, 32), BackColor = Color.FromArgb(37, 99, 235), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnApply.FlatAppearance.BorderSize = 0;
            btnApply.Click += (s, e) => { Result = Collect(); DialogResult = DialogResult.OK; Close(); };
            Controls.Add(btnApply);
        }

        private void LoadInitial(PropertyFilterOptions current)
        {
            if (current == null) return;
            // Status field removed from filter - previously set here
            cmbType.SelectedItem = string.IsNullOrEmpty(current.PropertyType) ? "" : current.PropertyType;
            cmbTransaction.SelectedItem = current.TransactionType ?? "";
            if (current.MinPrice.HasValue) numMinPrice.Value = Clamp(numMinPrice.Minimum, numMinPrice.Maximum, current.MinPrice.Value);
            if (current.MaxPrice.HasValue) numMaxPrice.Value = Clamp(numMaxPrice.Minimum, numMaxPrice.Maximum, current.MaxPrice.Value);
            if (current.MinBedrooms.HasValue) numMinBeds.Value = Clamp(numMinBeds.Minimum, numMinBeds.Maximum, current.MinBedrooms.Value);
            if (current.MinBathrooms.HasValue) numMinBaths.Value = Clamp(numMinBaths.Minimum, numMinBaths.Maximum, current.MinBathrooms.Value);
        }

        private static decimal Clamp(decimal min, decimal max, decimal value)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }

        private PropertyFilterOptions Collect()
        {
            var opts = new PropertyFilterOptions
            {
                // Status removed from filter options
                PropertyType = NullIfEmpty(cmbType.SelectedItem as string),
                TransactionType = NullIfEmpty(cmbTransaction.SelectedItem as string)
            };

            if (numMinPrice.Value > 0) opts.MinPrice = numMinPrice.Value;
            if (numMaxPrice.Value > 0) opts.MaxPrice = numMaxPrice.Value;
            if (numMinBeds.Value > 0) opts.MinBedrooms = (int)numMinBeds.Value;
            if (numMinBaths.Value > 0) opts.MinBathrooms = (int)numMinBaths.Value;

            return opts;
        }

        private static string? NullIfEmpty(string? s)
            => string.IsNullOrWhiteSpace(s) ? null : s;
    }
}


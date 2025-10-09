using RealEstateCRMWinForms.ViewModels;
using RealEstateCRMWinForms.Models;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Microsoft.EntityFrameworkCore;

namespace RealEstateCRMWinForms.Views
{
    public partial class InquiriesView : UserControl
    {
        private DataGridView dataGridViewInquiries;
        private Panel panelHeader;
        private Label lblTitle;
        private ComboBox cmbFilter;
        private Button btnRefresh;

        public InquiriesView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Main panel
            this.BackColor = Color.FromArgb(248, 249, 250);
            this.Dock = DockStyle.Fill;

            // Header panel
            panelHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 80,
                BackColor = Color.White,
                Padding = new Padding(30, 20, 30, 20)
            };

            // Title
            lblTitle = new Label
            {
                Text = "Client Inquiries",
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                ForeColor = Color.FromArgb(33, 37, 41),
                Location = new Point(30, 25),
                AutoSize = true
            };

            // Filter combo box
            cmbFilter = new ComboBox
            {
                Font = new Font("Segoe UI", 10F),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Location = new Point(800, 25),
                Width = 150
            };
            // Removed "Closed" from filter options per request
            cmbFilter.Items.AddRange(new object[] { "All", "Pending", "Read", "Responded" });
            cmbFilter.SelectedIndex = 1; // Default to "Pending"
            cmbFilter.SelectedIndexChanged += CmbFilter_SelectedIndexChanged;

            // Refresh button
            btnRefresh = new Button
            {
                Text = "ðŸ”„ Refresh",
                Font = new Font("Segoe UI", 10F),
                Location = new Point(960, 22),
                Size = new Size(100, 35),
                BackColor = Color.FromArgb(0, 123, 255),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnRefresh.FlatAppearance.BorderSize = 0;
            btnRefresh.Click += BtnRefresh_Click;
            btnRefresh.Visible = false; // hide refresh button in broker view

            panelHeader.Controls.AddRange(new Control[] { lblTitle, cmbFilter });

            // Reconfigure header to be responsive
            try
            {
                panelHeader.Controls.Remove(lblTitle);
                panelHeader.Controls.Remove(cmbFilter);
                panelHeader.Controls.Remove(btnRefresh);

                lblTitle.Dock = DockStyle.Left;

                var rightHost = new FlowLayoutPanel
                {
                    Dock = DockStyle.Right,
                    FlowDirection = FlowDirection.LeftToRight,
                    WrapContents = false,
                    AutoSize = true,
                    AutoSizeMode = AutoSizeMode.GrowAndShrink,
                    BackColor = Color.Transparent,
                    Padding = new Padding(0),
                    Margin = new Padding(0)
                };
                cmbFilter.Margin = new Padding(0, 0, 8, 0);
                if (btnRefresh.Text?.Contains("Refresh") != true) btnRefresh.Text = "Refresh";
                rightHost.Controls.Add(cmbFilter);
                // refresh button removed
                // rightHost.Controls.Add(btnRefresh);
                panelHeader.Controls.Add(rightHost);
                panelHeader.Controls.Add(lblTitle);
            }
            catch { }
            // DataGridView
            dataGridViewInquiries = new DataGridView
            {
                Dock = DockStyle.Fill,
                Location = new Point(30, 100),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                ColumnHeadersHeight = 40,
                RowTemplate = { Height = 50 },
                Font = new Font("Segoe UI", 10F)
            };

            dataGridViewInquiries.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(248, 249, 250);
            dataGridViewInquiries.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(73, 80, 87);
            dataGridViewInquiries.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            dataGridViewInquiries.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(248, 249, 250);
            dataGridViewInquiries.DefaultCellStyle.SelectionBackColor = Color.FromArgb(225, 239, 255);
            dataGridViewInquiries.DefaultCellStyle.SelectionForeColor = Color.FromArgb(33, 37, 41);
            dataGridViewInquiries.EnableHeadersVisualStyles = false;

            dataGridViewInquiries.CellDoubleClick += DataGridViewInquiries_CellDoubleClick;

            Controls.AddRange(new Control[] { dataGridViewInquiries, panelHeader });

            this.ResumeLayout(false);

            // Load inquiries after all controls are initialized
            LoadInquiries();
        }

        private void LoadInquiries()
        {
            try
            {
                // Add null checks
                if (lblTitle == null)
                {
                    MessageBox.Show("lblTitle is null", "Debug", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (dataGridViewInquiries == null)
                {
                    MessageBox.Show("dataGridViewInquiries is null", "Debug", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                using var db = Data.DbContextHelper.CreateDbContext();

                if (db == null)
                {
                    MessageBox.Show("Database context is null", "Debug", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Get the selected filter
                var filter = cmbFilter?.SelectedItem?.ToString() ?? "Pending";

                // Query inquiries with related data
                var query = db.Inquiries
                    .Include(i => i.Property)
                    .Include(i => i.Client)
                    .AsQueryable();

                if (query == null)
                {
                    MessageBox.Show("Query is null", "Debug", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Apply filter
                if (filter != "All")
                {
                    var status = Enum.Parse<InquiryStatus>(filter);
                    query = query.Where(i => i.Status == status);
                }

                var inquiries = query
                    .OrderByDescending(i => i.CreatedAt)
                    .Select(i => new
                    {
                        i.Id,
                        PropertyTitle = i.Property != null ? i.Property.Title : "N/A",
                        PropertyAddress = i.Property != null ? i.Property.Address : "N/A",
                        ClientName = i.Client != null ? i.Client.FullName : "N/A",
                        ClientEmail = i.Client != null ? i.Client.Email : "N/A",
                        i.Message,
                        i.Status,
                        CreatedDate = i.CreatedAt.ToString("MM/dd/yyyy hh:mm tt"),
                        i.ContactEmail,
                        i.ContactPhone
                    })
                    .ToList();

                if (inquiries == null)
                {
                    MessageBox.Show("Inquiries list is null", "Debug", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                dataGridViewInquiries.DataSource = inquiries;

                // Configure columns - wrap in try-catch to handle any binding issues
                try
                {
                    if (dataGridViewInquiries.Columns.Count > 0)
                    {
                        // Hide ID column
                        if (dataGridViewInquiries.Columns.Contains("Id"))
                            dataGridViewInquiries.Columns["Id"].Visible = false;

                        // Configure visible columns
                        if (dataGridViewInquiries.Columns.Contains("PropertyTitle"))
                        {
                            dataGridViewInquiries.Columns["PropertyTitle"].HeaderText = "Property";
                            dataGridViewInquiries.Columns["PropertyTitle"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                            dataGridViewInquiries.Columns["PropertyTitle"].FillWeight = 140;
                            dataGridViewInquiries.Columns["PropertyTitle"].MinimumWidth = 160;
                        }
                        if (dataGridViewInquiries.Columns.Contains("PropertyAddress"))
                        {
                            dataGridViewInquiries.Columns["PropertyAddress"].HeaderText = "Location";
                            dataGridViewInquiries.Columns["PropertyAddress"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                            dataGridViewInquiries.Columns["PropertyAddress"].FillWeight = 120;
                            dataGridViewInquiries.Columns["PropertyAddress"].MinimumWidth = 140;
                        }
                        if (dataGridViewInquiries.Columns.Contains("ClientName"))
                        {
                            dataGridViewInquiries.Columns["ClientName"].HeaderText = "Client";
                            dataGridViewInquiries.Columns["ClientName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                            dataGridViewInquiries.Columns["ClientName"].FillWeight = 110;
                            dataGridViewInquiries.Columns["ClientName"].MinimumWidth = 120;
                        }
                        if (dataGridViewInquiries.Columns.Contains("ClientEmail"))
                        {
                            dataGridViewInquiries.Columns["ClientEmail"].HeaderText = "Email";
                            dataGridViewInquiries.Columns["ClientEmail"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                            dataGridViewInquiries.Columns["ClientEmail"].FillWeight = 150;
                            dataGridViewInquiries.Columns["ClientEmail"].MinimumWidth = 160;
                        }
                        if (dataGridViewInquiries.Columns.Contains("Message"))
                        {
                            dataGridViewInquiries.Columns["Message"].HeaderText = "Message";
                            dataGridViewInquiries.Columns["Message"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                            dataGridViewInquiries.Columns["Message"].FillWeight = 220; // larger share but controlled
                            dataGridViewInquiries.Columns["Message"].MinimumWidth = 220; // avoid collapsing too small
                        }
                        if (dataGridViewInquiries.Columns.Contains("Status"))
                        {
                            dataGridViewInquiries.Columns["Status"].HeaderText = "Status";
                            dataGridViewInquiries.Columns["Status"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                            dataGridViewInquiries.Columns["Status"].FillWeight = 90;
                            dataGridViewInquiries.Columns["Status"].MinimumWidth = 90;
                        }
                        if (dataGridViewInquiries.Columns.Contains("CreatedDate"))
                        {
                            dataGridViewInquiries.Columns["CreatedDate"].HeaderText = "Date";
                            dataGridViewInquiries.Columns["CreatedDate"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                            dataGridViewInquiries.Columns["CreatedDate"].FillWeight = 140;
                            dataGridViewInquiries.Columns["CreatedDate"].MinimumWidth = 140;
                        }
                        if (dataGridViewInquiries.Columns.Contains("ContactEmail"))
                        {
                            dataGridViewInquiries.Columns["ContactEmail"].HeaderText = "Alt. Email";
                            dataGridViewInquiries.Columns["ContactEmail"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                            dataGridViewInquiries.Columns["ContactEmail"].FillWeight = 160;
                            dataGridViewInquiries.Columns["ContactEmail"].MinimumWidth = 160;
                        }
                        if (dataGridViewInquiries.Columns.Contains("ContactPhone"))
                        {
                            dataGridViewInquiries.Columns["ContactPhone"].HeaderText = "Phone";
                            dataGridViewInquiries.Columns["ContactPhone"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                            dataGridViewInquiries.Columns["ContactPhone"].FillWeight = 120;
                            dataGridViewInquiries.Columns["ContactPhone"].MinimumWidth = 120;
                        }
                        // widths and fill already set above; no duplicates here
                    }
                }
                catch (Exception colEx)
                {
                    // Log column configuration error but don't stop the load
                    System.Diagnostics.Debug.WriteLine($"Error configuring columns: {colEx.Message}");
                }

                // Update count in title
                lblTitle.Text = $"Client Inquiries ({inquiries.Count})";
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error loading inquiries: {ex.Message}\n\nStack Trace:\n{ex.StackTrace}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void DataGridViewInquiries_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            try
            {
                var inquiryId = Convert.ToInt32(dataGridViewInquiries.Rows[e.RowIndex].Cells["Id"].Value);

                using var db = Data.DbContextHelper.CreateDbContext();
                var inquiry = db.Inquiries
                    .Include(i => i.Property)
                    .Include(i => i.Client)
                    .FirstOrDefault(i => i.Id == inquiryId);

                if (inquiry != null)
                {
                    // Mark as read if still pending
                    if (inquiry.Status == InquiryStatus.Pending)
                    {
                        inquiry.Status = InquiryStatus.Read;
                        inquiry.ReadAt = DateTime.Now;
                        db.SaveChanges();
                    }

                    // Show assign agent dialog
                    var assignDialog = new AssignAgentDialog(inquiry);
                    if (assignDialog.ShowDialog() == DialogResult.OK)
                    {
                        // Process the agent assignment and create deal
                        ProcessAgentAssignment(inquiry, assignDialog.SelectedAgentId!.Value, assignDialog.DealNotes);
                    }

                    // Refresh the list
                    LoadInquiries();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error viewing inquiry: {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void ProcessAgentAssignment(Inquiry inquiry, int agentId, string? dealNotes)
        {
            try
            {
                using var db = Data.DbContextHelper.CreateDbContext();

                // Re-fetch inquiry with full tracking
                var trackedInquiry = db.Inquiries
                    .Include(i => i.Property)
                    .Include(i => i.Client)
                    .FirstOrDefault(i => i.Id == inquiry.Id);

                if (trackedInquiry == null)
                {
                    MessageBox.Show("Inquiry not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Get agent's full name for assignment marker
                var agent = db.Users.Find(agentId);
                if (agent == null)
                {
                    MessageBox.Show("Agent not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string agentFullName = $"{agent.FirstName} {agent.LastName}".Trim();

                // Step 1: Find or create contact from client
                Contact? contact = null;
                if (trackedInquiry.Client != null)
                {
                    // Check if contact already exists for this client
                    contact = db.Contacts.FirstOrDefault(c =>
                        c.Email == trackedInquiry.Client.Email && c.IsActive);

                    if (contact == null)
                    {
                        // Create new contact from client
                        contact = new Contact
                        {
                            FullName = trackedInquiry.Client.FullName,
                            Email = trackedInquiry.Client.Email,
                            Phone = trackedInquiry.ContactPhone ?? "",
                            Type = "Buyer", // Default type for inquiry-based contacts
                            CreatedAt = DateTime.UtcNow,
                            IsActive = true
                        };
                        db.Contacts.Add(contact);
                        db.SaveChanges(); // Save to get contact ID

                        Services.LoggingService.LogAction(
                            "Contact Created from Inquiry",
                            $"Contact '{contact.FullName}' created from inquiry #{trackedInquiry.Id}");
                    }
                }

                // Step 2: Create deal entry
                bool isBroker = agent.Role == UserRole.Broker;
                string fullNotes;
                string createdBy = null;

                if (isBroker)
                {
                    fullNotes = string.IsNullOrWhiteSpace(dealNotes) ? string.Empty : dealNotes.Trim();
                    createdBy = agentFullName;
                }
                else
                {
                    string assignmentMarkerName = $"[ASSIGN:{agentFullName}]";
                    string assignmentMarkerId = $"[ASSIGNID:{agent.Id}]";
                    string combinedMarker = $"{assignmentMarkerName} {assignmentMarkerId}";
                    fullNotes = string.IsNullOrWhiteSpace(dealNotes)
                        ? combinedMarker
                        : $"{combinedMarker}\n\n{dealNotes}";
                }

                var deal = new Deal
                {
                    Title = $"{trackedInquiry.Property?.Title ?? "Property"} - {trackedInquiry.Client?.FullName ?? "Client"}",
                    Description = $"Deal created from inquiry.\n\nClient Message: {trackedInquiry.Message}",
                    PropertyId = trackedInquiry.PropertyId,
                    ContactId = contact?.Id,
                    Status = BoardViewModel.NewBoardName,
                    Value = trackedInquiry.Property?.Price,
                    Notes = string.IsNullOrWhiteSpace(fullNotes) ? null : fullNotes,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true,
                    CreatedBy = createdBy
                };

                db.Deals.Add(deal);
                // Step 3: Update inquiry status
                trackedInquiry.Status = InquiryStatus.Responded;
                trackedInquiry.RespondedAt = DateTime.Now;
                trackedInquiry.Status = InquiryStatus.Responded;
                trackedInquiry.RespondedAt = DateTime.Now;
                trackedInquiry.RespondedByBrokerId = agentId;
                trackedInquiry.BrokerResponse = isBroker
                    ? $"Broker {agentFullName} accepted the inquiry and created a deal in the New pipeline."
                    : $"Assigned to {agentFullName} - pending acceptance.";

                db.SaveChanges();

                var logMessage = isBroker
                    ? $"Broker {agentFullName} converted inquiry #{trackedInquiry.Id} into a deal in the New pipeline."
                    : $"Deal '{deal.Title}' created from inquiry #{trackedInquiry.Id} and assigned to {agentFullName} - pending acceptance";

                Services.LoggingService.LogAction(
                    "Deal Assigned from Inquiry",
                    logMessage);

                var infoMessage = isBroker
                    ? $"Inquiry has been converted into a deal for broker {agentFullName}.\n\n" +
                      $"• Contact: {contact?.FullName ?? "N/A"}\n" +
                      $"• Deal created in pipeline: New\n" +
                      $"• Inquiry marked as responded"
                    : $"Assignment Created!\n\n" +
                      $"• Agent '{agentFullName}' assigned to inquiry\n" +
                      $"• Contact created: {contact?.FullName ?? "N/A"}\n" +
                      $"• Deal created and pending agent acceptance\n" +
                      $"• Inquiry marked as responded";

                var infoTitle = isBroker ? "Inquiry Converted" : "Assignment Pending";

                MessageBox.Show(
                    infoMessage,
                    infoTitle,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error processing agent assignment: {ex.Message}\n\nStack Trace:\n{ex.StackTrace}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void CmbFilter_SelectedIndexChanged(object? sender, EventArgs e)
        {
            LoadInquiries();
        }

        private void BtnRefresh_Click(object? sender, EventArgs e)
        {
            LoadInquiries();
        }
    }
}


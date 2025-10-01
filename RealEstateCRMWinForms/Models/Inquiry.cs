using System;

namespace RealEstateCRMWinForms.Models
{
    public enum InquiryStatus
    {
        Pending,
        Read,
        Responded,
        Closed
    }

    public class Inquiry
    {
        public int Id { get; set; }

        // Property being inquired about
        public int PropertyId { get; set; }
        public Property? Property { get; set; }

        // Client making the inquiry
        public int ClientId { get; set; }
        public User? Client { get; set; }

        // Inquiry details
        public string Message { get; set; } = string.Empty;
        public InquiryStatus Status { get; set; } = InquiryStatus.Pending;

        // Contact information (optional, in case client wants to provide additional contact)
        public string? ContactEmail { get; set; }
        public string? ContactPhone { get; set; }

        // Timestamps
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? ReadAt { get; set; }
        public DateTime? RespondedAt { get; set; }

        // Broker response
        public string? BrokerResponse { get; set; }
        public int? RespondedByBrokerId { get; set; }
        public User? RespondedByBroker { get; set; }
    }
}

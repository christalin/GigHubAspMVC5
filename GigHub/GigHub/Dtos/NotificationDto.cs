using System;
using System.ComponentModel.DataAnnotations;
using GigHub.Models;

namespace GigHub.Dtos
{
    public class NotificationDto
    {
        public DateTime DateTime { get; set; }
        public NotificationType Type { get; set; }
        public DateTime? OriginalDateTime { get; set; }
        public string OriginalVenue { get; set; }
        [Required]
        public GigDto Gig { get; set; }

    }
}
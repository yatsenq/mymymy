using System;

namespace StayFit.DAL.Entities;

public class UserSession
{
	public int UserSessionId { get; set; }
	public int UserId { get; set; }
	public string AccessTokenHash { get; set; } = string.Empty;
	public string RefreshTokenHash { get; set; } = string.Empty;
	public string? DeviceInfo { get; set; }
	public string? IpAddress { get; set; }
	public bool IsActive { get; set; } = true;
	public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
	public DateTime AccessTokenExpiresAt { get; set; }
	public DateTime RefreshTokenExpiresAt { get; set; }

	public User User { get; set; } = null!;
}

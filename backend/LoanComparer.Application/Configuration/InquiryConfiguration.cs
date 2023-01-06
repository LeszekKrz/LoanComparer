using System.ComponentModel.DataAnnotations;

namespace LoanComparer.Application.Configuration;

public sealed class InquiryConfiguration : IEquatable<InquiryConfiguration>
{
    public const string SectionName = "Inquiries";
    
    [Required]
    [Range(typeof(TimeSpan), "00:00:10", "00:10:00")]
    public TimeSpan RefreshInterval { get; init; }
    
    [Required]
    [Range(typeof(TimeSpan), "00:00:30", "01:00:00")]
    public TimeSpan CleanupInterval { get; init; }
    
    [Required]
    [Range(typeof(TimeSpan), "00:01:00", "24:00:00")]
    public TimeSpan TimeoutInterval { get; init; }

    public bool Equals(InquiryConfiguration? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return RefreshInterval.Equals(other.RefreshInterval) && CleanupInterval.Equals(other.CleanupInterval) &&
               TimeoutInterval.Equals(other.TimeoutInterval);
    }

    public override bool Equals(object? obj)
    {
        return ReferenceEquals(this, obj) || obj is InquiryConfiguration other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(RefreshInterval, CleanupInterval, TimeoutInterval);
    }

    public static bool operator ==(InquiryConfiguration? left, InquiryConfiguration? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(InquiryConfiguration? left, InquiryConfiguration? right)
    {
        return !Equals(left, right);
    }
}
using Himam_main.Data;
using Microsoft.EntityFrameworkCore;

namespace Himam_main.Services;

public class BruteForceProtectionService
{
    private readonly HimanAlhayahContext _context;
    private readonly Dictionary<string, FailedLoginAttempt> _failedAttempts;
    private readonly object _lock = new object();

    private const int MaxFailedAttempts = 5;
    private const int LockoutDurationMinutes = 15;

    public BruteForceProtectionService(HimanAlhayahContext context)
    {
        _context = context;
        _failedAttempts = new Dictionary<string, FailedLoginAttempt>();
    }

    public bool IsLockedOut(string identifier)
    {
        lock (_lock)
        {
            if (_failedAttempts.TryGetValue(identifier, out var attempt))
            {
                // Check if lockout has expired
                if (DateTime.UtcNow < attempt.LockUntil)
                {
                    return true;
                }
                else
                {
                    // Lockout expired, remove entry
                    _failedAttempts.Remove(identifier);
                }
            }
            return false;
        }
    }

    public void RecordFailedAttempt(string identifier)
    {
        lock (_lock)
        {
            if (_failedAttempts.TryGetValue(identifier, out var attempt))
            {
                attempt.FailedCount++;
                attempt.LastAttempt = DateTime.UtcNow;

                if (attempt.FailedCount >= MaxFailedAttempts)
                {
                    attempt.LockUntil = DateTime.UtcNow.AddMinutes(LockoutDurationMinutes);
                }
            }
            else
            {
                _failedAttempts[identifier] = new FailedLoginAttempt
                {
                    FailedCount = 1,
                    LastAttempt = DateTime.UtcNow
                };
            }
        }
    }

    public void ResetFailedAttempts(string identifier)
    {
        lock (_lock)
        {
            _failedAttempts.Remove(identifier);
        }
    }

    public int GetRemainingLockoutTime(string identifier)
    {
        lock (_lock)
        {
            if (_failedAttempts.TryGetValue(identifier, out var attempt))
            {
                if (DateTime.UtcNow < attempt.LockUntil)
                {
                    return (int)(attempt.LockUntil - DateTime.UtcNow).TotalMinutes;
                }
            }
            return 0;
        }
    }

    private class FailedLoginAttempt
    {
        public int FailedCount { get; set; }
        public DateTime LastAttempt { get; set; }
        public DateTime LockUntil { get; set; }
    }
}

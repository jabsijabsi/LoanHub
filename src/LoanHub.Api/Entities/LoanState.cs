namespace LoanHub.Api.Entities;

/// <summary>Lifecycle states a loan can move through.</summary>
public enum LoanState
{
    Pending,
    Approved,
    Rejected,
    Closed,
    Overdue
}

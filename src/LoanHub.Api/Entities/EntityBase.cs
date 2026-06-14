namespace LoanHub.Api.Entities;

public abstract class EntityBase : ISoftDeletable
{
    public int Id { get; set; }

    public DateTime CreatedOn { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public bool IsDeleted { get; set; }
}

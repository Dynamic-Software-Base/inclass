using SharedKernel.ValueObjects.StronglyTypedIds;

namespace SharedKernel.ValueObjects.Registration;

public sealed record ReviewNote
{
    public UserId ReviewedBy { get; }
    public DateTime ReviewedAt { get; }

    /// <summary>Motif de rejet — obligatoire si le dossier est rejeté</summary>
    public string? Comment { get; }

    private ReviewNote(UserId reviewedBy, DateTime reviewedAt, string? comment)
    {
        ReviewedBy = reviewedBy;
        ReviewedAt = reviewedAt;
        Comment = comment;
    }

    public static ErrorOr<ReviewNote> CreateApproval(UserId reviewedBy, DateTime reviewedAt)
    {
        return new ReviewNote(reviewedBy, reviewedAt, null);
    }

    public static ErrorOr<ReviewNote> CreateRejection(
        UserId reviewedBy,
        DateTime reviewedAt,
        string comment)
    {
        if (string.IsNullOrWhiteSpace(comment))
        {
            return Error.Validation("ReviewNote.Comment.Empty",
                "Le motif de rejet est obligatoire.");

        }

        if (comment.Length > 1000)
        {
            return Error.Validation("ReviewNote.Comment.TooLong",
                "Le motif de rejet ne peut pas dépasser 1000 caractères.");
        }


        return new ReviewNote(reviewedBy, reviewedAt, comment.Trim());
    }
}

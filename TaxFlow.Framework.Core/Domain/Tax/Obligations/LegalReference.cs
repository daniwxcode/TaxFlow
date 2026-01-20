using Core.Domain.Contracts.Abstracts;

namespace Core.Domain.Tax.Obligations;

/// <summary>
/// Represents a legal reference that justifies a tax obligation.
/// </summary>
public sealed class LegalReference : AuditableEntity
{
    /// <summary>
    /// Type of legal text (law, decree, order, circular, etc.).
    /// </summary>
    public LegalTextType TextType { get; init; } = LegalTextType.Law;

    /// <summary>
    /// Reference number or identifier of the legal text.
    /// </summary>
    public string Reference { get; init; } = string.Empty;

    /// <summary>
    /// Title or short description of the legal text.
    /// </summary>
    public string Title { get; init; } = string.Empty;

    /// <summary>
    /// Specific article(s) within the legal text.
    /// </summary>
    public string? Article { get; init; }

    /// <summary>
    /// Publication date of the legal text.
    /// </summary>
    public DateOnly? PublicationDate { get; init; }

    /// <summary>
    /// Effective date from which the legal text applies.
    /// </summary>
    public DateOnly? EffectiveDate { get; init; }

    /// <summary>
    /// URL or link to the official publication.
    /// </summary>
    public string? Url { get; init; }

    /// <summary>
    /// Additional notes or commentary.
    /// </summary>
    public string? Notes { get; init; }

    /// <summary>
    /// Creates a new legal reference.
    /// </summary>
    public static LegalReference Create(
        LegalTextType textType,
        string reference,
        string title,
        string? article = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(reference);
        ArgumentException.ThrowIfNullOrWhiteSpace(title);

        return new LegalReference
        {
            TextType = textType,
            Reference = reference.Trim(),
            Title = title.Trim(),
            Article = string.IsNullOrWhiteSpace(article) ? null : article.Trim()
        };
    }

    /// <summary>
    /// Sets the publication and effective dates.
    /// </summary>
    public LegalReference WithDates(DateOnly? publicationDate, DateOnly? effectiveDate = null)
    {
        return new LegalReference
        {
            TextType = TextType,
            Reference = Reference,
            Title = Title,
            Article = Article,
            PublicationDate = publicationDate,
            EffectiveDate = effectiveDate ?? publicationDate,
            Url = Url,
            Notes = Notes
        };
    }

    /// <summary>
    /// Sets the URL for the legal reference.
    /// </summary>
    public LegalReference WithUrl(string url)
    {
        return new LegalReference
        {
            TextType = TextType,
            Reference = Reference,
            Title = Title,
            Article = Article,
            PublicationDate = PublicationDate,
            EffectiveDate = EffectiveDate,
            Url = string.IsNullOrWhiteSpace(url) ? null : url.Trim(),
            Notes = Notes
        };
    }

    /// <summary>
    /// Gets a formatted citation string.
    /// </summary>
    public string GetCitation()
    {
        var citation = $"{TextType.GetDisplayName()} {Reference}";
        if (!string.IsNullOrWhiteSpace(Article))
            citation += $", art. {Article}";
        return citation;
    }

    public override string ToString() => GetCitation();
}

/// <summary>
/// Type of legal text.
/// </summary>
public enum LegalTextType
{
    /// <summary>
    /// Law (Loi).
    /// </summary>
    Law = 1,

    /// <summary>
    /// Decree (Décret).
    /// </summary>
    Decree = 2,

    /// <summary>
    /// Order (Arrêté).
    /// </summary>
    Order = 3,

    /// <summary>
    /// Circular (Circulaire).
    /// </summary>
    Circular = 4,

    /// <summary>
    /// Instruction (Instruction).
    /// </summary>
    Instruction = 5,

    /// <summary>
    /// Tax Code (Code Général des Impôts).
    /// </summary>
    TaxCode = 6,

    /// <summary>
    /// Finance Law (Loi de Finances).
    /// </summary>
    FinanceLaw = 7,

    /// <summary>
    /// Regulation (Règlement).
    /// </summary>
    Regulation = 8,

    /// <summary>
    /// Convention (Convention fiscale).
    /// </summary>
    Convention = 9,

    /// <summary>
    /// Other type of legal text.
    /// </summary>
    Other = 99
}

/// <summary>
/// Extension methods for LegalTextType.
/// </summary>
public static class LegalTextTypeExtensions
{
    /// <summary>
    /// Gets the display name for a legal text type.
    /// </summary>
    public static string GetDisplayName(this LegalTextType type) => type switch
    {
        LegalTextType.Law => "Loi",
        LegalTextType.Decree => "Décret",
        LegalTextType.Order => "Arrêté",
        LegalTextType.Circular => "Circulaire",
        LegalTextType.Instruction => "Instruction",
        LegalTextType.TaxCode => "CGI",
        LegalTextType.FinanceLaw => "LF",
        LegalTextType.Regulation => "Règlement",
        LegalTextType.Convention => "Convention",
        LegalTextType.Other => "Texte",
        _ => type.ToString()
    };
}

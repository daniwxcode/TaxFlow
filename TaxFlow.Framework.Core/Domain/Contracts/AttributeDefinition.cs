using Core.Domain.Contracts.Abstracts;
using Core.Domain.Enums;

namespace Core.Domain.Contracts;

public class EnumDefinition : AuditableEntity
{
    public string Key { get; set; } = default!;
    public string Label { get; set; } = default!;

    public ICollection<EnumItem> Items { get; set; } = new List<EnumItem>();
}
public class EnumItem : AuditableEntity
{
    public string Code { get; set; } = default!;
    public string Label { get; set; } = default!;
    public int Order { get; set; }

    public int EnumDefinitionId { get; set; }
    public EnumDefinition EnumDefinition { get; set; } = default!;
}
public class AttributeDefinition: AuditableEntity
{
    public string Key { get; set; } 
    public string Label { get; set; }= string.Empty;
    public AttributeDataType DataType { get; set; } = AttributeDataType.String;
    public bool IsRequired { get; set; }
    public int? EnumDefinitionId { get; set; }
    public EnumDefinition? EnumDefinition { get; set; }
    public string? RegexPattern { get; private set; }
    public static AttributeDefinition Create(string key, string label, AttributeDataType dataType, EnumDefinition?definition=null, bool isRequired=false
        ,string regexPattern= null)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("La clé de l'attribut ne doit pas être vide.", nameof(key));
        if (string.IsNullOrWhiteSpace(label))
            throw new ArgumentException("Le label de l'attribut ne doit pas être vide.", nameof(label));
        return new AttributeDefinition
        {
            Key = key.Trim(),
            Label = label.Trim(),
            DataType = dataType,
            IsRequired = isRequired,
            EnumDefinition = definition,
            RegexPattern = regexPattern
        };
    }
}

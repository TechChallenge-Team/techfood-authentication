using TechFood.Domain.Enums;
using TechFood.Domain.Common.Exceptions;
using TechFood.Domain.Common.Validations;
using TechFood.Domain.Common.ValueObjects;

namespace TechFood.Domain.ValueObjects;

public class Document : ValueObject
{
    public Document(
        DocumentType type,
        string value)
    {
        Type = type;
        Value = value;
        Validate();
    }
    public DocumentType Type { get; set; }

    public string Value { get; set; }

    private void Validate()
    {
        if (Type == DocumentType.CPF && !ValidaDocument.ValidarCPF(Value))
        {
            throw new DomainException(Resources.Exceptions.Customer_ThrowDocumentCPFInvalid);
        }
    }
}

using System;
using TechFood.Shared.Domain.Exceptions;
using TechFood.Shared.Domain.Validations;
using TechFood.Shared.Domain.ValueObjects;

namespace TechFood.Domain.ValueObjects;

public class Email : ValueObject, IEquatable<Email>
{
    public Email(string address)
    {
        Address = address;
        Validate();
    }

    public string Address { get; set; }

    public bool Equals(Email? other)
    {
        return other is not null &&
               Address == other.Address;
    }

    public static implicit operator Email(string address) => new(address);

    public static implicit operator string(Email email) => email.Address;
    private void Validate()
    {
        if (!ValidateEmail.IsValidEmail(Address))
        {
            throw new DomainException(Resources.Exceptions.Customer_ThrowEmailInvalid);
        }
    }
}

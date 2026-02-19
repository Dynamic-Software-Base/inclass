using System.ComponentModel.DataAnnotations;

namespace Application.Schools.Contracts;

public sealed record CreateSchoolRequest(
    [property: Required(AllowEmptyStrings = false)]
    [property: MinLength(2)]
    [property: MaxLength(200)]
    string Name);

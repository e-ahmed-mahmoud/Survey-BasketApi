namespace SurveyBasket.Contracts.Users;

public record UserUpdateRequest(string FirstName, string LastName, string PhoneNumber, IList<string> Roles);



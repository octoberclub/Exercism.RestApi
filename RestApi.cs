using System;
using System.Linq;
using System.Text.Json;


public class RestApi
{
    private UserDto[] Users { get; set; }
    public RestApi(string database)
    {
        Users = JsonSerializer.Deserialize<UserDto[]>(database);
    }

    public string Get(string url, string payload = null)
    {
        if (url == "/users" && payload != null)
        {
            var request = JsonSerializer.Deserialize<UsersRequest>(payload);
            if (request.users.Length > 0)
            {
                return JsonSerializer.Serialize(Users.Where(u => u.name == request.users[0]));
            }
        }
        return "[]";
    }

    public string Post(string url, string payload)
    {
        var createUser = JsonSerializer.Deserialize<CreateUserCommand>(payload);
        var userDto = new UserDto
        {
            name = createUser.user,
            owes = new OwesDto(),
            owed_by = new OwedByDto(),
            balance = 0.0m,
        };
        
        return JsonSerializer.Serialize(userDto);
    }

    public class CreateUserCommand
    {
        public string user { get; set; }
    }

    public class UserDto
    {
        public string name { get; set; }
        public OwesDto owes { get; set; }
        public OwedByDto owed_by { get; set; }
        public decimal balance { get; set; }
    }

    public class OwesDto
    {
    }

    public class OwedByDto
    {
    }
    public class UsersRequest
    {
        public string [] users { get; set; }
    }
}


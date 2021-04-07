using System;
using System.Collections.Generic;
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
        if (url == "/add")
        {
            var createUser = JsonSerializer.Deserialize<CreateUserCommand>(payload);
            var userDto = new UserDto
            {
                name = createUser.user,
            };

            return JsonSerializer.Serialize(userDto);
        }

        if (url == "/iou")
        {
            var paymentCommand = JsonSerializer.Deserialize<PaymentCommand>(payload);

            var borrowed = new Dictionary<string, decimal> {{paymentCommand.borrower, paymentCommand.amount}};
            var lent = new Dictionary<string, decimal> {{paymentCommand.lender, paymentCommand.amount}};

            var updatedUsers = Users.Select(u => new UserDto()
            {
                name = u.name,
                owed_by = u.name == paymentCommand.lender ? borrowed : u.owed_by,
                owes = u.name == paymentCommand.borrower ? lent : u.owes,
                balance = u.name == paymentCommand.lender ? paymentCommand.amount : -paymentCommand.amount,
            });

            return JsonSerializer.Serialize(updatedUsers);
        }
        return "[]";
    }

    public class PaymentCommand
    {
        public string borrower { get; set; }
        public string lender { get; set; }
        public decimal amount { get; set; }
    }

    public class CreateUserCommand
    {
        public string user { get; set; }
    }

    public class UserDto
    {
        public UserDto()
        {
            owes = new Dictionary<string, decimal>();
            owed_by = new Dictionary<string, decimal>();
            balance = 0.0m;
        }
        public string name { get; set; }
        public Dictionary<string, decimal> owes { get; set; }
        public Dictionary<string, decimal> owed_by { get; set; }
        public decimal balance { get; set; }
    }

    
    public class UsersRequest
    {
        public string [] users { get; set; }
    }
}


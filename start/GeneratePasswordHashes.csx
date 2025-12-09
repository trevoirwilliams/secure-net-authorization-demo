using Microsoft.AspNetCore.Identity;

var hasher = new PasswordHasher<IdentityUser>();
var adminUser = new IdentityUser { Id = "admin-user-id", UserName = "admin@demo.local" };
var aliceUser = new IdentityUser { Id = "alice-user-id", UserName = "alice@demo.local" };
var bobUser = new IdentityUser { Id = "bob-user-id", UserName = "bob@demo.local" };

Console.WriteLine("Admin: " + hasher.HashPassword(adminUser, "AdminPassword123!"));
Console.WriteLine("Alice: " + hasher.HashPassword(aliceUser, "AlicePassword123!"));
Console.WriteLine("Bob: " + hasher.HashPassword(bobUser, "BobPassword123!"));
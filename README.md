# Task9 - User Panel

Simple ASP.NET Core MVC application for practicing password hashing, cookie authentication, and role-based authorization.

## How to run

From the project folder:

```powershell
cd C:\Users\Anton\RiderProjects\Task9\Task9
dotnet run
```

Open the URL printed by `dotnet run`, usually `http://localhost:5000` or `https://localhost:5001`.

The app stores demo data in `Task9/App_Data/users.json`. This file is ignored by Git because it contains local users and password hashes.

## How to create a test user

1. Open `/Account/Register`.
2. Enter an email address.
3. Enter a password with at least 8 characters.
4. Submit the form.

Every registered user gets the `User` role.

## How to log in as Admin

On first startup the app creates one local demo admin account if it does not already exist:

- Email: `admin@example.com`
- Password: `Admin123!`

This is only a local demonstration password for the assignment. Do not reuse it for real systems.

## Where password hashing code is

Password hashing is done with ASP.NET Core Identity `PasswordHasher<AppUser>`:

- `Task9/Program.cs` seeds the demo admin and hashes the admin password.
- `Task9/Controllers/AccountController.cs` hashes passwords during registration.
- `Task9/Controllers/AccountController.cs` verifies password hashes during login.

No plain text password is stored in the repository. The password is not printed in views, logs, errors, or this README except for the local demo admin password above.

## Where authentication is configured

Cookie authentication is configured in `Task9/Program.cs`:

- `AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)`
- `AddCookie(...)`
- `UseAuthentication()`
- `UseAuthorization()`

After a successful login, `AccountController` creates a `ClaimsPrincipal` with the user id, email, name, and role, then signs it in with `HttpContext.SignInAsync`.

Logout uses `HttpContext.SignOutAsync`.

## Protected actions

Private dashboard:

- `DashboardController` is protected with `[Authorize]`.
- Anonymous users are redirected to `/Account/Login`.
- Notes are loaded by the current signed-in user's id, so one user's notes are hidden from other users.

Admin page:

- `AdminController` is protected with `[Authorize(Roles = UserRoles.Admin)]`.
- Normal users cannot open `/Admin`.
- Hiding or showing the menu link is only for convenience; the controller authorization is the security boundary.

## Security questions

### Why must passwords not be stored as plain text?

If a database is leaked, plain text passwords can be used immediately. Many people reuse passwords, so one leak can compromise other accounts too.

### Why is raw SHA-256 not a good choice for passwords?

SHA-256 is designed to be fast. Password hashing should be slow and salted so attackers cannot test huge numbers of guesses cheaply.

### Why do we use salt?

A salt makes equal passwords produce different hashes and prevents attackers from using one precomputed table against every user.

### What is the difference between salt and pepper?

A salt is unique per password and stored with the hash. A pepper is an extra secret shared by the application and kept outside the database, such as in user secrets or environment configuration.

### What is the difference between authentication and authorization?

Authentication checks who the user is. Authorization checks what that authenticated user is allowed to do.

### Why is hiding a link in a view not enough as security?

A user can still type the URL directly or send a request manually. The controller action must enforce authorization.

### Why can a "there is no such user" login message be a problem?

It lets attackers discover which email addresses are registered. A generic failed login message avoids revealing whether the email or password was wrong.

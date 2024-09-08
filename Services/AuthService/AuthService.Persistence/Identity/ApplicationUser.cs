// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.


using AuthService.Application.Models.Dtos;
using Microsoft.AspNetCore.Identity;

namespace AuthService.Persistence.Identity;

// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; }
}
﻿namespace SkyBox.Domain.Models.User;

public class ShortUser
{
    public Guid Id { get; set; }
    
    public string UserName { get; set; } = string.Empty;
    
    public string Email { get; set; } = string.Empty;
}
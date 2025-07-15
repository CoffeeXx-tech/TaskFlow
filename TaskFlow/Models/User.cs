using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class User
{
    public string? Username { get; set; }
    public string? Password { get; set; }
    public List<TaskItem> Tasks { get; set; } = new(); 
}


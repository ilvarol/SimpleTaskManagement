﻿namespace SimpleTaskManagement.Api.Domain.Models;

public abstract class BaseEntity
{
    public int Id { get; set; }

    public DateTime CreatedDate { get; set; }
}

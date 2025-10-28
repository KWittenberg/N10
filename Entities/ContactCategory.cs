﻿namespace N10.Entities;

public class ContactCategory : BaseEntity<Guid>
{
    public Guid? ApplicationUserId { get; set; }
    public virtual ApplicationUser? ApplicationUser { get; set; }


    public string Name { get; set; } = string.Empty;

    public string? Color { get; set; } = "#ffffff";



    public virtual ICollection<Contact> Contacts { get; set; } = [];
}

namespace N10.Entities;

public class Note : BaseAuditableEntity<Guid>
{
    public Note() { }

    public Guid? ApplicationUserId { get; set; }
    public virtual ApplicationUser? ApplicationUser { get; set; }

    public Guid? NoteCategoryId { get; set; }
    public NoteCategory? NoteCategories { get; set; }




    // Organizing: notebook/folder (what si ti zvao "Organizer" / mapu)
    //public Guid? OrganizerId { get; set; }
    //public Organizer? Organizer { get; set; }



    // Hierarchy: parent/child notes (sub‑notes, nested pages)
    public Guid? ParentNoteId { get; set; }
    public Note? ParentNote { get; set; }
    public List<Note> Children { get; set; } = new();

    // Core content
    public string Title { get; set; } = string.Empty;
    public string? Body { get; set; }
    public NoteFormat Format { get; set; } = NoteFormat.Markdown;
    public string? Excerpt { get; set; } // short preview / summary

    // Presentation / ordering
    public bool IsPinned { get; set; }
    public int? SortOrder { get; set; } // user defined ordering inside a folder
    public string? Color { get; set; } // hex or named color for UI

    // Lifecycle / state
    public bool IsArchived { get; set; }
    public bool IsTrashed { get; set; } // soft delete
    public bool IsLocked { get; set; } // e.g., read‑only or protected
    public bool IsPrivate { get; set; } // visibility flag

    // Reminders / scheduling
    public DateTime? ReminderAt { get; set; }
    public DateTime? ReminderSnoozedUntil { get; set; }

    // Security / encryption metadata
    public bool IsEncrypted { get; set; } // true if Body is stored encrypted
    public string? EncryptionMetadata { get; set; } // e.g., KMS key id / hint / nonce info







    //// Attachments, tags and comments
    //public List<Attachment> Attachments { get; set; } = new();
    //public List<Tag> Tags { get; set; } = new();
    //public List<Comment> Comments { get; set; } = new();

    //// Collaboration / sharing
    //public List<SharedEntity> SharedWith { get; set; } = new(); // access control entries

    //// Versioning / syncing
    //public int Version { get; set; } = 1; // optimistic versioning
    //public string? ExternalId { get; set; } // for sync with external services
}

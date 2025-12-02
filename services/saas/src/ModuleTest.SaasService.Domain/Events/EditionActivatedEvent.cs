using System;

namespace ModuleTest.SaasService.Events;

[Serializable]
public class EditionActivatedEvent
{
    public Guid EditionId { get; set; }
    
    public string EditionName { get; set; }
    
    public bool IsActivated { get; set; }
    
    public DateTime ChangedAt { get; set; }

    public EditionActivatedEvent(Guid editionId, string editionName, bool isActivated)
    {
        EditionId = editionId;
        EditionName = editionName;
        IsActivated = isActivated;
        ChangedAt = DateTime.UtcNow;
    }
}

using AutoMapper;
using JobApplicationTrackerApi.DTO.Contacts;
using JobApplicationTrackerApi.Persistence.DefaultContext.Entity;

namespace JobApplicationTrackerApi.Mappings;

/// <summary>
/// AutoMapper profile for Contacts entity and DTOs
/// </summary>
public class ContactsMappingProfile : Profile
{
    /// <summary>
    /// Initializes a new instance of the ContactsMappingProfile class
    /// </summary>
    public ContactsMappingProfile()
    {
        // Entity to Response DTO
        CreateMap<Contact, ContactResponseDto>();

        // Create DTO to Entity
        CreateMap<CreateContactDto, Contact>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedUtc, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedUtc, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedUtc, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedBy, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.Ignore())
            .ForMember(dest => dest.Application, opt => opt.Ignore());

        // Update DTO to Entity (for partial updates)
        CreateMap<UpdateContactDto, Contact>()
            .ForAllMembers(opt => opt.Condition((_, _, srcMember) => srcMember != null));
    }
}

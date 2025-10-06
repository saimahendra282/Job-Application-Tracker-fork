using AutoMapper;
using JobApplicationTrackerApi.DTO.Notes;
using JobApplicationTrackerApi.Persistence.DefaultContext.Entity;

namespace JobApplicationTrackerApi.Mappings;

/// <summary>
/// AutoMapper profile for Notes entity and DTOs
/// </summary>
public class NotesMappingProfile : Profile
{
    /// <summary>
    /// Initializes a new instance of the NotesMappingProfile class
    /// </summary>
    public NotesMappingProfile()
    {
        // Entity to Response DTO
        CreateMap<Note, NoteResponseDto>();

        // Create DTO to Entity
        CreateMap<CreateNoteDto, Note>()
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
        CreateMap<UpdateNoteDto, Note>()
            .ForAllMembers(opt => opt.Condition((_, _, srcMember) => srcMember != null));
    }
}

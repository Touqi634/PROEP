using AutoMapper;
using webApp.Models;
using webApp.Resources;

namespace webApp.Mapping
{
    public class ModelToResourceProfile : Profile
    {
        public ModelToResourceProfile()
        {
            CreateMap<TimeRestriction, TimeRestrictionResource>();
            CreateMap<User, UserResource>();
            CreateMap<Message, MessageResource>();
            CreateMap<FlaggedMessage, FlaggedMessageResource>();
        }
    }
}

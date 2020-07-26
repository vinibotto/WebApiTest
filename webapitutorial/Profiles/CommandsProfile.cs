using AutoMapper;
using webapitutorial.Dtos;
using webapitutorial.Models;

namespace webapitutorial.Profiles
{
    public class CommandsProfile : Profile
    {
        public CommandsProfile()
        {
            //Source => Target
            CreateMap<Command,CommandReadDto>();
            CreateMap<CommandCreateDto, Command>();
            CreateMap<CommandUpdateDto, Command>();
            CreateMap<Command, CommandUpdateDto>();
        }
    }
}

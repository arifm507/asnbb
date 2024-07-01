using AllamaShibliQuiz.Models;
using AllamaShibliQuiz.Models.ViewModels;
using AutoMapper;

namespace AllamaShibliQuiz
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<Team, TeamViewModel>().ReverseMap();
            CreateMap<Student, StudentViewModel>().ReverseMap();
        }
    }
}

using AutoMapper;
using BookManagement.DTOs;
using BookManagement.Models;

namespace BookManagement.Mapper
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<Users, LoginDTO>();
            CreateMap<Users, RegisterDTO>();
            CreateMap<LoginDTO, RegisterDTO>();
            CreateMap<LoginDTO, Users>();
            CreateMap<RegisterDTO, LoginDTO>();
            CreateMap<RegisterDTO, Users>();
            CreateMap<Books, BookDTO>();
            CreateMap<BookDTO, Books>();
            CreateMap<BooksBorrowed, BooksBorrowedDTO>();
            CreateMap<BooksBorrowedDTO, BooksBorrowed>();
        }
    }
}

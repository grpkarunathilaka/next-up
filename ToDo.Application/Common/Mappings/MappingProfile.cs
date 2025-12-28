using AutoMapper;
using ToDo.Application.Todos.Queries;
using TodoApp.Domain.Entities;
using static System.Runtime.InteropServices.JavaScript.JSType;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<TodoItem, TodoDto>();
    }
}
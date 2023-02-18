using OutOfTimePrototype.DAL.Models;
using OutOfTimePrototype.DTO;
using static OutOfTimePrototype.Utilities.ClassUtilities;

namespace OutOfTimePrototype.Services
{
    public interface IClassService
    {
        Task<ClassCreationResult> TryCreateClass(ClassDto classDTO);
    }
}

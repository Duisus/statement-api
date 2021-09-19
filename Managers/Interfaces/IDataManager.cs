using System.Threading.Tasks;
using Server.Dto.ModelDto;
using Server.Dto.RequestDto;
using Server.Dto.ResponseDto;
using Server.Logic;

namespace Server.Managers.Interfaces
{
    public interface IDataManager
    {
        public Task<GetStudentsResponseDto> GetStudentsAsync(GetStudentsRequestDto dto);

        public Task<OperationResult<StudentDto>> GetStudentAsync(int id);
    }
}
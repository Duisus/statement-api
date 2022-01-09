using System.Threading.Tasks;
using Aspose.Cells;
using Server.Logic;

namespace Server.Services
{
    public class SpecializationWorksheetAppender: IWorksheetAppender<string>
    {
        public async Task<OperationResult> Append(Workbook workbook, string specializationTitle)
        {
            throw new System.NotImplementedException();
        }
    }
}
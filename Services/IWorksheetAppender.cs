using System.Threading.Tasks;
using Aspose.Cells;
using Server.Logic;

namespace Server.Services
{
    public interface IWorksheetAppender<T>
    {
        public Task<OperationResult> Append(Workbook workbook, T workSheetData);
    }
}
using Aspose.Cells;

namespace Server.Services
{
    public interface IWorksheetAppender<T>
    {
        public void Append(Workbook workbook, T workSheetData);
    }
}
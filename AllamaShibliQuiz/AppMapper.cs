using AllamaShibliQuiz.Models;
using AllamaShibliQuiz.Models.ViewModels;
using Riok.Mapperly.Abstractions;

namespace AllamaShibliQuiz;

// RequiredMappingStrategy.Target: all target members must be mapped (or explicitly ignored).
// Extra source-side members that have no target counterpart are silently ignored,
// which covers: School's Address/CentreCode/Rank/IsActive/ExamDate/ContactNumber/CreateDate/UpdateDate,
// and StudentViewModel's OtherSchoolName/ExamCentreName/ExamCentreCode/ExamDate when mapping → Student,
// and TeamViewModel's ImageBase64 (source) when mapping → Team.
[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class AppMapper
{
    // Student → StudentViewModel
    // These four properties exist only in StudentViewModel, not in the Student entity.
    [MapperIgnoreTarget(nameof(StudentViewModel.OtherSchoolName))]
    [MapperIgnoreTarget(nameof(StudentViewModel.ExamCentreName))]
    [MapperIgnoreTarget(nameof(StudentViewModel.ExamCentreCode))]
    [MapperIgnoreTarget(nameof(StudentViewModel.ExamDate))]
    public partial StudentViewModel StudentToViewModel(Student student);

    // StudentViewModel → Student
    // All Student target members are present in StudentViewModel; extra ViewModel props are ignored by strategy.
    public partial Student ViewModelToStudent(StudentViewModel vm);

    // Team → TeamViewModel
    // ImageBase64 is a get-only computed property on TeamViewModel (no setter).
    [MapperIgnoreTarget(nameof(TeamViewModel.ImageBase64))]
    public partial TeamViewModel TeamToViewModel(Team team);

    // TeamViewModel → Team
    // All Team target members are present in TeamViewModel; ImageBase64 source prop is ignored by strategy.
    public partial Team ViewModelToTeam(TeamViewModel vm);

    // School → SchoolViewModel (reverse direction is never used, so it is not defined here)
    // School's extra properties (Address, CentreCode, Rank, IsActive, ExamDate, ContactNumber,
    // CreateDate, UpdateDate) are ignored by the RequiredMappingStrategy.Target strategy.
    public partial SchoolViewModel SchoolToViewModel(School school);
}

using Grpc.Core;
using GrpcServiceApp.Protos;

namespace GrpcServiceApp.Services
{
    public class StudentService : Student.StudentBase
    {
        ResponseInfo students = new ResponseInfo();
        public override Task<AddStudentResponse> AddStudent(RequestInfo request, ServerCallContext context)
        {
            var student = new StudentInfo
            {
                FirstName=request.FirstName,
                LastName=request.LastName,
                Age=request.Age,
                Email=request.Email,
                BirthDate=request.BirthDate
            };

            students.Students.Add(student);

            if (student == null) return Task.FromResult(new AddStudentResponse { Response = false });
            return Task.FromResult(new AddStudentResponse { Response = true });
        }

        public override Task<ResponseInfo> GetStudentInfo(GetStudentInfoRequest request, ServerCallContext context)
        {
            return Task.FromResult(students);
        }
    }
}

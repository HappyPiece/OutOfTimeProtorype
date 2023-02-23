using OutOfTimePrototype.DAL.Models;
using System.Net;

namespace OutOfTimePrototype.Utilities
{
    public static class ClassUtilities
    {
        public class ClassOperationResult
        {
            public OperationStatus? Status { get; set; }
            public string? Message { get; set; }
            public List<Class>? QueryResult { get; set; }
            public HttpStatusCode HttpStatusCode { get; set; }
            public ClassOperationResult(OperationStatus operationStatus, HttpStatusCode httpStatusCode, 
                string? errorMessage = null, List<Class>? queryResult = null)
            {
                Status = operationStatus;
                Message = errorMessage;
                HttpStatusCode = httpStatusCode;
                QueryResult = queryResult;
            }
            

            public enum OperationStatus
            {
                Success,
                ClassCreated,
                ClassEdited,
                ClassDeleted,
                TimeSlotNotFound,
                ClusterNotFound,
                ClusterOccupied,
                LectureHallNotFound,
                LectureHallOccupied,
                EducatorNotFound,
                EducatorOccupied,
                ClassTypeNotFound,
                ClassNotFound,
                UnspecifiedDate,
                UnspecifiedCluster,
                UnspecifiedTimeSlot
            }

            /// <summary>
            /// Creates default ClassOperationResult based on what status was supplied as first parameter.
            /// Second parameter is evaluated according to the value of the first and is used to create describeful message.
            /// </summary>
            /// <param name="status"></param>
            /// <param name="arg"></param>
            /// <param name="queryResult"></param>
            /// <returns></returns>
            public static ClassOperationResult GenerateDefaultOperationResult(OperationStatus status, string? arg = null, List<Class>? queryResult = null)
            {
                string message = "";
                HttpStatusCode httpStatusCode = HttpStatusCode.InternalServerError;
                switch (status)
                {
                    case OperationStatus.Success:
                        {
                            message = "Operation successful";
                            httpStatusCode = HttpStatusCode.OK;
                            break;
                        }
                    case OperationStatus.ClassCreated:
                        {
                            message = $"Class was successfully created, Id {arg}";
                            httpStatusCode = HttpStatusCode.Created;
                            break;
                        }
                    case OperationStatus.ClassEdited:
                        {
                            message = $"Class with id {arg} was successfully edited";
                            httpStatusCode = HttpStatusCode.OK;
                            break;
                        }
                    case OperationStatus.ClassDeleted:
                        {
                            message = $"Class with id {arg} was successfully deleted";
                            httpStatusCode = HttpStatusCode.OK;
                            break;
                        }
                    case OperationStatus.TimeSlotNotFound:
                        {
                            message = $"Time slot number '{arg}' does not exist";
                            httpStatusCode = HttpStatusCode.NotFound;
                            break;
                        }
                    case OperationStatus.ClusterNotFound:
                        {
                            message = $"Cluster number '{arg}' does not exist";
                            httpStatusCode = HttpStatusCode.NotFound;
                            break;
                        }
                    case OperationStatus.ClusterOccupied:
                        {
                            message = $"Cluster number '{arg}' is occupied at specified time";
                            httpStatusCode = HttpStatusCode.Conflict;
                            break;
                        }
                    case OperationStatus.LectureHallNotFound:
                        {
                            message = $"Lecture hall with id '{arg}' does not exist";
                            httpStatusCode = HttpStatusCode.NotFound;
                            break;
                        }
                    case OperationStatus.LectureHallOccupied:
                        {
                            message = $"Lecture hall with id '{arg}' is occupied at specified time";
                            httpStatusCode = HttpStatusCode.Conflict;
                            break;
                        }
                    case OperationStatus.EducatorNotFound:
                        {
                            message = $"Educator with id '{arg}' does not exist";
                            httpStatusCode = HttpStatusCode.NotFound;
                            break;
                        }
                    case OperationStatus.EducatorOccupied:
                        {
                            message = $"Educator with id '{arg}' is occupied at specified time";
                            httpStatusCode = HttpStatusCode.Conflict;
                            break;
                        }
                    case OperationStatus.ClassTypeNotFound:
                        {
                            message = $"Class type with name '{arg}' does not exist";
                            httpStatusCode = HttpStatusCode.NotFound;
                            break;
                        }
                    case OperationStatus.ClassNotFound:
                        {
                            message = $"Class with id '{arg}' does not exist";
                            httpStatusCode = HttpStatusCode.NotFound;
                            break;
                        }
                    case OperationStatus.UnspecifiedDate:
                        {
                            message = $"Date property was not supplied";
                            httpStatusCode = HttpStatusCode.BadRequest;
                            break;
                        }
                    case OperationStatus.UnspecifiedCluster:
                        {
                            message = $"Cluster property was not supplied";
                            httpStatusCode = HttpStatusCode.BadRequest;
                            break;
                        }
                    case OperationStatus.UnspecifiedTimeSlot:
                        {
                            message = $"TimeSlot property was not supplied";
                            httpStatusCode = HttpStatusCode.BadRequest;
                            break;
                        }
                }
                return new ClassOperationResult(status, httpStatusCode, errorMessage: message, queryResult: queryResult);
            }
        }
    }
}
